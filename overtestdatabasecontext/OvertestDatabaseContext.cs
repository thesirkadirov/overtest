using System;
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
        
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DbSet<UserGroup> UserGroups { get; set; }
        
        public DbSet<ProgrammingTask> ProgrammingTasks { get; set; }
        /* ||=> */ public DbSet<ProgrammingTaskCategory> ProgrammingTaskCategories { get; set; }
        /* ||=> */ public DbSet<ProgrammingTaskTestingData> ProgrammingTasksTestingData { get; set; }
        
        public DbSet<TestingServer> TestingServers { get; set; }
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
                
                entity
                    .HasOne(u => u.Curator)
                    .WithMany()
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasForeignKey(u => u.CuratorId);
                
                entity
                    .HasOne(u => u.UserGroup)
                    .WithMany(g => g.Users)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasForeignKey(u => u.UserGroupId);
                
                // UserGroups list defined in [UserGroup]
                
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
                
                entity
                    .HasOne(g => g.Curator)
                    .WithMany(u => u.UserGroups)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasForeignKey(g => g.CuratorId);
                
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

                entity.Property(t => t.RatingPerTest).HasDefaultValue(1).IsRequired();
                entity.Property(t => t.TestsCount).HasDefaultValue(1).IsRequired();
                
                /*
                 * Relationships
                 */
                
                entity
                    .HasOne(t => t.Category)
                    .WithMany(c => c.ProgrammingTasks)
                    .HasForeignKey(t => t.CategoryId);
                
                entity
                    .HasOne(t => t.TestingData)
                    .WithOne(d => d.ProgrammingTask)
                    .HasForeignKey<ProgrammingTask>(t => t.TestingDataId);
                
            });
            
            /*
             * [ProgrammingTaskTestingData] entity
             */
            
            modelBuilder.Entity<ProgrammingTaskTestingData>(entity =>
            {
                
                entity.HasKey(d => d.Id);

                entity.Property(d => d.TestingDataPackageFile).HasDefaultValue().IsRequired(true);
                
                /*
                 * Relationships
                 */
                
                // ProgrammingTask link defined in [ProgrammingTask]
                
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
             * [TestingServer] entity
             */

            modelBuilder.Entity<TestingServer>(entity =>
            {
                
                entity.HasKey(s => s.Fingerprint);

                entity.Property(s => s.DisplayName).IsUnicode().HasDefaultValue().IsRequired(false);

                entity.Property(s => s.LastStartupTime).HasDefaultValue(DateTime.MinValue).IsRequired();
                entity.Property(s => s.LastOperationTime).HasDefaultValue(DateTime.MinValue).IsRequired();

                entity.Property(s => s.Platform).IsRequired();
                entity.Property(s => s.ThreadsCount).IsRequired();

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
                
                entity.OwnsOne(a => a.SourceCode);

                entity.Property(a => a.SourceCode.SourceCode).IsUnicode().IsRequired();
                    
                entity.HasOne(a => a.SourceCode.ProgrammingLanguage)
                    .WithMany()
                    .HasForeignKey(a => a.SourceCode.ProgrammingLanguageId);
                
                /*
                 * [ApplicationTestingResults] owned class
                 */
                
                entity.OwnsOne(a => a.TestingResults);
                
                entity.Property(a => a.TestingResults.RawTestingResults).HasDefaultValue();
                entity.Property(a => a.TestingResults.PassedTestsCount).IsRequired();
                
                /*
                 * Other relationships
                 */
                
                entity
                    .HasOne(a => a.Author)
                    .WithMany()
                    .HasForeignKey(a => a.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity
                    .HasOne(a => a.Competition)
                    .WithMany()
                    .HasForeignKey(a => a.CompetitionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity
                    .HasOne(a => a.ProgrammingTask)
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
                entity.Property(c => c.WaitingPageActivationTime).IsRequired();
                
                /*
                 * Relationships
                 */
                
                entity
                    .HasOne(c => c.Curator)
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
                
                entity
                    .HasOne(t => t.ProgrammingTask)
                    .WithMany()
                    .HasForeignKey(t => t.ProgrammingTaskId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity
                    .HasOne(t => t.Competition)
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
                
                // TODO!
                
            });

        }
        
    }
    
}