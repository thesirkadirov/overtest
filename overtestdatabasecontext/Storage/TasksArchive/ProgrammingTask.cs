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
        public DateTime LastModified { get; set; }
        
        public bool VisibleInFreeMode { get; set; }
        public bool VisibleInCompetitionMode { get; set; }
        
        [Required, MinLength(1), MaxLength(255)]
        public string Title { get; set; }
        
        [Required, MinLength(1)]
        public string Description { get; set; }
        
        [Required, Range(typeof(byte), "0", "100")]
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