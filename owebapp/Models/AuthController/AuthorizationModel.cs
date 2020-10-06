using System.ComponentModel.DataAnnotations;

namespace Sirkadirov.Overtest.WebApplication.Models.AuthController
{
    
    public class AuthorizationModel
    {
        
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [Required]
        public bool RememberMe { get; set; }
        
        public string ReturnUrl { get; set; }
        
    }
    
}