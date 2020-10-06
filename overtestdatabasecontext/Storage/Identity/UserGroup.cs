using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Identity
{
    
    public class UserGroup
    {
        
        public Guid Id { get; set; }
        
        [Required, MinLength(2), MaxLength(100)]
        public string DisplayName { get; set; }
        
        [MaxLength(255)]
        public string AccessToken { get; set; }
        
        public User GroupCurator { get; set; }
        public Guid GroupCuratorId { get; set; }
        
        public List<User> Users { get; set; }
        
    }
    
}