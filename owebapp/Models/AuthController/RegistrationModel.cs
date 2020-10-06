using System.ComponentModel.DataAnnotations;
namespace Sirkadirov.Overtest.WebApplication.Models.AuthController
{
    
    public class RegistrationModel
    {
        
        [Required]
        [MinLength(3), MaxLength(255)]
        public string FullName { get; set; }
        
        [Required, EmailAddress]
        public string Email { get; set; }
        
        [Required, Compare(nameof(PasswordRepeat))]
        public string Password { get; set; }
        
        [Required, Compare(nameof(Password))]
        public string PasswordRepeat { get; set; }
        
        [Required]
        public string SecurityToken { get; set; }
        
    }
    
}