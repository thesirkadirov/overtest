using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Sirkadirov.Overtest.Libraries.Shared.Database;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Identity;
using Sirkadirov.Overtest.Libraries.Shared.Methods;
using Sirkadirov.Overtest.WebApplication.Areas.Administration.Models.Social.UserGroupsAdministrationController;
using Sirkadirov.Overtest.WebApplication.Extensions.Filters;

namespace Sirkadirov.Overtest.WebApplication.Areas.Administration.Controllers
{
    
    [Area("Administration")]
    [Route("/Administration/Social/UserGroups")]
    [AllowedUserTypesFilter(UserType.Instructor, UserType.Administrator, UserType.SuperUser)]
    public class UserGroupsAdministrationController : Controller
    {
        
        private const string ViewsDirectoryPath = "~/Areas/Administration/Views/Social/UserGroupsAdministrationController/";
        
        private readonly OvertestDatabaseContext _databaseContext;
        private readonly UserManager<User> _userManager;
        private readonly IStringLocalizer<UserGroupsAdministrationController> _localizer;
        
        public UserGroupsAdministrationController(OvertestDatabaseContext databaseContext, UserManager<User> userManager, IStringLocalizer<UserGroupsAdministrationController> localizer)
        {
            _databaseContext = databaseContext;
            _userManager = userManager;
            _localizer = localizer;
        }
        
        [HttpGet, Route("Edit/{userGroupId:guid?}")]
        public async Task<IActionResult> Edit(Guid? userGroupId = null)
        {
            
            const string actionViewPath = ViewsDirectoryPath + nameof(Edit) + ".cshtml";
            
            var currentUserId = new Guid(_userManager.GetUserId(HttpContext.User));
            
            if (userGroupId == null)
                return View(actionViewPath);
            
            if (!await _databaseContext.UserGroups.AnyAsync(g => g.Id == userGroupId && g.GroupCuratorId == currentUserId))
                return Forbid();
            
            var model = await _databaseContext.UserGroups
                .AsNoTracking()
                .Where(g => g.Id == userGroupId)
                .Select(s => new UserGroup
                {
                    Id = s.Id,
                    DisplayName = s.DisplayName
                })
                .FirstAsync();
            
            return View(actionViewPath, model);
            
        }

        [HttpPost, ValidateAntiForgeryToken, Route("Edit/{userGroupId:guid?}")]
        [SuppressMessage("ReSharper", "RedundantAssignment")]
        public async Task<IActionResult> Edit(UserGroup model, Guid? userGroupId = null)
        {
            
            const string actionViewPath = ViewsDirectoryPath + nameof(Edit) + ".cshtml";
            
            if (!ModelState.IsValid)
                return View(actionViewPath, model);
            
            var currentUserId = new Guid(_userManager.GetUserId(HttpContext.User));
            
            if (model.Id != default)
            {
                
                if (!await _databaseContext.UserGroups.AnyAsync(g => g.Id == model.Id && g.GroupCuratorId == currentUserId))
                    return NotFound();

                var groupFromDatabase = await _databaseContext.UserGroups.FirstAsync(g => g.Id == model.Id);

                groupFromDatabase.DisplayName = model.DisplayName;

                _databaseContext.UserGroups.Update(groupFromDatabase);
                await _databaseContext.SaveChangesAsync();

                userGroupId = groupFromDatabase.Id;

            }
            else
            {

                var newGroup = new UserGroup
                {
                    DisplayName = model.DisplayName,
                    GroupCuratorId = currentUserId
                };

                await _databaseContext.UserGroups.AddAsync(newGroup);
                await _databaseContext.SaveChangesAsync();

                userGroupId = newGroup.Id;

            }
            
            return RedirectToAction("View", "UserGroups", new {area = "Social", userGroupId});
            
        }
        
