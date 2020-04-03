using System;

namespace Sirkadirov.Overtest.Libraries.Shared.Database.Storage.TasksArchive.TestingData
{
    
    public class ProgrammingTaskTestingData
    {
        
        public Guid Id { get; set; }
        
        public byte[] DataPackageFile { get; set; }
        public string DataPackageHash { get; set; }
        
        public ProgrammingTask ProgrammingTask { get; set; }
        public Guid ProgrammingTaskId { get; set; }
        
    }
    
}