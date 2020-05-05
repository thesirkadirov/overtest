using System;
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
        
        public Guid GetUserCuratorId() => GetUserCuratorId(_user);
        public bool CanBeEditedBy(User editor) => CanBeEditedBy(_user, editor);
        
        public static Guid GetUserCuratorId(User user)
        {
            return user.UserGroup.GroupCuratorId;
        }

        public static bool CanBeEditedBy(User edited, User editor)
        {

            if (edited.Id == editor.Id)
                return true;

            if (IsUserTypeSetSuperUser(editor) && !IsUserTypeSetSuperUser(edited))
                return true;
            
            if (GetUserCuratorId(edited) == editor.Id)
                return true;
            
            return false;

        }
        
        #endregion
        
    }
    
}