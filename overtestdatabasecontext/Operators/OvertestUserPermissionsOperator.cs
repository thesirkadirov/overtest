using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Identity;

namespace Sirkadirov.Overtest.Libraries.Shared.Database.Operators
{
    
    public class OvertestUserPermissionsOperator
    {

        private readonly User _user;
        
        public OvertestUserPermissionsOperator(User user)
        {
            _user = user;
        }
        
        #region USER_TYPESETS
        
        public bool IsUserTypeSetSuperUser() => IsUserTypeSetSuperUser(_user);
        public bool IsUserTypeSetAdministrator() => IsUserTypeSetAdministrator(_user);
        public bool IsUserTypeSetCurator() => IsUserTypeSetCurator(_user);
        public bool IsUserTypeSetApprovedUser() => IsUserTypeSetApprovedUser(_user);
        
        public static bool IsUserTypeSetSuperUser(User user)
        {
            return user.Type == UserType.SuperUser;
        }

        public static bool IsUserTypeSetAdministrator(User user)
        {
            return user.Type == UserType.Administrator || IsUserTypeSetSuperUser(user);
        }

        public static bool IsUserTypeSetCurator(User user)
        {
            return user.Type == UserType.Instructor || IsUserTypeSetAdministrator(user);
        }
        
        public static bool IsUserTypeSetApprovedUser(User user)
        {
            return IsUserTypeSetSuperUser(user) || (!user.IsBanned && user.UserGroupId != null);
        }
        
        #endregion
        
        #region EDIT_ACCESS_RIGHTS
        
        public Guid? GetUserCuratorId() => GetUserCuratorId(_user);
        
        public static Guid? GetUserCuratorId(User user)
        {

            if (user.UserGroupId == null)
                return null;
            
            if (user.UserGroup == null)
                throw new NullReferenceException(nameof(UserGroup));
            
            return user.UserGroup.GroupCuratorId;
        }
        
        public static async Task<bool> GetUserDataEditPermission(OvertestDatabaseContext databaseContext, Guid editedUserId, Guid editorUserId)
        {
            
            if (editedUserId == editorUserId)
                return true;
            
            try
            {
                
                if (await databaseContext.Users.Where(u => u.Id == editedUserId || u.Id == editorUserId).CountAsync() != 2)
                    return false;

                if (await databaseContext.Users.Where(u => u.Id == editorUserId && u.Type == UserType.SuperUser).AnyAsync())
                    return true;

                var editedUserCuratorId = await databaseContext.Users
                    .Where(u => u.Id == editedUserId)
                    .Include(u => u.UserGroup)
                    .Select(u => u.UserGroup.GroupCuratorId)
                    .FirstAsync();

                if (editedUserCuratorId == editorUserId)
                    return true;

                var editedUserHeadCuratorId = await databaseContext.Users
                    .Where(u => u.Id == editedUserId)
                    .Include(u => u.UserGroup)
                    .ThenInclude(g => g.GroupCurator)
                    .ThenInclude(u => u.UserGroup)
                    .Select(u => u.UserGroup.GroupCurator.UserGroup.GroupCuratorId)
                    .FirstAsync();

                if (editedUserHeadCuratorId == editorUserId)
                    return true;
                
            }
            catch
            {
                return false;
            }
            
            return false;

        }
        
        #endregion
        
    }
    
}