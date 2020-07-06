using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Sirkadirov.Overtest.WebApplication.Areas.Administration.Models.TasksArchive.ProgrammingTasksAdministrationController
{
    public class ProgrammingTaskEditTestingDataModel
    {
        [Required]
        public Guid ProgrammingTaskId { get; set; }
        
        [Required, DataType(DataType.Upload)]
        public IFormFile TestingDataFile { get; set; }
    }
}