using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Sirkadirov.Overtest.Libraries.Shared.Database;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Identity;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.TasksArchive;
using Sirkadirov.Overtest.WebApplication.Areas.Administration.Models.TasksArchive.ProgrammingTasksAdministrationController;
using Sirkadirov.Overtest.WebApplication.Extensions.Filters;

namespace Sirkadirov.Overtest.WebApplication.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Route("/Administration/TasksArchive/ProgrammingTasks")]
    [AllowedUserTypesFilter(UserType.SuperUser)]
    public class ProgrammingTasksAdministrationController : Controller
    {
        
        private const string ViewsDirectoryPath = "~/Areas/Administration/Views/TasksArchive/ProgrammingTasksAdministrationController/";
        
        private readonly OvertestDatabaseContext _databaseContext;
        private readonly IStringLocalizer<ProgrammingTasksAdministrationController> _localizer;

        public ProgrammingTasksAdministrationController(OvertestDatabaseContext databaseContext, IStringLocalizer<ProgrammingTasksAdministrationController> localizer)
        {
            _databaseContext = databaseContext;
            _localizer = localizer;
        }
        
        #region Create programming task
        
        [HttpGet, Route(nameof(Create))]
        public IActionResult Create()
        {
            const string actionViewPath = ViewsDirectoryPath + nameof(Create) + ".cshtml";
            
            return View(actionViewPath, new ProgrammingTaskCreationModel
            {
                VisibleInFreeMode = false,
                VisibleInCompetitionMode = false
            });
        }
        
        [HttpPost, Route(nameof(Create))]
        public async Task<IActionResult> Create(ProgrammingTaskCreationModel model)
        {
            const string actionViewPath = ViewsDirectoryPath + nameof(Create) + ".cshtml";
            
            if (!ModelState.IsValid)
                return View(actionViewPath, model);
            
            var programmingTask = new ProgrammingTask
            {
                Title = model.Title,
                Description = _localizer["Перейти на темну сторону сили!"],
                
                VisibleInFreeMode = model.VisibleInFreeMode,
                VisibleInCompetitionMode = model.VisibleInCompetitionMode,
                
                Difficulty = 0,
                
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            };
            
            await _databaseContext.ProgrammingTasks.AddAsync(programmingTask);
            await _databaseContext.SaveChangesAsync();
            
            return RedirectToAction(nameof(EditDetails), new { programmingTaskId = programmingTask.Id });
        }
        
        #endregion
        
        #region Edit details
        
        [HttpGet, Route(nameof(EditDetails) + "/{programmingTaskId:guid}")]
        public async Task<IActionResult> EditDetails(Guid programmingTaskId)
        {
            const string actionViewPath = ViewsDirectoryPath + nameof(EditDetails) + ".cshtml";
            
            if (!await ProgrammingTaskExists(programmingTaskId))
                return NotFound();
            
            var model = await _databaseContext.ProgrammingTasks
                .Where(t => t.Id == programmingTaskId)
                .Select(s => new ProgrammingTaskEditDetailsModel
                {
                    ProgrammingTaskId = s.Id,
                    
                    Title = s.Title,
                    Description = s.Description,
                    Difficulty = s.Difficulty,
                    
                    VisibleInFreeMode = s.VisibleInFreeMode,
                    VisibleInCompetitionMode = s.VisibleInCompetitionMode,
                    
                    CategoryId = s.CategoryId
                })
                .AsNoTracking()
                .FirstAsync();
            
            return View(actionViewPath, model);
        }
        
        [HttpPost, Route(nameof(EditDetails) + "/{programmingTaskId:guid}")]
        public async Task<IActionResult> EditDetails(Guid programmingTaskId, ProgrammingTaskEditDetailsModel model)
        {
            const string actionViewPath = ViewsDirectoryPath + nameof(EditDetails) + ".cshtml";
            
            if (!await ProgrammingTaskExists(programmingTaskId))
                return NotFound();
            
            if (!ModelState.IsValid)
                return View(actionViewPath, model);
            
            model.ProgrammingTaskId = programmingTaskId;

            if (model.CategoryId != null && !await _databaseContext.ProgrammingTaskCategories.AnyAsync(c => c.Id == model.CategoryId))
            {
                ModelState.AddModelError(
                    nameof(model.CategoryId),
                    _localizer["Вказаної вами категорії завдань не знайдено!"]
                );
                return View(actionViewPath, model);
            }
            
            var programmingTaskObject = await _databaseContext.ProgrammingTasks
                .Where(t => t.Id == programmingTaskId)
                .FirstAsync();
            
            programmingTaskObject.Title = model.Title;
            programmingTaskObject.Description = model.Description;
            programmingTaskObject.Difficulty = model.Difficulty;

            programmingTaskObject.CategoryId = model.CategoryId;
            
            programmingTaskObject.VisibleInFreeMode = model.VisibleInFreeMode;
            programmingTaskObject.VisibleInCompetitionMode = model.VisibleInCompetitionMode;
            
            programmingTaskObject.LastModified = DateTime.UtcNow;
            
            _databaseContext.ProgrammingTasks.Update(programmingTaskObject);
            await _databaseContext.SaveChangesAsync();
            
            return RedirectToAction("View", "ProgrammingTasksBrowsing", new
            {
                area = "TasksArchive",
                programmingTaskId
            });
        }
        
        #endregion
        
        #region Edit testing data
        
        [HttpGet, Route(nameof(EditTestingData) + "/{programmingTaskId:guid}")]
        public async Task<IActionResult> EditTestingData(Guid programmingTaskId)
        {
            const string actionViewPath = ViewsDirectoryPath + nameof(EditTestingData) + ".cshtml";
            
            if (!await ProgrammingTaskExists(programmingTaskId))
                return NotFound();

            return View(actionViewPath, new ProgrammingTaskEditTestingDataModel { ProgrammingTaskId = programmingTaskId });
        }
        
        [HttpPost, ValidateAntiForgeryToken, Route(nameof(EditTestingData) + "/{programmingTaskId:guid}")]
        public async Task<IActionResult> EditTestingData(Guid programmingTaskId, ProgrammingTaskEditTestingDataModel model)
        {
            const string actionViewPath = ViewsDirectoryPath + nameof(EditTestingData) + ".cshtml";
            
            if (!await ProgrammingTaskExists(programmingTaskId))
                return NotFound();
            
            model.ProgrammingTaskId = programmingTaskId;
            
            if (!ModelState.IsValid)
            {
                model.TestingDataFile = null;
                return View(actionViewPath, model);
            }
            
            try
            {
                if (model.TestingDataFile == null)
                    throw new FormatException();
                
                if (model.TestingDataFile.Length == 0)
                    throw new FormatException();
                
                var fileExtension = (Path.GetExtension(model.TestingDataFile.FileName) ?? throw new FormatException())
                    .Replace(".", string.Empty);
                
                if (fileExtension != "zip")
                    throw new FormatException();
            }
            catch
            {
                ModelState.AddModelError(
                    nameof(model.TestingDataFile),
                    _localizer["Завантажений вами файл не є допустимим типом файлу!"]
                );
                model.TestingDataFile = null;
                return View(actionViewPath, model);
            }
            
            var programmingTaskObject = new ProgrammingTask { Id = programmingTaskId };
            
            using (var binaryReader = new BinaryReader(model.TestingDataFile.OpenReadStream()))
            {
                programmingTaskObject.TestingData = new ProgrammingTask.ProgrammingTaskTestingData
                {
                    DataPackageFile = binaryReader.ReadBytes((int) model.TestingDataFile.Length),
                    DataPackageHash = Guid.NewGuid().ToString()
                };
            }
            
            model.TestingDataFile = null;
            
            _databaseContext.ProgrammingTasks.Attach(programmingTaskObject);
            _databaseContext.Entry(programmingTaskObject.TestingData).Property(p => p.DataPackageFile).IsModified = true;
            _databaseContext.Entry(programmingTaskObject.TestingData).Property(p => p.DataPackageHash).IsModified = true;
            await _databaseContext.SaveChangesAsync();
            
            return RedirectToAction("View", "ProgrammingTasksBrowsing", new
            {
                area = "TasksArchive",
                programmingTaskId
            });
        }
        
        #endregion

        [HttpGet, Route(nameof(DownloadTestingData) + "/{programmingTaskId:guid}")]
        public async Task<IActionResult> DownloadTestingData(Guid programmingTaskId)
        {
            const string contentType = "application/zip";
            
            if (!await ProgrammingTaskExists(programmingTaskId))
                return NotFound();

            var testingData = await _databaseContext.ProgrammingTasks
                .Where(t => t.Id == programmingTaskId)
                .Select(s => s.TestingData.DataPackageFile)
                .FirstAsync();

            if (testingData == null)
                return NotFound();

            HttpContext.Response.ContentType = contentType;
            
            return new FileContentResult(testingData, contentType)
            {
                FileDownloadName = $"TestingData_{programmingTaskId}_{DateTime.UtcNow.ToShortDateString()}_{DateTime.UtcNow.ToShortTimeString()}.zip"
            };
        }
        
        [HttpPost, ValidateAntiForgeryToken, Route(nameof(Remove) + "/{programmingTaskId:guid}")]
        public async Task<IActionResult> Remove(Guid programmingTaskId)
        {
            if (!await ProgrammingTaskExists(programmingTaskId))
                return NotFound();
            
            var taskCategoryId = await _databaseContext.ProgrammingTasks
                .AsNoTracking()
                .Where(t => t.Id == programmingTaskId)
                .Select(s => s.CategoryId)
                .FirstAsync();
            
            _databaseContext.ProgrammingTasks.Remove(new ProgrammingTask { Id = programmingTaskId });
            await _databaseContext.SaveChangesAsync();
            
            return RedirectToAction("List", "ProgrammingTasksArchive", new { area = "TasksArchive", category = taskCategoryId });
        }
        
        [NonAction]
        private async Task<bool> ProgrammingTaskExists(Guid programmingTaskId) =>
            await _databaseContext.ProgrammingTasks.AnyAsync(t => t.Id == programmingTaskId);
        
    }
}