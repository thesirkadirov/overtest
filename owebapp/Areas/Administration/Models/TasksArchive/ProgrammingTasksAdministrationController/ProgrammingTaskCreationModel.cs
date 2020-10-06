using System.ComponentModel.DataAnnotations;

namespace Sirkadirov.Overtest.WebApplication.Areas.Administration.Models.TasksArchive.ProgrammingTasksAdministrationController
{
    public class ProgrammingTaskCreationModel
    {
        [Required, MinLength(1), MaxLength(255)]
        public string Title { get; set; }
        
        public bool VisibleInFreeMode { get; set; }
        public bool VisibleInCompetitionMode { get; set; }
    }
}