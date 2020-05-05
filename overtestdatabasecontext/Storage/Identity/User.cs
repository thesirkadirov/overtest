using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Sirkadirov.Overtest.Libraries.Shared.Database.Operators;

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
        
        public UserGroup UserGroup { get; set; }
        public Guid? UserGroupId { get; set; }

        public UserPhoto UserPhoto { get; set; }
        public Guid? UserPhotoId { get; set; }
        
        /*
         * Methods
         */
        
        // Get permissions operator
        public OvertestUserPermissionsOperator GetPermissionsOperator()
            => new OvertestUserPermissionsOperator(this);
        
    }
    
    public enum UserType
    {
        
        Anonymous,
        Student,
        Instructor,
        Administrator,
        SuperUser
        
    }
    
}