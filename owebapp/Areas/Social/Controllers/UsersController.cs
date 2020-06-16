using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sirkadirov.Overtest.Libraries.Shared.Database;
using Sirkadirov.Overtest.WebApplication.Areas.Social.Models.UsersController;
using Sirkadirov.Overtest.WebApplication.Models.Shared.Pagination;

namespace Sirkadirov.Overtest.WebApplication.Areas.Social.Controllers
{
    
    [Area("Social"), Route("/Social/Users")]
    public class UsersController : Controller
    {
        
        private const string ViewsDirectoryPath = "~/Areas/Social/Views/UsersController/";
        private const int ItemsPerPage = 50;
        
        private readonly OvertestDatabaseContext _databaseContext;
        
        public UsersController(OvertestDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }
        
        #region User profile
        
        [HttpGet, Route("Profile")]
        public IActionResult Profile()
        {
            return RedirectToAction(nameof(UserProfile), new
            {
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            });
        }
        
        [HttpGet, Route("{userId:Guid}"), Route("Profile/{userId:Guid}")]
        public async Task<IActionResult> UserProfile(Guid userId)
        {
            
            var userInfo = await _databaseContext.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Include(u => u.UserGroup)
                .FirstOrDefaultAsync();
            
            if (userInfo == null)
                return NotFound();
            
            return View(ViewsDirectoryPath + "UserProfile.cshtml", userInfo);
            
        }
        
        #endregion

        [HttpGet, Route(nameof(List) + "/{page:int?}")]
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public async Task<IActionResult> List(int page = 1, Guid? groupId = null, string institutionName = "", string searchQuery = "")
        {
            
            if (page < 1)
                return NotFound();
            
            ViewData["SpecifiedUsersGroupId"] = groupId;
            ViewData["SpecifiedInstitutionName"] = institutionName;
            ViewData["SpecifiedSearchQuery"] = searchQuery;
            
            var model = new PaginatedListModel<UsersListItemModel>
            {
                Pagination = new PaginationInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = ItemsPerPage
                }
            };
            
            model.ItemsList = await
                (
                    from user in _databaseContext.Users
                    
                    where (groupId == null || user.UserGroupId == groupId) 
                    where (searchQuery == null || searchQuery.Length == 0 || EF.Functions.Like(user.FullName, $"%{searchQuery}%")) 
                    where (institutionName == null || institutionName.Length == 0 || EF.Functions.Like(user.InstitutionName, $"%{institutionName}%"))
                    
                    join application in _databaseContext.TestingApplications on user.Id equals application.AuthorId into applications 
                    from application in applications.DefaultIfEmpty() 
                    select new 
                    {
                        user.Id, user.Type, user.UserGroupId, 
                        user.FullName, user.InstitutionName, 
                        application.CompetitionId, application.TestingType, 
                        application.TestingResults.SolutionAdjudgement, application.TestingResults.GivenDifficulty
                    }
                ).GroupBy(g => new
                {
                    g.Id, g.Type, g.UserGroupId, g.FullName, g.InstitutionName
                })
                .Select(s => new UsersListItemModel 
                {
                    Id = s.Key.Id, Type = s.Key.Type, UserGroupId = s.Key.UserGroupId, 
                    FullName = s.Key.FullName, InstitutionName = s.Key.InstitutionName, 
                    Rating = s.Sum(sum => sum.GivenDifficulty)
                })
                .Skip(model.Pagination.PreviousItemsCount).Take(ItemsPerPage)
                .OrderByDescending(order => order.Rating)
                .AsNoTracking()
                .ToListAsync();
            
            model.Pagination.TotalItems = model.ItemsList.Count;
            
            if (model.Pagination.TotalItems <= 0 && page > 1)
                return NotFound();
            
            if (model.Pagination.TotalItems > 0 && model.Pagination.TotalPages < page)
                return NotFound();
            
            return View(ViewsDirectoryPath + "List.cshtml", model);
            
        }
        
    }
    
}