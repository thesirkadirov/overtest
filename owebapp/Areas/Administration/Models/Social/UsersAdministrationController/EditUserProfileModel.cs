using System;
using System.ComponentModel.DataAnnotations;

namespace Sirkadirov.Overtest.WebApplication.Areas.Administration.Models.Social.UsersAdministrationController
{
    public class EditUserProfileModel
    {
        [Required]
        public Guid UserId { get; set; }
        
        [Required, DataType(DataType.Text), MinLength(3), MaxLength(255)]
        public string FullName { get; set; }
        
        [MinLength(0), MaxLength(255)]
        public string InstitutionName { get; set; }
    }
}