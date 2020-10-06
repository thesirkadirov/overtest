using System;
using System.ComponentModel.DataAnnotations;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.TestingApplications;

namespace Sirkadirov.Overtest.WebApplication.Areas.TasksArchive.ViewComponents.Models.ProgrammingTasksBrowsingController
{
    public class TestingApplicationCreationPartialModel
    {
        [Required]
        public Guid ProgrammingTaskId { get; set; }

        [Required]
        public TestingApplication.ApplicationTestingType? TestingType { get; set; }
        
        [Required]
        [MaxLength(65535)]
        public string SourceCode { get; set; }
        
        [Required]
        public Guid? ProgrammingLanguageId { get; set; }
    }
}