        [HttpGet, Route("Remove/{userGroupId:guid}")]
        public async Task<IActionResult> Remove(Guid userGroupId)
        {
            
            const string actionViewPath = ViewsDirectoryPath + nameof(Remove) + ".cshtml";
            var currentUserId = new Guid(_userManager.GetUserId(HttpContext.User));

            if (!await _databaseContext.UserGroups.AnyAsync(g => g.Id == userGroupId && g.GroupCuratorId == currentUserId))
                return NotFound();

            return View(actionViewPath, new UserGroupRemovalModel
            {
                UserGroupId = userGroupId
            });

        }
        
        [HttpPost, ValidateAntiForgeryToken, Route("Remove/{userGroupId:guid}")]
        public async Task<IActionResult> Remove(Guid userGroupId, UserGroupRemovalModel model)
        {
            
            const string actionViewPath = ViewsDirectoryPath + nameof(Remove) + ".cshtml";
            var currentUserId = new Guid(_userManager.GetUserId(HttpContext.User));
            
            model.UserGroupId = userGroupId;
            
            if (!ModelState.IsValid)
                return View(actionViewPath, model);
            
            if (!await _databaseContext.UserGroups.AnyAsync(g => g.Id == userGroupId && g.GroupCuratorId == currentUserId))
                return NotFound();
            
            if (!await _userManager.CheckPasswordAsync(await _userManager.GetUserAsync(HttpContext.User), model.PasswordConfirmation))
            {
                ModelState.AddModelError(string.Empty, _localizer["Ви ввели неправильний пароль до свого облікового запису! Перевірте його, та спробуйте ще раз."]);
                return View(actionViewPath, model);
            }
            
            _databaseContext.UserGroups.Remove(new UserGroup { Id = userGroupId });
            await _databaseContext.SaveChangesAsync();

            return RedirectToAction("List", "UserGroups", new {Area = "Social"});

        }
        
        #region User groups API
        
        [HttpGet, Route("Api/" + nameof(RegenerateUserGroupJoinLink) + "/{userGroupId:guid}")]
        public async Task<IActionResult> RegenerateUserGroupJoinLink(Guid userGroupId)
        {
            if (!await VerifyUsersGroupCuratorAccess(new Guid(_userManager.GetUserId(HttpContext.User)), userGroupId))
                return Forbid();
            
            return Content(GenerateUserGroupJoinLink(await ResetAccessKeyInternalAsync(userGroupId)));
        }
        
        [HttpGet, Route("Api/" + nameof(GetUserGroupJoinLink) + "/{userGroupId:guid}")]
        public async Task<IActionResult> GetUserGroupJoinLink(Guid userGroupId)
        {
            if (!await VerifyUsersGroupCuratorAccess(new Guid(_userManager.GetUserId(HttpContext.User)), userGroupId))
                return Forbid();
            
            return Content(GenerateUserGroupJoinLink(await GetAccessKeyInternalAsync(userGroupId) ?? await ResetAccessKeyInternalAsync(userGroupId)));
        }
        
        [NonAction]
        private async Task<string> GetAccessKeyInternalAsync(Guid userGroupId) => await _databaseContext.UserGroups
            .Where(g => g.Id == userGroupId)
            .Select(s => s.AccessToken)
            .FirstAsync();

        [NonAction]
        private async Task<string> ResetAccessKeyInternalAsync(Guid userGroupId)
        {
            var userGroup = await _databaseContext.UserGroups.FirstAsync(g => g.Id == userGroupId);
            userGroup.AccessToken = GuidAccessTokenGenerator.Generate();
            
            _databaseContext.UserGroups.Update(userGroup);
            await _databaseContext.SaveChangesAsync();

            return userGroup.AccessToken;
        }
        
        [NonAction]
        private async Task<bool> VerifyUsersGroupCuratorAccess(Guid userId, Guid userGroupId) =>
            await _databaseContext.UserGroups.AnyAsync(g => g.Id == userGroupId && g.GroupCuratorId == userId);
        
        [NonAction]
        private string GenerateUserGroupJoinLink(string securityToken) =>
            Url.Action("JoinByInviteSecurityToken", "Auth", new { securityToken }, HttpContext.Request.Scheme);
        
        #endregion
        
    }
    
}