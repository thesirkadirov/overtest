using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Identity
{
    public class User : IdentityUser<Guid>
    {
        [MinLength(3), MaxLength(255)]
        public string FullName { get; set; }
        
        [MinLength(0), MaxLength(255)]
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
        Student = 1,
        Curator = 50,
        SuperUser = 100
    }
}