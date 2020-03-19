using System;
using System.Collections.Generic;

namespace Sirkadirov.Overtest.Libraries.Shared.Database.Storage.TasksArchive
{
    
    public class ProgrammingTaskCategory
    {
        
        public Guid Id { get; set; }
        
        public string DisplayName { get; set; }
        public string Description { get; set; }
        
        /*
         * Relationships
         */
        
        public List<ProgrammingTask> ProgrammingTasks { get; set; }
        
    }
    
}