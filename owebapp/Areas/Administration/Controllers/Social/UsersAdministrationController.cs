using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Sirkadirov.Overtest.Libraries.Shared.Database;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Identity;
using Sirkadirov.Overtest.WebApplication.Areas.Administration.Models.Social.UsersAdministrationController;
using Sirkadirov.Overtest.WebApplication.Extensions.Filters;

namespace Sirkadirov.Overtest.WebApplication.Areas.Administration.Controllers
{
    
    [Area("Administration")]
    [Route("/Administration/Social/Users")]
    public class UsersAdministrationController : Controller
    {
        
        private const string ViewsDirectoryPath = "~/Areas/Administration/Views/Social/UsersAdministrationController/";
        
        private readonly OvertestDatabaseContext _databaseContext;
        private readonly UserManager<User> _userManager;
        private readonly IStringLocalizer<UsersAdministrationController> _localizer;

        public UsersAdministrationController(OvertestDatabaseContext databaseContext, UserManager<User> userManager, IStringLocalizer<UsersAdministrationController> localizer)
        {
            _databaseContext = databaseContext;
            _userManager = userManager;
            _localizer = localizer;
        }
        
        [HttpGet, Route("Edit/Profile/{userId:guid}")]
        public async Task<IActionResult> EditUserProfile(Guid userId)
        {
            if (!await UserExists(userId))
                return NotFound();
            
            const string actionViewPath = ViewsDirectoryPath + nameof(EditUserProfile) + ".cshtml";
            
            var currentUserId = new Guid(_userManager.GetUserId(HttpContext.User));

            if (!await _databaseContext.UserPermissionsOperator.GetUserDataEditPermissionAsync(userId, currentUserId))
                return Forbid();

            var userInfo = await _databaseContext.Users
                .Where(u => u.Id == userId)
                .Select(s => new EditUserProfileModel
                {
                    UserId = s.Id,
                    FullName = s.FullName,
                    InstitutionName = s.InstitutionName
                })
                .FirstAsync();

            return View(actionViewPath, userInfo);
        }
        
        [HttpPost, ValidateAntiForgeryToken, Route("Edit/Profile/{userId:guid}")]
        public async Task<IActionResult> EditUserProfile(Guid userId, EditUserProfileModel model)
        {
            if (!await UserExists(userId))
                return NotFound();
            
            const string actionViewPath = ViewsDirectoryPath + nameof(EditUserProfile) + ".cshtml";
            
            var currentUserId = new Guid(_userManager.GetUserId(HttpContext.User));
            model.UserId = userId;

            if (!await _databaseContext.UserPermissionsOperator.GetUserDataEditPermissionAsync(userId, currentUserId))
                return Forbid();

            if (!ModelState.IsValid)
                return View(actionViewPath, model);

            var userObject = await _databaseContext.Users.FirstAsync(u => u.Id == userId);
            userObject.FullName = model.FullName;
            userObject.InstitutionName = model.InstitutionName;

            _databaseContext.Users.Update(userObject);
            await _databaseContext.SaveChangesAsync();

            return RedirectToAction("UserProfile", "Users", new { area = "Social", userId });
        }
        
        [HttpGet, Route("Edit/ChangePassword/{userId:guid}")]
        public async Task<IActionResult> ChangeUserPassword(Guid userId)
        {
            if (!await UserExists(userId))
                return NotFound();
            
            const string actionViewPath = ViewsDirectoryPath + nameof(ChangeUserPassword) + ".cshtml";
            
            var currentUserId = new Guid(_userManager.GetUserId(HttpContext.User));

            if (!await _databaseContext.UserPermissionsOperator.GetUserDataEditPermissionAsync(userId, currentUserId))
                return Forbid();

            return View(actionViewPath, new ChangeUserPasswordModel { UserId = userId });
        }
        
