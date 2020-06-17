using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        
        private async Task<UserType> GetUserTypeByIdAsync(Guid userId) =>
            await _databaseContext.Users
                .Where(u => u.Id == userId)
                .Select(s => s.Type)
                .FirstAsync();

        [SuppressMessage("ReSharper", "VariableHidesOuterVariable")]
        public async Task<bool> VerifyUserTypeSetAsync(Guid userId, UserTypeSet awaitedTypeSet)
        {
            
            if (!await UserExistsAsync(userId))
                throw new KeyNotFoundException();
            
            var userType = await GetUserTypeByIdAsync(userId);

            return awaitedTypeSet switch
            {
                UserTypeSet.ApprovedUser => IsApprovedUser(userId, userType),
                UserTypeSet.Curator => IsCurator(userType),
                UserTypeSet.Administrator => IsAdministrator(userType),
                UserTypeSet.SuperUser => IsSuperUser(userType),
                _ => throw new ArgumentException(null, nameof(awaitedTypeSet))
            };
            
            static bool IsSuperUser(UserType userType) => userType == UserType.SuperUser;
            static bool IsAdministrator(UserType userType) => userType == UserType.Administrator || IsSuperUser(userType);
            static bool IsCurator(UserType userType) => userType == UserType.Instructor || IsAdministrator(userType);
            bool IsApprovedUser(Guid userId, UserType userType) => IsSuperUser(userType) || _databaseContext.Users
                .Any(u => u.Id == userId && u.IsBanned == false && u.UserGroupId != null);
            
        }
        
        public enum UserTypeSet
        {
            ApprovedUser,
            Curator,
            Administrator,
            SuperUser
        }
        
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

        public async Task<bool> GetUserDataEditPermissionAsync(Guid editedUserId, Guid editorUserId)
        {
            
            if (editedUserId == editorUserId)
                return true;
            
            try
            {
                
                if (await _databaseContext.Users.Where(u => u.Id == editedUserId || u.Id == editorUserId).CountAsync() != 2)
                    return false;
                
                if (await _databaseContext.Users.Where(u => u.Id == editorUserId && u.Type == UserType.SuperUser).AnyAsync())
                    return true;
                
                var editedUserCuratorId = await _databaseContext.Users
                    .Where(u => u.Id == editedUserId)
                    .Include(u => u.UserGroup)
                    .Select(u => u.UserGroup.GroupCuratorId)
                    .FirstAsync();
                
                if (editedUserCuratorId == editorUserId)
                    return true;
                
                var editedUserHeadCuratorId = await _databaseContext.Users
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