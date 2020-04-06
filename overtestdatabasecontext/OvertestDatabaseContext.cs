using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Competitions;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Competitions.Extras;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Identity;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.TasksArchive;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.TasksArchive.TestingData;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.TasksArchive.TestingData.Extras;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.TestingApplications;

namespace Sirkadirov.Overtest.Libraries.Shared.Database
{
    
    // ReSharper disable once ClassNeverInstantiated.Global
    public class OvertestDatabaseContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        
        // ReSharper disable once UnusedMember.Global
        public DbSet<UserGroup> UserGroups { get; set; }
        // ReSharper disable once UnusedMember.Global
        public DbSet<UserPhoto> UserPhotos { get; set; }
        
        public DbSet<ProgrammingTask> ProgrammingTasks { get; set; }
        /* ||=> */ public DbSet<ProgrammingTaskCategory> ProgrammingTaskCategories { get; set; }
        /* ||=> */ public DbSet<ProgrammingTaskTestingData> ProgrammingTaskTestingDatas { get; set; }
        
        public DbSet<ProgrammingLanguage> ProgrammingLanguages { get; set; }
        
        public DbSet<TestingApplication> TestingApplications { get; set; }
        
        public DbSet<Competition> Competitions { get; set; }
        /* ||=> */ public DbSet<CompetitionProgrammingTask> CompetitionProgrammingTasks { get; set; }
        /* ||=> */ public DbSet<CompetitionUser> CompetitionUsers { get; set; }

        public OvertestDatabaseContext(DbContextOptions options) : base(options) { }

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
                
                /*
                 * Relationships
                 */
                
                entity.HasOne(u => u.Curator)
                    .WithMany()
                    .HasForeignKey(u => u.CuratorId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(u => u.UserGroup)
                    .WithMany(g => g.Users)
                    .HasForeignKey(u => u.UserGroupId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                // UserPhoto defined in [UserPhoto]
                
            });
            
            /*
             * [UserGroup] entity
             */

            modelBuilder.Entity<UserGroup>(entity =>
            {

                entity.HasKey(g => g.Id);
                
                entity.HasIndex(g => new {g.CuratorId, Title = g.DisplayName}).IsUnique();
                
                /*
                 * Relationships
                 */

                entity.HasOne(g => g.Curator)
                    .WithMany()
                    .HasForeignKey(g => g.CuratorId)
                    .OnDelete(DeleteBehavior.Cascade);

            });
            
            /*
             * [UserPhoto] entity
             */

            modelBuilder.Entity<UserPhoto>(entity =>
            {
                
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Source).IsRequired();
                
                /*
                 * Relationships
                 */
                
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
                entity.Property(t => t.LastTestingDataModification).IsRequired();

                entity.Property(t => t.Enabled).HasDefaultValue(true).IsRequired();
                
                entity.Property(t => t.Title).IsUnicode().IsRequired();
                entity.Property(t => t.Description).IsUnicode().IsRequired();

                entity.Property(t => t.Difficulty).HasDefaultValue(100).IsRequired();
                
                /*
                 * Relationships
                 */

                entity.HasOne(t => t.Category)
                    .WithMany(c => c.ProgrammingTasks)
                    .HasForeignKey(t => t.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);
                
            });
            
            /*
             * [ProgrammingTaskTestingData] entity
             */

            modelBuilder.Entity<ProgrammingTaskTestingData>(entity =>
            {
                
                entity.HasKey(d => d.Id);

                entity.Property(d => d.DataPackageFile).IsRequired();
                entity.Property(d => d.DataPackageHash).IsRequired();
                
                /*
                 * Relationships
                 */
                
                entity.HasOne(d => d.ProgrammingTask)
                    .WithOne(d => d.TestingData)
                    .HasForeignKey<ProgrammingTaskTestingData>(d => d.ProgrammingTaskId)
                    .OnDelete(DeleteBehavior.Cascade);

            });
            
            /*
             * ProgrammingTaskCategory] entity
             */
            
            modelBuilder.Entity<ProgrammingTaskCategory>(entity =>
            {
                
                entity.HasKey(c => c.Id);
                
                entity.Property(c => c.DisplayName).IsUnicode().IsRequired();
                entity.Property(c => c.Description).IsUnicode().HasDefaultValue().IsRequired(false);
                
                /*
                 * Relationships
                 */
                
                // ProgrammingTasks list defined in [ProgrammingTask]
                
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
                entity.Property(a => a.ProcessingTime).HasDefaultValue(TimeSpan.Zero).IsRequired();

                entity.Property(a => a.TestingType).IsRequired();
                entity.Property(a => a.Status).HasDefaultValue(TestingApplication.ApplicationStatus.Waiting).IsRequired();
                
                /*
                 * [TestingApplicationSourceCode] owned class
                 */
                
                entity.OwnsOne(a => a.SourceCode, builder =>
                {
                    builder.Property(c => c.SourceCode).IsUnicode().IsRequired();
                    
                    builder.HasOne(c => c.ProgrammingLanguage)
                        .WithMany()
                        .HasForeignKey(c => c.ProgrammingLanguageId)
                        .OnDelete(DeleteBehavior.Cascade);
                    
                });
                
                /*
                 * [ApplicationTestingResults] owned class
                 */
                
                entity.OwnsOne(a => a.TestingResults, builder =>
                {
                    builder.Property(r => r.RawTestingResults).HasDefaultValue();
                    builder.Property(r => r.CompletionPercentage).HasDefaultValue(0).IsRequired();
                });
                
                /*
                 * Other relationships
                 */
                
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
                
                /*
                 * Relationships
                 */
                
                entity.HasOne(c => c.Curator)
                    .WithMany()
                    .HasForeignKey(c => c.CuratorId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                // CompetitionProgrammingTasks list defined in [CompetitionProgrammingTask]

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
                
                /*
                 * Relationships
                 */
                
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
                
                /*
                 * Relationships
                 */

                entity.HasOne(u => u.Competition)
                    .WithMany(c => c.CompetitionUsers)
                    .HasForeignKey(u => u.CompetitionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(u => u.User)
                    .WithMany()
                    .HasForeignKey(u => u.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

            });
            
        }

        public async Task InitializeDatabaseAsync()
        {
            
            if (await Database.EnsureCreatedAsync())
            {
                /* Apply default data */
            }
            
        }
        
    }
    
}