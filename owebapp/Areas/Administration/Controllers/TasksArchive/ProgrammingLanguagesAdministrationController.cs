using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sirkadirov.Overtest.Libraries.Shared.Database;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Identity;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.TasksArchive.Extras;
using Sirkadirov.Overtest.WebApplication.Extensions.Filters;

namespace Sirkadirov.Overtest.WebApplication.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Route("/Administration/TasksArchive/ProgrammingLanguages")]
    public class ProgrammingLanguagesAdministrationController : Controller
    {
        private const string ViewsDirectoryPath = "~/Areas/Administration/Views/TasksArchive/ProgrammingLanguagesAdministrationController/";
        
        private readonly OvertestDatabaseContext _databaseContext;

        public ProgrammingLanguagesAdministrationController(OvertestDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }
        
        [HttpGet, Route(nameof(List))]
        public async Task<IActionResult> List()
        {
            const string actionViewPath = ViewsDirectoryPath + nameof(List) + ".cshtml";
            
            var programmingLanguages = await _databaseContext.ProgrammingLanguages
                .OrderBy(o => o.DisplayName)
                .ToListAsync();
            
            return View(actionViewPath, programmingLanguages);
        }
        
        [AllowedUserTypesFilter(UserType.SuperUser)]
        [HttpGet, Route(nameof(Create))]
        public IActionResult Create()
        {
            return RedirectToAction(nameof(Edit));
        }
        
        [AllowedUserTypesFilter(UserType.SuperUser)]
        [HttpGet, Route(nameof(Edit) + "/{programmingLanguageId:guid?}")]
        public async Task<IActionResult> Edit(Guid? programmingLanguageId = null)
        {
            const string actionViewPath = ViewsDirectoryPath + nameof(Edit) + ".cshtml";

            if (programmingLanguageId == null)
                return View(actionViewPath, new ProgrammingLanguage { Id = default });

            if (!await _databaseContext.ProgrammingLanguages.AnyAsync(l => l.Id == programmingLanguageId.Value))
                return NotFound();
            
            return View(actionViewPath, await _databaseContext.ProgrammingLanguages.FirstAsync(l => l.Id == programmingLanguageId));
        }
        
        [AllowedUserTypesFilter(UserType.SuperUser)]
        [HttpPost, ValidateAntiForgeryToken, Route(nameof(Edit) + "/{programmingLanguageId:guid?}")]
        public async Task<IActionResult> Edit(ProgrammingLanguage programmingLanguage, Guid? programmingLanguageId = null)
        {
            const string actionViewPath = ViewsDirectoryPath + nameof(Edit) + ".cshtml";
            
            if (programmingLanguageId == null)
                programmingLanguage.Id = default;
            else
            {
                programmingLanguage.Id = programmingLanguageId.Value;

                if (!await _databaseContext.ProgrammingLanguages.AnyAsync(l => l.Id == programmingLanguage.Id))
                    return NotFound();
            }
            
            if (!ModelState.IsValid)
            {
                programmingLanguage.DisplayName = await _databaseContext.ProgrammingLanguages
                    .Where(l => l.Id == programmingLanguageId.Value)
                    .Select(s => s.DisplayName)
                    .FirstAsync();
                
                return View(actionViewPath, programmingLanguage);
            }

            if (programmingLanguage.Id == default)
                await _databaseContext.ProgrammingLanguages.AddAsync(programmingLanguage);
            else
                _databaseContext.ProgrammingLanguages.Update(programmingLanguage);

            await _databaseContext.SaveChangesAsync();

            programmingLanguageId = programmingLanguage.Id;
            
            return RedirectToAction(nameof(Edit), new { programmingLanguageId });
        }
        
        [AllowedUserTypesFilter(UserType.SuperUser)]
        [HttpPost, ValidateAntiForgeryToken, Route(nameof(Remove))]
        public async Task<IActionResult> Remove(Guid programmingLanguageId)
        {
            if (!await _databaseContext.ProgrammingLanguages.AnyAsync(l => l.Id == programmingLanguageId))
                return NotFound();

            _databaseContext.ProgrammingLanguages.Remove(new ProgrammingLanguage { Id = programmingLanguageId });
            await _databaseContext.SaveChangesAsync();

            return RedirectToAction(nameof(List));
        }
    }
}