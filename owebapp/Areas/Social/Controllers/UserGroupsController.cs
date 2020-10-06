using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sirkadirov.Overtest.Libraries.Shared.Database;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Identity;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.TestingApplications;
using Sirkadirov.Overtest.WebApplication.Areas.Social.Models.UserGroupsController;

namespace Sirkadirov.Overtest.WebApplication.Areas.Social.Controllers
{
    
    [Area("Social"), Route("/Social/UserGroups/")]
    public class UserGroupsController : Controller
    {
        
        private const string ViewsDirectoryPath = "~/Areas/Social/Views/UserGroupsController/";
        
        private readonly OvertestDatabaseContext _databaseContext;
        private readonly UserManager<User> _userManager;
        
        public UserGroupsController(OvertestDatabaseContext databaseContext, UserManager<User> userManager)
        {
            _databaseContext = databaseContext;
            _userManager = userManager;
        }

        [HttpGet, Route(nameof(List) + "/{userId:guid?}")]
        public async Task<IActionResult> List(Guid? userId = null)
        {
            
            userId ??= new Guid(_userManager.GetUserId(HttpContext.User));
            
            if (!await _databaseContext.Users.AnyAsync(u => u.Id == userId))
                return NotFound();
            
            if (!await _databaseContext.UserPermissionsOperator.GetUserHasSpecifiedTypeAsync(userId.Value, UserType.Curator, UserType.SuperUser))
            {
                var previousUserId = (Guid)userId;
                userId = await _databaseContext.Users
                    .AsNoTracking()
                    .Where(u => u.Id == previousUserId)
                    .Include(i => i.UserGroup)
                    .Select(s => s.UserGroup.GroupCuratorId)
                    .FirstAsync();
            }

            var model = new UserGroupsListModel
            {
                CuratorId = (Guid) userId,
                CuratorFullName = await _databaseContext.Users
                    .Where(u => u.Id == userId)
                    .Select(s => s.FullName)
                    .FirstAsync(),
                UserGroupsList = await _databaseContext.UserGroups
                    .AsNoTracking()
                    .Where(g => g.GroupCuratorId == (Guid)userId)
                    .Select(s => new UserGroup
                    {
                        Id = s.Id,
                        DisplayName = s.DisplayName
                    })
                    .ToListAsync()
            };

            return View(ViewsDirectoryPath + "List.cshtml", model);

        }
        
        [HttpGet, Route(nameof(View) + "/{userGroupId:guid}")]
        public async Task<IActionResult> View(Guid userGroupId)
        {
            
            if (!await _databaseContext.UserGroups.AnyAsync(g => g.Id == userGroupId))
                return NotFound();

            ViewBag.UserGroupId = userGroupId;
            
            var model = await
                (
                    from user in _databaseContext.Users
                    where (user.UserGroupId == userGroupId)
                    join application
                        in _databaseContext.TestingApplications.Include(i => i.TestingResult)
                        on user.Id equals application.AuthorId
                        into applications
                    from application in applications
                        .Where(a => a.CompetitionId == null)
                        .Where(a => a.Status == TestingApplication.ApplicationStatus.Verified)
                        .Where(a => a.TestingResult.SolutionAdjudgement != TestingApplicationResult.SolutionAdjudgementType.ZeroSolution)
                        .Where(a => a.TestingType == TestingApplication.ApplicationTestingType.ReleaseMode)
                        .DefaultIfEmpty()
                    select new 
                    {
                        user.Id, user.Type, user.FullName, user.InstitutionName, 
                        application.TestingResult.GivenDifficulty
                    }
                )
                .GroupBy(g => new
                {
                    g.Id, g.Type, g.FullName, g.InstitutionName
                })
                .Select(s => new UserGroupUsersListItemModel
                {
                    Id = s.Key.Id, Type = s.Key.Type,
                    FullName = s.Key.FullName, InstitutionName = s.Key.InstitutionName, 
                    Rating = s.Sum(sum => sum.GivenDifficulty)
                })
                .OrderByDescending(order => order.Rating)
                .ThenBy(order => order.FullName)
                .AsNoTracking()
                .ToListAsync();

            return View(ViewsDirectoryPath + "View.cshtml", model);

        }
        
    }
    
}