using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Identity
{
    
    public class User : IdentityUser<Guid>
    {
        
        public string FullName { get; set; }
        
        public string InstitutionName { get; set; }
        
        public UserType Type { get; set; }
        
        public bool IsBanned { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Registered { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime LastSeen { get; set; }
        
        /*
         * Relationships
         */
        
        public User Curator { get; set; }
        public Guid? CuratorId { get; set; }
        
        public UserGroup UserGroup { get; set; }
        public Guid? UserGroupId { get; set; }
        
        public List<UserGroup> UserGroups { get; set; }
        
        /*
         * Methods
         */
        
        public static bool IsSuperUser(User user)
        {
            return user.Type == UserType.SuperUser;
        }
        
        public static bool IsApprovedUser(User user)
        {
            return user.IsSuperUser() || (
                       !user.IsBanned
                       && user.CuratorId != null
                       && user.UserGroupId != null
                   );
        }

        public static bool IsAdministrator(User user)
        {
            return user.Type == UserType.Administrator || user.IsSuperUser();
        }

        public static bool IsCurator(User user)
        {
            return user.Type == UserType.Instructor || user.IsAdministrator();
        }
        
        public bool IsSuperUser() => IsSuperUser(this);
        public bool IsApprovedUser() => IsApprovedUser(this);
        public bool IsAdministrator() => IsAdministrator(this);
        public bool IsCurator() => IsCurator(this);
        
        public enum UserType
        {
        
            Anonymous,
            Student,
            Instructor,
            Administrator,
            SuperUser
        
        }
        
    }
    
}