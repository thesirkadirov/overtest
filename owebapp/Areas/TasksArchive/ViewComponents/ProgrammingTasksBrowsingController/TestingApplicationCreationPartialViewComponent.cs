using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sirkadirov.Overtest.Libraries.Shared.Database;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Identity;
using Sirkadirov.Overtest.WebApplication.Areas.TasksArchive.ViewComponents.Models.ProgrammingTasksBrowsingController;

namespace Sirkadirov.Overtest.WebApplication.Areas.TasksArchive.ViewComponents.ProgrammingTasksBrowsingController
{
    public class TestingApplicationCreationPartialViewComponent : ViewComponent
    {
        private readonly OvertestDatabaseContext _databaseContext;
        private readonly UserManager<User> _userManager;

        public TestingApplicationCreationPartialViewComponent(OvertestDatabaseContext databaseContext, UserManager<User> userManager)
        {
            _databaseContext = databaseContext;
            _userManager = userManager;
        }
        
        public async Task<IViewComponentResult> InvokeAsync(Guid programmingTaskId)
        {
            const string viewComponentViewPath = "~/Areas/TasksArchive/ViewComponents/Views/ProgrammingTasksBrowsingController/TestingApplicationCreationPartial.cshtml";
            
            if (!HttpContext.User.Identity.IsAuthenticated)
                return null;

            if (!await _databaseContext.ProgrammingTasks.AnyAsync(t => t.Id == programmingTaskId))
                return null;
            
            // TODO: Check whether programming task is available during competition

            var currentUserId = new Guid(_userManager.GetUserId(HttpContext.User));
            var currentCompetitionId = (Guid?)null;

            var lastTestingApplicationDatabaseQuery = _databaseContext.TestingApplications
                .AsNoTracking()
                .Where(a => a.AuthorId == currentUserId)
                .Where(a => a.CompetitionId == currentCompetitionId)
                .Where(a => a.ProgrammingTaskId == programmingTaskId)
                .OrderByDescending(a => a.Created);

            if (!await lastTestingApplicationDatabaseQuery.AnyAsync())
            {
                return View(viewComponentViewPath, new TestingApplicationCreationPartialModel
                {
                    ProgrammingTaskId = programmingTaskId,
                    
                    TestingType = null,
                    ProgrammingLanguageId = null,
                    SourceCode = null
                });
            }
            
            return View(
                viewComponentViewPath,
                await lastTestingApplicationDatabaseQuery
                    .Select(s => new TestingApplicationCreationPartialModel
                    {
                        ProgrammingTaskId = programmingTaskId,
                        TestingType = s.TestingType,
                        
                        SourceCode = Encoding.UTF8.GetString(s.SourceCode.SourceCode),
                        ProgrammingLanguageId = s.SourceCode.ProgrammingLanguageId
                    })
                    .FirstAsync()
            );
        }
    }
}