using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Sirkadirov.Overtest.Libraries.Shared.Database;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Identity;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.TestingApplications;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.TestingApplications.Extras;
using Sirkadirov.Overtest.WebApplication.Areas.TasksArchive.ViewComponents.Models.ProgrammingTasksBrowsingController;

namespace Sirkadirov.Overtest.WebApplication.Areas.TasksArchive.Controllers
{
    [Area("TasksArchive")]
    [Route("/TasksArchive/TestingApplications")]
    public class ProgrammingTasksTestingApplicationsController : Controller
    {
        private const string ViewsDirectoryPath = "~/Areas/TasksArchive/Views/ProgrammingTasksTestingApplicationsController/";
        
        private readonly OvertestDatabaseContext _databaseContext;
        private readonly UserManager<User> _userManager;
        private readonly IStringLocalizer<ProgrammingTasksTestingApplicationsController> _localizer;

        public ProgrammingTasksTestingApplicationsController(OvertestDatabaseContext databaseContext,
            UserManager<User> userManager, IStringLocalizer<ProgrammingTasksTestingApplicationsController> localizer)
        {
            _databaseContext = databaseContext;
            _userManager = userManager;
            _localizer = localizer;
        }

        [HttpGet, Route(nameof(List) + "/{userId:guid?}")]
        public async Task<IActionResult> List(Guid? userId)
        {
            throw new NotImplementedException();
        }

        [HttpGet, Route(nameof(View) + "/{testingApplicationId:guid}")]
        public async Task<IActionResult> View(Guid testingApplicationId)
        {
            const string actionViewPath = ViewsDirectoryPath + nameof(View) + ".cshtml";
            
            var currentUserId = new Guid(_userManager.GetUserId(HttpContext.User));
            var testingApplicationQuery = _databaseContext.TestingApplications
                .Where(a => a.Id == testingApplicationId).AsNoTracking();

            if (!await testingApplicationQuery.AnyAsync())
                return NotFound();

            var hasAccessRights = await _databaseContext.UserPermissionsOperator.GetUserDataEditPermissionAsync(
                await testingApplicationQuery.Select(s => s.AuthorId).FirstAsync(),
                currentUserId
            );

            if (!hasAccessRights)
                return Forbid();

            var testingApplication = await testingApplicationQuery.FirstAsync();

            return View(actionViewPath, testingApplication);
        }
        
        [HttpPost, ValidateAntiForgeryToken, Route(nameof(SubmitTestingApplication))]
        public async Task<IActionResult> SubmitTestingApplication(TestingApplicationCreationPartialModel model)
        {
            if (!ModelState.IsValid || !model.ProgrammingLanguageId.HasValue || !model.TestingType.HasValue)
                return BadRequest(ModelState.ToList());
            
            var currentUserId = new Guid(_userManager.GetUserId(HttpContext.User));
            var isSuperUser = await _databaseContext.UserPermissionsOperator.GetUserHasSpecifiedTypeAsync(currentUserId, UserType.SuperUser);
            
            if (!await _databaseContext.ProgrammingTasks.AnyAsync(t => t.Id == model.ProgrammingTaskId))
                return NotFound();
            
            // TODO: Disabled for development and testing stages only
            //if (!await _databaseContext.ProgrammingTasks.AnyAsync(t => t.Id == model.ProgrammingTaskId && t.TestingData.DataPackageFile != null))
            //    return BadRequest(_localizer["Обране вами завдання не містить даних, необхідних для його тестування! Зв'яжіться з автором завдання чи адміністратором системи."].Value);
            
            var programmingTaskInfo = await _databaseContext.ProgrammingTasks
                .Where(t => t.Id == model.ProgrammingTaskId)
                .Select(s => new
                {
                    s.VisibleInFreeMode,
                    s.VisibleInCompetitionMode
                })
                .FirstAsync();
            
            var isCompetitionModeApplication = false;
            
            if (!isSuperUser)
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (!isCompetitionModeApplication && !programmingTaskInfo.VisibleInFreeMode)
                    return Forbid();

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (isCompetitionModeApplication && !programmingTaskInfo.VisibleInCompetitionMode)
                    return Forbid();
            }
            
            // Create testing application prototype
            var testingApplication = new TestingApplication
            {
                ProgrammingTaskId = model.ProgrammingTaskId,
                AuthorId = currentUserId,
                CompetitionId = null,
                
                Created = DateTime.UtcNow,
                
                SourceCode = new TestingApplicationSourceCode
                {
                    ProgrammingLanguageId = model.ProgrammingLanguageId.Value,
                    SourceCode = Encoding.UTF8.GetBytes(model.SourceCode.Trim().Replace("\r", string.Empty).Replace("\n", "\r\n"))
                },
                TestingType = model.TestingType.Value
            };
            
            // TODO: Implement FLOOD and DDOS security

            try
            {
                await using var transaction = await _databaseContext.Database.BeginTransactionAsync();
                
                var previousWaitingSubmissionsQuery = _databaseContext.TestingApplications
                    .Where(a => a.Status == TestingApplication.ApplicationStatus.Waiting)
                    .Where(a => a.ProgrammingTaskId == model.ProgrammingTaskId)
                    .Where(a => a.AuthorId == currentUserId);
                
                if (await previousWaitingSubmissionsQuery.AnyAsync())
                {
                    var previousWaitingSubmissions = await previousWaitingSubmissionsQuery
                        .Select(s => new TestingApplication { Id = s.Id, Status = s.Status})
                        .AsNoTracking()
                        .ToListAsync();
                    
                    _databaseContext.TestingApplications.AttachRange(previousWaitingSubmissions);
                    _databaseContext.TestingApplications.RemoveRange(previousWaitingSubmissions);
                }

                await _databaseContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex) { return BadRequest(ex.GetType().FullName); /* There's nothing to do here... */ }

            // TODO: Check if there are any submissions for this problem in "testing" stage, created by this user
            
            if (testingApplication.TestingType == TestingApplication.ApplicationTestingType.ReleaseMode)
            {
                var previousSubmissionsList = await _databaseContext.TestingApplications
                    .Where(a => a.AuthorId == testingApplication.AuthorId)
                    .Where(a => a.ProgrammingTaskId == testingApplication.ProgrammingTaskId)
                    .Where(a => a.CompetitionId == testingApplication.CompetitionId)
                    .Select(s => new TestingApplication { Id = s.Id, Status = s.Status })
                    .ToListAsync();
            
                foreach (var item in previousSubmissionsList)
                    _databaseContext.TestingApplications.Remove(item);
            }
            
            // Add a newly-created testing application to the database
            await _databaseContext.TestingApplications.AddAsync(testingApplication);
            
            // Save changes made to the database
            await _databaseContext.SaveChangesAsync();
            
            return RedirectToAction(nameof(View), new { testingApplicationId = testingApplication.Id });
        }
    }
}