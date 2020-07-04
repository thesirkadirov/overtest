using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Identity
{
    
    public class User : IdentityUser<Guid>
    {
        
        [Required, MinLength(3), MaxLength(255)]
        public string FullName { get; set; }
        
        [MaxLength(255)]
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
        
        public List<UserGroup> CuratedUserGroups { get; set; }
        
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