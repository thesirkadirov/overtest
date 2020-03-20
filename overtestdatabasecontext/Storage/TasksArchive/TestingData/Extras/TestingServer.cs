using System;

namespace Sirkadirov.Overtest.Libraries.Shared.Database.Storage.TasksArchive.TestingData.Extras
{
    
    public class TestingServer
    {
        
        public Guid Fingerprint { get; set; }
        
        public string DisplayName { get; set; }
        
        public DateTime LastStartupTime { get; set; }
        public DateTime LastOperationTime { get; set; }
        
        public string Platform { get; set; }
        public int ThreadsCount { get; set; }
        
    }
    
}