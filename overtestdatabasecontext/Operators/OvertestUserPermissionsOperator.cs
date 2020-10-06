using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Identity;

namespace Sirkadirov.Overtest.Libraries.Shared.Database.Operators
{
    
    public class OvertestUserPermissionsOperator
    {

        private readonly OvertestDatabaseContext _databaseContext;
        
        public OvertestUserPermissionsOperator(OvertestDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }
        
        #region User's type sets
        
        public async Task<UserType> GetUserTypeByIdAsync(Guid userId) =>
            await _databaseContext.Users
                .Where(u => u.Id == userId)
                .Select(s => s.Type)
                .FirstAsync();

        public async Task<bool> GetUserHasSpecifiedTypeAsync(Guid userId, params UserType[] allowedUserTypes) =>
            allowedUserTypes.Contains(await GetUserTypeByIdAsync(userId));
        
        #endregion
        
        public async Task<bool> UserExistsAsync(Guid userId) => await _databaseContext.Users.Where(u => u.Id == userId).AnyAsync();
        
        #region Access rights
        
        public async Task<Guid?> GetUserCuratorIdAsync(Guid userId)
        {
            if (!await UserExistsAsync(userId))
                return null;

            try
            {
                var userGroupId = await _databaseContext.Users
                    .Where(u => u.Id == userId)
                    .Select(u => u.UserGroupId)
                    .FirstAsync();
                
                if (userGroupId == null)
                    return null;
                
                return await _databaseContext.UserGroups
                    .Where(g => g.Id == userGroupId)
                    .Select(s => s.GroupCuratorId)
                    .FirstAsync();
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> GetUserDataEditPermissionAsync(Guid editedUserId, Guid editorUserId, bool allowSameUser = true)
        {
            
            if (allowSameUser && editedUserId == editorUserId)
                return true;
            
            try
            {
                // Check specified users exist
                if (await _databaseContext.Users.Where(u => u.Id == editedUserId || u.Id == editorUserId).CountAsync() != 2)
                    return false;
                
                // If editor type is SuperUser, allow to edit without any questions
                if (await _databaseContext.Users.Where(u => u.Id == editorUserId && u.Type == UserType.SuperUser).AnyAsync())
                    return true;
                
                // Get edited user's curator
                var editedUserCuratorId = await _databaseContext.Users
                    .Where(u => u.Id == editedUserId)
                    .Include(u => u.UserGroup)
                    .Select(u => u.UserGroup.GroupCuratorId)
                    .FirstAsync();
                
                if (editedUserCuratorId == editorUserId)
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