using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sirkadirov.Overtest.Libraries.Shared.Database.Storage.TasksArchive
{
    
    public class ProgrammingTaskCategory
    {
        
        public Guid Id { get; set; }
        
        [Required, MinLength(3), MaxLength(100)]
        public string DisplayName { get; set; }
        
        [MinLength(0), MaxLength(255)]
        public string Description { get; set; }
        
        /*
         * Relationships
         */
        
        public List<ProgrammingTask> ProgrammingTasks { get; set; }
        
    }
    
}