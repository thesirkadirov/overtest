using System;
using System.ComponentModel.DataAnnotations;

namespace Sirkadirov.Overtest.WebApplication.Areas.Administration.Models.TasksArchive.ProgrammingTasksAdministrationController
{
    public class ProgrammingTaskEditDetailsModel
    {
        [Required]
        public Guid ProgrammingTaskId { get; set; }
        
        [Required, MinLength(1), MaxLength(255)]
        public string Title { get; set; }
        
        [Required, MinLength(1)]
        public string Description { get; set; }
        
        [Required, Range(typeof(byte), "0", "100")]
        public byte Difficulty { get; set; }
        
        [Required]
        public bool VisibleInFreeMode { get; set; }
        
        [Required]
        public bool VisibleInCompetitionMode { get; set; }
        
        public Guid? CategoryId { get; set; }
    }
}