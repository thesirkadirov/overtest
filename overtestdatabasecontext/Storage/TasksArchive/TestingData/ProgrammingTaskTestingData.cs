using System;
using System.Collections.Generic;

namespace Sirkadirov.Overtest.Libraries.Shared.Database.Storage.TasksArchive.TestingData
{
    
    public class ProgrammingTaskTestingData
    {
        
        public Guid Id { get; set; }
        
        public byte[] TestingDataPackageFile { get; set; }
        
        public ProgrammingTask ProgrammingTask { get; set; }
        
    }
    
}