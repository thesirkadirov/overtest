using System;
using System.Collections.Generic;

namespace Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Identity
{
    
    public class UserGroup
    {
        
        public Guid Id { get; set; }
        
        public string DisplayName { get; set; }

        public string AccessToken { get; set; }
        
        public User GroupCurator { get; set; }
        public Guid GroupCuratorId { get; set; }
        
        public List<User> Users { get; set; }
        
    }
    
}