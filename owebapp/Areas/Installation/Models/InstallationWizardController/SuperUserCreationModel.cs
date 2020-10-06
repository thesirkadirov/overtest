using System.ComponentModel.DataAnnotations;

namespace Sirkadirov.Overtest.WebApplication.Areas.Installation.Models.InstallationWizardController
{
    
    public class SuperUserCreationModel
    {
        
        [Required]
        [MinLength(2), MaxLength(255)]
        [DataType(DataType.Text)]
        public string FullName { get; set; }
        
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(PasswordRepeat))]
        public string Password { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string PasswordRepeat { get; set; }

        [Required]
        [MinLength(2), MaxLength(255)]
        [DataType(DataType.Text)]
        public string InstitutionName { get; set; }

    }
    
}