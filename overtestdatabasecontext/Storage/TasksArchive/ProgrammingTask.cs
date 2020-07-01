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
        public DateTime LastModification { get; set; }

        public bool Enabled { get; set; }
        
        public string Title { get; set; }
        public string Description { get; set; }
        
        [Range(typeof(byte), "0", "100")]
        public byte Difficulty { get; set; }
        
        public ProgrammingTaskTestingData TestingData { get; set; }
        
        /*
         * Relationships
         */
        
        public ProgrammingTaskCategory Category { get; set; }
        public Guid? CategoryId { get; set; }
        
        /*
         * Owned classes
         */
        
        [Serializable]
        public class ProgrammingTaskTestingData
        {
            
            public byte[] DataPackageFile { get; set; }
            public string DataPackageHash { get; set; }
            
        }
        
    }
    
}