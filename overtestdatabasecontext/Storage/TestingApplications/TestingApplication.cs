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
        
        public TestingApplicationSourceCode SourceCode { get; set; }
        public ApplicationTestingType TestingType { get; set; }
        
        public ApplicationStatus Status { get; set; }
        
        public User Author { get; set; }
        public Guid AuthorId { get; set; }

        public Competition Competition { get; set; }
        public Guid? CompetitionId { get; set; }
        
        public ProgrammingTask ProgrammingTask { get; set; }
        public Guid ProgrammingTaskId { get; set; }
        
        public TestingApplicationResult TestingResult { get; set; }
        
        public enum ApplicationTestingType
        {
            SyntaxMode = 1,
            DebugMode = 50,
            ReleaseMode = 100
        }

        public enum ApplicationStatus
        {
            Waiting = 1,
            Selected = 2,
            
            AutomaticJudging = 90,
            ManualJudging = 91,
            
            Verified = 100
        }
        
    }
}