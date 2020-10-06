using System;
using System.Data;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Sirkadirov.Overtest.Libraries.Shared.Database.Operators;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Competitions;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Competitions.Extras;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Identity;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.TasksArchive;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.TasksArchive.Extras;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.TestingApplications;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
namespace Sirkadirov.Overtest.Libraries.Shared.Database
{
    
    // ReSharper disable once ClassNeverInstantiated.Global
    public class OvertestDatabaseContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        
        /* ===== Database sets ===== */
        
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<UserPhoto> UserPhotos { get; set; }
        
        public DbSet<ProgrammingTask> ProgrammingTasks { get; set; }
        /* ||=> */ public DbSet<ProgrammingTaskCategory> ProgrammingTaskCategories { get; set; }
        
        public DbSet<ProgrammingLanguage> ProgrammingLanguages { get; set; }
        
        public DbSet<TestingApplication> TestingApplications { get; set; }
        public DbSet<TestingApplication> TestingApplicationsResults { get; set; }
        
        public DbSet<Competition> Competitions { get; set; }
        /* ||=> */ public DbSet<CompetitionProgrammingTask> CompetitionProgrammingTasks { get; set; }
        /* ||=> */ public DbSet<CompetitionUser> CompetitionUsers { get; set; }
        
        public DbSet<ConfigurationStorage.ConfigurationKeyValuePair> SystemConfigurationStore { get; set; }
        
        /* ===== Constructors ===== */
        
        public OvertestDatabaseContext()
        {
            Initialize();
        }
        
        public OvertestDatabaseContext(DbContextOptions options) : base(options)
        {
            Initialize();
        }
        
        /* ===== Internal methods and features ===== */
        
        public ConfigurationStorage SystemConfiguration { get; private set; }
        
        public OvertestUserPermissionsOperator UserPermissionsOperator { get; private set; }
        
        private void Initialize()
        {
            SystemConfiguration = new ConfigurationStorage(this);
            UserPermissionsOperator = new OvertestUserPermissionsOperator(this);
        }
        
        /* ===== Overrides ===== */
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            base.OnConfiguring(optionsBuilder);

            const string databaseConfigurationFileName = "overtest.database.config.json";

            var databaseConfiguration = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile(databaseConfigurationFileName, false, false)
                .Build();
            
            var databaseConnectionString = databaseConfiguration.GetValue<string>("database:connection_string");
            
            // To workaround https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/projects
            const string migrationsAssembly = "owebapp";
            
            switch (databaseConfiguration.GetValue<string>("database:provider").ToUpper())
            {
                
                case "MARIADB":
                case "MYSQL":
                    optionsBuilder.UseMySql(databaseConnectionString, builder =>
                    {
                        builder.CharSet(CharSet.Utf32);
                        builder.MigrationsAssembly(migrationsAssembly);
                    });
                    break;
                
                case "SQLSERVER":
                    optionsBuilder.UseSqlServer(databaseConnectionString, builder =>
                    {
                        builder.MigrationsAssembly(migrationsAssembly);
                    });
                    break;
                
                default:
                    throw new DataException("You are using an unknown database provider!");
                
            }
            
            // FOR DEBUG PURPOSES ONLY!
            //optionsBuilder.EnableDetailedErrors();
            //optionsBuilder.EnableSensitiveDataLogging();

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            base.OnModelCreating(modelBuilder);
            
            /* ===== [Identity] section ===== */
            
            /*
             * [User] entity
             */

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();

                entity.Property(u => u.FullName).IsUnicode().IsRequired();
                entity.Property(u => u.InstitutionName).IsUnicode().HasDefaultValue();

                entity.Property(u => u.Type).IsRequired();
                
                entity.Property(u => u.IsBanned).HasDefaultValue(false);
                
                entity.Property(u => u.Registered).HasDefaultValue(DateTime.MinValue).IsRequired();
                entity.Property(u => u.LastSeen).HasDefaultValue(DateTime.MinValue).IsRequired();

