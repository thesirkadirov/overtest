using System;
using System.ComponentModel.DataAnnotations;

namespace Sirkadirov.Overtest.WebApplication.Areas.Administration.Models.Social.UsersAdministrationController
{
    public class ChangeUserPasswordModel
    {
        public Guid UserId { get; set; }
        
        [DataType(DataType.Password), MaxLength(255)]
        public string OldPassword { get; set; }
        
        [Required, DataType(DataType.Password), MaxLength(255)]
        [Compare(nameof(NewPasswordConfirmation))]
        public string NewPassword { get; set; }
        
        [Required, DataType(DataType.Password), MaxLength(255)]
        public string NewPasswordConfirmation { get; set; }
    }
}