using System;
using System.ComponentModel.DataAnnotations;

namespace Sirkadirov.Overtest.WebApplication.Areas.Administration.Models.Social.UserGroupsAdministrationController
{
    
    public class UserGroupRemovalModel
    {
        
        public Guid UserGroupId { get; set; }
        
        [Required, DataType(DataType.Password)]
        public string PasswordConfirmation { get; set; }
        
    }
    
}