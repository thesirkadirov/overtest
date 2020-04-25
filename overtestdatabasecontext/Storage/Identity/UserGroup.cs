using System;
using System.Collections.Generic;

namespace Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Identity
{
    
    public class UserGroup
    {
        
        public Guid Id { get; set; }
        
        public string DisplayName { get; set; }

        public string AccessToken { get; set; }
        
        public User Curator { get; set; }
        public Guid CuratorId { get; set; }
        
        public List<User> Users { get; set; }
        
    }
    
}