        [HttpPost, ValidateAntiForgeryToken, Route("Edit/ChangePassword/{userId:guid}")]
        public async Task<IActionResult> ChangeUserPassword(Guid userId, ChangeUserPasswordModel model)
        {
            if (!await UserExists(userId))
                return NotFound();
            
            const string actionViewPath = ViewsDirectoryPath + nameof(ChangeUserPassword) + ".cshtml";
            
            var currentUserId = new Guid(_userManager.GetUserId(HttpContext.User));

            if (!await _databaseContext.UserPermissionsOperator.GetUserDataEditPermissionAsync(userId, currentUserId))
                return Forbid();

            model.UserId = userId;
            if (!ModelState.IsValid)
                return View(actionViewPath);
            
            var userObject = await _userManager.FindByIdAsync(userId.ToString());
            
            if (userId == currentUserId)
            {
                if (string.IsNullOrWhiteSpace(model.OldPassword))
                {
                    ModelState.AddModelError(
                        nameof(ChangeUserPasswordModel.OldPassword),
                        _localizer["Ви маєте вказати свій попередній пароль!"]
                    );
                    return View(actionViewPath);
                }

                if (!await _userManager.CheckPasswordAsync(userObject, model.OldPassword))
                {
                    ModelState.AddModelError(
                        nameof(ChangeUserPasswordModel.OldPassword),
                        _localizer["Ви ввели неправильний пароль від свого облікового запису!"]
                    );
                    return View(actionViewPath);
                }
            }
            
            var identityResult = await _userManager.ResetPasswordAsync(
                userObject,
                await _userManager.GeneratePasswordResetTokenAsync(userObject),
                model.NewPassword
            );

            if (identityResult.Succeeded)
                return RedirectToAction("UserProfile", "Users", new { area = "Social", userId });

            foreach (var identityError in identityResult.Errors)
            {
                ModelState.AddModelError(string.Empty, $"Error #{identityError.Code}: {identityError.Description}");
            }
            
            return View(actionViewPath, model);
        }

        [HttpPost, ValidateAntiForgeryToken, Route("Edit/UserType/{userId:guid}")]
        [AllowedUserTypesFilter(UserType.Administrator, UserType.SuperUser)]
        public async Task<IActionResult> ChangeUserType(Guid userId, UserType newUserType)
        {
            if (!await UserExists(userId))
                return NotFound();

            if (newUserType == UserType.Anonymous)
                return Forbid();
            
            var curatorUserId = new Guid(_userManager.GetUserId(HttpContext.User));
            
            if (!await _databaseContext.UserPermissionsOperator.GetUserDataEditPermissionAsync(userId, curatorUserId, false))
                return Forbid();
            
            var curatorUserType = await _databaseContext.Users
                .Where(u => u.Id == curatorUserId)
                .Select(s => s.Type)
                .FirstAsync();

            var currentUserType = await _databaseContext.Users
                .Where(u => u.Id == userId)
                .Select(s => s.Type)
                .FirstAsync();

            if (newUserType == curatorUserType)
                return Forbid();
            
            if (currentUserType == newUserType)
                return Forbid();
            
            if (curatorUserType == UserType.Administrator)
            {
                if ((currentUserType == UserType.Student && newUserType == UserType.Instructor) ||
                    (currentUserType == UserType.Anonymous && newUserType == UserType.Instructor))
                {
                    await UpdateUserType();
                }
            }
            else if (curatorUserType == UserType.SuperUser)
            {
                await UpdateUserType();
            }

            return RedirectToAction("UserProfile", "Users", new { area = "Social", userId });
            
            async Task UpdateUserType()
            {
                var userObject = await _databaseContext.Users.FirstAsync(u => u.Id == userId);
                userObject.Type = newUserType;
                _databaseContext.Users.Update(userObject);
                await _databaseContext.SaveChangesAsync();
            }
        }
        
        [HttpPost, ValidateAntiForgeryToken, Route("Edit/Remove/{userId:guid}")]
        [AllowedUserTypesFilter(UserType.Instructor, UserType.Administrator, UserType.SuperUser)]
        public async Task<IActionResult> RemoveUser(Guid userId)
        {
            if (!await UserExists(userId))
                return NotFound();
            
            var currentUserId = new Guid(_userManager.GetUserId(HttpContext.User));
            
            if (!await _databaseContext.UserPermissionsOperator.GetUserDataEditPermissionAsync(userId, currentUserId, false))
                return Forbid();
            
            var deletedUserGroupId = await _databaseContext.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(s => s.UserGroupId)
                .FirstAsync();
            
            _databaseContext.Users.Remove(await _userManager.FindByIdAsync(userId.ToString()));
            await _databaseContext.SaveChangesAsync();
            
            return deletedUserGroupId != null
                ? RedirectToAction("View", "UserGroups", new { area = "Social", userGroupId = deletedUserGroupId })
                : RedirectToAction("List", "UserGroups", new { area = "Social" });
        }

        [NonAction]
        private async Task<bool> UserExists(Guid userId) =>
            await _databaseContext.Users.AnyAsync(u => u.Id == userId);
        
    }
    
}