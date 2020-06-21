using System;
using System.Collections.Generic;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Identity;

namespace Sirkadirov.Overtest.WebApplication.Areas.Social.Models.UserGroupsController
{
    
    public class UserGroupsListModel
    {
        public Guid CuratorId { get; set; }
        public string CuratorFullName { get; set; }
        public List<UserGroup> UserGroupsList { get; set; }
    }
    
}