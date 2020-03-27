using System;
using System.ComponentModel.DataAnnotations;

namespace Sirkadirov.Overtest.Libraries.Shared.Database.Storage.TasksArchive
{
    
    public class ProgrammingTask
    {
        
        public Guid Id { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Created { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime LastTestingDataModification { get; set; }
        
        public bool Enabled { get; set; }
        
        public string Title { get; set; }
        public string Description { get; set; }
        
        [Range(typeof(byte), "0", "100")]
        public byte Difficulty { get; set; }
        
        public byte[] TestingDataPackageFile { get; set; }
        
        /*
         * Relationships
         */
        
        public ProgrammingTaskCategory Category { get; set; }
        public Guid CategoryId { get; set; }
        
    }
    
}