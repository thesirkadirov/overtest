using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sirkadirov.Overtest.Libraries.Shared.Database;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Identity;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.TasksArchive;
using Sirkadirov.Overtest.WebApplication.Extensions.Filters;

namespace Sirkadirov.Overtest.WebApplication.Areas.Administration.Controllers
{
    
    [Area("Administration")]
    [Route("/Administration/TasksArchive/Categories")]
    [AllowedUserTypesFilter(UserType.Administrator, UserType.SuperUser)]
    public class CategoriesAdministrationController : Controller
    {
        
        private const string ViewsDirectoryPath = "~/Areas/Administration/Views/TasksArchive/CategoriesAdministrationController/";
        
        private readonly OvertestDatabaseContext _databaseContext;
        
        public CategoriesAdministrationController(OvertestDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }
        
        [HttpGet]
        [Route(nameof(Create))]
        public IActionResult Create()
        {
            return RedirectToAction(nameof(Edit));
        }
        
        [HttpGet]
        [Route(nameof(Edit) + "/{categoryId:Guid?}")]
        public async Task<IActionResult> Edit(Guid? categoryId = null)
        {

            ProgrammingTaskCategory model = null;

            // ReSharper disable once InvertIf
            if (categoryId != null)
            {
                if (!await CategoryExistsAsync(categoryId.Value))
                    return NotFound();
                
                model = await _databaseContext.ProgrammingTaskCategories
                    .Where(c => c.Id == categoryId)
                    .AsNoTracking()
                    .FirstAsync();
                
            }
            
            return View(ViewsDirectoryPath + "Edit.cshtml", model);
            
        }
        
        [HttpPost, ValidateAntiForgeryToken, Route(nameof(Edit) + "/{categoryId:Guid?}")]
        public async Task<IActionResult> Edit(ProgrammingTaskCategory model, Guid? categoryId = null)
        {

            // ReSharper disable once InvertIf
            if (ModelState.IsValid)
            {
                
                /*
                 * Create a new tasks category
                 */
                
                if (categoryId == null)
                {
                    
                    var createdCategory = new ProgrammingTaskCategory
                    {
                        DisplayName = model.DisplayName,
                        Description = model.Description
                    };

                    await _databaseContext.ProgrammingTaskCategories.AddAsync(createdCategory);
                    await _databaseContext.SaveChangesAsync();
                    
                    return RedirectToAction("Categories", "Archive", new {area = "TasksArchive"});
                    
                }
                
                /*
                 * Update existing tasks category
                 */
                
                if (!await CategoryExistsAsync(categoryId.Value))
                    return NotFound();

                var modifiedCategory = await _databaseContext.ProgrammingTaskCategories
                    .Where(c => c.Id == categoryId)
                    .FirstAsync();

                modifiedCategory.DisplayName = model.DisplayName;
                modifiedCategory.Description = model.Description;

                _databaseContext.ProgrammingTaskCategories.Update(modifiedCategory);
                await _databaseContext.SaveChangesAsync();

                return RedirectToAction(nameof(Edit), new { categoryId });

            }

            return View(ViewsDirectoryPath + "Edit.cshtml", model);

        }
        
        [NonAction]
        private async Task<bool> CategoryExistsAsync(Guid id)
        {
            return await _databaseContext.ProgrammingTaskCategories
                .Where(c => c.Id == id)
                .AnyAsync();
        }
        
    }
    
}