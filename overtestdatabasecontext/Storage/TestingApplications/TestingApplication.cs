using System;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Competitions;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Identity;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.TasksArchive;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.TestingApplications.Extras;

namespace Sirkadirov.Overtest.Libraries.Shared.Database.Storage.TestingApplications
{
    
    public class TestingApplication
    {
        
        public Guid Id { get; set; }
        
        public DateTime Created { get; set; }
        public TimeSpan ProcessingTime { get; set; }
        
        public ApplicationTestingType TestingType { get; set; }
        public ApplicationStatus Status { get; set; }
        
        public TestingApplicationSourceCode SourceCode { get; set; }
        public ApplicationTestingResults TestingResults { get; set; }
        
        public User Author { get; set; }
        public Guid AuthorId { get; set; }

        public Competition Competition { get; set; }
        public Guid? CompetitionId { get; set; }
        
        public ProgrammingTask ProgrammingTask { get; set; }
        public Guid ProgrammingTaskId { get; set; }
        
        public enum ApplicationTestingType
        {
            SyntaxMode,
            DebugMode,
            ReleaseMode
        }

        public enum ApplicationStatus
        {
            Waiting,
            
            Selected,
            Preparation,
            Compilation,
            Testing,
            
            AutomaticJudging,
            ManualJudging,
            
            Verified
        }

        public class ApplicationTestingResults
        {
            public string RawTestingResults { get; set; }
            
            public int PassedTestsCount { get; set; }
        }
        
    }
    
}