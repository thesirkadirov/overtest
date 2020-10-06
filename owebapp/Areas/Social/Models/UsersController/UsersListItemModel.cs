using System;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Identity;

namespace Sirkadirov.Overtest.WebApplication.Areas.Social.Models.UsersController
{
    
    public class UsersListItemModel
    {
        
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string InstitutionName { get; set; }
        public UserType Type { get; set; }
        public Guid? UserGroupId { get; set; }
        
        public int Rating { get; set; }
        
    }
    
}