                entity.HasOne(u => u.UserGroup)
                    .WithMany(g => g.Users)
                    .HasForeignKey(g => g.UserGroupId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            
            /*
             * [UserGroup] entity
             */
            
            modelBuilder.Entity<UserGroup>(entity =>
            {
                entity.HasKey(g => g.Id);
                
                entity.Property(g => g.DisplayName).IsUnicode();
                
                entity.Property(g => g.AccessToken)
                    .HasMaxLength(255)
                    .HasDefaultValue();
                
                entity.HasOne(g => g.GroupCurator)
                    .WithMany(u => u.CuratedUserGroups)
                    .HasForeignKey(g => g.GroupCuratorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            
            /*
             * [UserPhoto] entity
             */

            modelBuilder.Entity<UserPhoto>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Source).IsRequired();
                
                entity.HasOne(p => p.User)
                    .WithOne(u => u.UserPhoto)
                    .HasForeignKey<User>(u => u.UserPhotoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            
            /* ===== [TasksArchive] section ===== */
            
            /*
             * [ProgrammingTask] entity
             */
            
            modelBuilder.Entity<ProgrammingTask>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Created).IsRequired();
                entity.Property(t => t.LastModified).IsRequired();

                entity.Property(t => t.VisibleInFreeMode).HasDefaultValue(true).IsRequired();
                entity.Property(t => t.VisibleInCompetitionMode).HasDefaultValue(true).IsRequired();
                
                entity.Property(t => t.Title).IsUnicode().IsRequired();
                entity.Property(t => t.Description).IsUnicode().IsRequired();

                entity.Property(t => t.Difficulty).HasDefaultValue(0).IsRequired();

                entity.OwnsOne(t => t.TestingData, ownedEntity =>
                {
                    ownedEntity.Property(d => d.DataPackageFile).HasDefaultValue().IsRequired();
                    ownedEntity.Property(d => d.DataPackageHash).HasDefaultValue(string.Empty).IsRequired();
                });
                
                /*
                 * Relationships
                 */
                
                entity.HasOne(t => t.Category)
                    .WithMany(c => c.ProgrammingTasks)
                    .HasForeignKey(t => t.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
            
            /*
             * ProgrammingTaskCategory] entity
             */
            
            modelBuilder.Entity<ProgrammingTaskCategory>(entity =>
            {
                entity.HasKey(c => c.Id);
                
                entity.Property(c => c.DisplayName).IsUnicode().IsRequired();
                entity.Property(c => c.Description).IsUnicode().HasDefaultValue().IsRequired(false);
            });
            
            /*
             * [ProgrammingLanguage] entity
             */
            
            modelBuilder.Entity<ProgrammingLanguage>(entity =>
            {
                entity.HasKey(l => l.Id);

                entity.Property(l => l.DisplayName).IsUnicode().IsRequired();
                entity.Property(l => l.Description).IsUnicode().HasDefaultValue().IsRequired(false);
                
                entity.Property(l => l.SyntaxHighlightingOptions).HasDefaultValue().IsRequired(false);
            });
            
            /* ===== [TestingApplications] section ===== */
            
            /*
             * [TestingApplication] entity
             */

            modelBuilder.Entity<TestingApplication>(entity =>
            {
                entity.HasKey(a => a.Id);

                entity.Property(a => a.Created).IsRequired();

                entity.Property(a => a.TestingType).IsRequired();
                entity.Property(a => a.Status)
                    .HasDefaultValue(TestingApplication.ApplicationStatus.Waiting)
                    .IsConcurrencyToken()
                    .IsRequired();
                
                entity.OwnsOne(a => a.SourceCode, builder =>
                {
                    builder.Property(c => c.SourceCode).IsUnicode().IsRequired();
                    
                    builder.HasOne(c => c.ProgrammingLanguage)
                        .WithMany()
                        .HasForeignKey(c => c.ProgrammingLanguageId)
                        .OnDelete(DeleteBehavior.Cascade);
                    
                });
                
                entity.HasOne(a => a.Author)
                    .WithMany()
                    .HasForeignKey(a => a.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.Competition)
                    .WithMany()
                    .HasForeignKey(a => a.CompetitionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.ProgrammingTask)
                    .WithMany()
                    .HasForeignKey(a => a.ProgrammingTaskId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            
            /*
             * [TestingApplicationResult] entity
             */
            
            modelBuilder.Entity<TestingApplicationResult>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.Property(r => r.ProcessingTime).IsRequired();
                
                entity.Property(r => r.RawTestingResults).HasDefaultValue().IsRequired();
                
                entity.Property(r => r.GivenDifficulty).IsRequired();
                entity.Property(r => r.SolutionAdjudgement).IsRequired();

                entity.HasOne(r => r.TestingApplication)
                    .WithOne(a => a.TestingResult)
                    .HasForeignKey<TestingApplicationResult>(r => r.TestingApplicationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            /* ===== [Competitions] section ===== */
            
            /*
             * [Competition] entity
             */

            modelBuilder.Entity<Competition>(entity =>
            {
                
                entity.HasKey(c => c.Id);

                entity.Property(c => c.VisibleTo).HasDefaultValue(Competition.Visibility.Hidden).IsRequired();
                
                entity.Property(c => c.Title).IsUnicode().IsRequired();
                entity.Property(c => c.Description).IsUnicode().IsRequired();
                
                
                entity.Property(c => c.Created).IsRequired();
                entity.Property(c => c.Starts).IsRequired();
                entity.Property(c => c.Ends).IsRequired();
                
                entity.Property(c => c.PinCodeEnabled).HasDefaultValue(false).IsRequired();
                entity.Property(c => c.PinCodeClearText).HasDefaultValue().IsRequired();
                
                entity.Property(c => c.UserExitEnabled).HasDefaultValue(false).IsRequired();
                entity.Property(c => c.UserExitAction).HasDefaultValue(Competition.CompetitionExitAction.Default).IsRequired();
                
                entity.Property(c => c.EnableWaitingPage).HasDefaultValue(false).IsRequired();
                entity.Property(c => c.WaitingPageActivationTime).HasDefaultValue(DateTime.MinValue).IsRequired();
                
                entity.HasOne(c => c.Curator)
                    .WithMany()
                    .HasForeignKey(c => c.CuratorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            
            /*
             * [CompetitionProgrammingTask] entity
             */
            
            modelBuilder.Entity<CompetitionProgrammingTask>(entity =>
            {
                
                entity.HasKey(t => new {t.CompetitionId, t.ProgrammingTaskId});

                entity.Property(t => t.JudgementType)
                    .HasDefaultValue(CompetitionProgrammingTask.ProgrammingTaskJudgementType.CompleteSolution)
                    .IsRequired();
                
                entity.HasOne(t => t.ProgrammingTask)
                    .WithMany()
                    .HasForeignKey(t => t.ProgrammingTaskId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(t => t.Competition)
                    .WithMany(c => c.CompetitionProgrammingTasks)
                    .HasForeignKey(t => t.CompetitionId)
                    .OnDelete(DeleteBehavior.Cascade);

            });
            
            /*
             * [CompetitionUser] entity
             */

            modelBuilder.Entity<CompetitionUser>(entity =>
            {
                entity.HasKey(u => new {u.CompetitionId, u.UserId});
                
                entity.HasOne(u => u.Competition)
                    .WithMany(c => c.CompetitionUsers)
                    .HasForeignKey(u => u.CompetitionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(u => u.User)
                    .WithMany()
                    .HasForeignKey(u => u.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            
            /* ===== [System] section ===== */
            
            /*
             * [ConfigurationStorage] entity
             */
            
            modelBuilder.Entity<ConfigurationStorage.ConfigurationKeyValuePair>(entity =>
            {
                entity.HasKey(s => s.Key);
                entity.Property(s => s.Value);
            });
            
        }
        
    }
    
}