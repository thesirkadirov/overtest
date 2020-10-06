using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Sirkadirov.Overtest.Libraries.Shared.Database;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Identity;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.TasksArchive;
using Sirkadirov.Overtest.WebApplication.Areas.Administration.Controllers;

namespace Sirkadirov.Overtest.WebApplication.Areas.TasksArchive.Controllers
{
    [Area("TasksArchive")]
    [Route("/TasksArchive/ProgrammingTasks")]
    public class ProgrammingTasksBrowsingController : Controller
    {
        private const string ViewsDirectoryPath = "~/Areas/TasksArchive/Views/ProgrammingTasksBrowsingController/";
        
        private readonly OvertestDatabaseContext _databaseContext;
        private readonly UserManager<User> _userManager;
        private readonly IStringLocalizer<ProgrammingTasksAdministrationController> _localizer;
        
        public ProgrammingTasksBrowsingController(OvertestDatabaseContext databaseContext, UserManager<User> userManager, IStringLocalizer<ProgrammingTasksAdministrationController> localizer)
        {
            _databaseContext = databaseContext;
            _userManager = userManager;
            _localizer = localizer;
        }
        
        [HttpGet, Route(nameof(View) + "/{programmingTaskId:guid}")]
        public async Task<IActionResult> View(Guid programmingTaskId)
        {
            const string actionViewPath = ViewsDirectoryPath + nameof(View) + ".cshtml";

            if (!await _databaseContext.ProgrammingTasks.AnyAsync(t => t.Id == programmingTaskId))
                return NotFound();
            
            var currentUserId = new Guid(_userManager.GetUserId(HttpContext.User));

            if (!await _databaseContext.UserPermissionsOperator.GetUserHasSpecifiedTypeAsync(currentUserId, UserType.SuperUser))
            {
                if (!await _databaseContext.ProgrammingTasks.AnyAsync(t => t.Id == programmingTaskId && t.VisibleInFreeMode))
                    return Forbid();
                
                // TODO: Competition check
            }
            
            var programmingTaskObject = await _databaseContext.ProgrammingTasks
                .Where(t => t.Id == programmingTaskId)
                .Include(i => i.Category)
                .Select(s => new ProgrammingTask
                {
                    Id = s.Id,
                    Title = s.Title,
                    Description = s.Description,
                    
                    Difficulty = s.Difficulty,
                    
                    CategoryId = s.CategoryId
                })
                .FirstAsync();
            
            return View(actionViewPath, programmingTaskObject);
        }
    }
}