using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sirkadirov.Overtest.Libraries.Shared.Database;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.TasksArchive;
using Sirkadirov.Overtest.WebApplication.Models.Shared.Pagination;

namespace Sirkadirov.Overtest.WebApplication.Areas.TasksArchive.Controllers
{
    [Area("TasksArchive")]
    [Route("/TasksArchive/Archive")]
    public class ProgrammingTasksArchiveController : Controller
    {
        
        private const string ViewsDirectoryPath = "~/Areas/TasksArchive/Views/ProgrammingTasksArchiveController/";
        private const int ItemsPerPage = 27;
        
        private readonly OvertestDatabaseContext _databaseContext;
        
        public ProgrammingTasksArchiveController(OvertestDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }
        
        [HttpGet]
        [Route(nameof(List))]
        public async Task<IActionResult> List(int page = 1, Guid? category = null, string searchQuery = "")
        {

            if (page < 1)
                return NotFound();
            
            ViewData["SelectedCategoryId"] = category;
            ViewData["SearchQuery"] = searchQuery;
            
            var model = new PaginatedListModel<ProgrammingTask>
            {
                Pagination = new PaginationInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = ItemsPerPage
                }
            };
            
            var databaseQuery =
                (
                    from programmingTask in _databaseContext.ProgrammingTasks
                    where programmingTask.VisibleInFreeMode
                    where EF.Functions.Like(programmingTask.Title, $"%{searchQuery}%")
                    where (category == null || programmingTask.CategoryId == category)
                    select programmingTask
                )
                .OrderBy(t => t.Difficulty)
                .ThenBy(t => t.CategoryId)
                .ThenBy(t => t.Title)
                .AsNoTracking();
            
            model.Pagination.TotalItems = await databaseQuery.CountAsync();
            
            if (model.Pagination.TotalItems <= 0 && page > 1)
                return NotFound();
            
            if (model.Pagination.TotalItems > 0 && model.Pagination.TotalPages < page)
                return NotFound();
            
            model.ItemsList = await databaseQuery
                .Skip(model.Pagination.PreviousItemsCount).Take(ItemsPerPage)
                .Select(t => new ProgrammingTask
                {
                    Id = t.Id,
                    Title = t.Title,
                    Difficulty = t.Difficulty,
                    CategoryId = t.CategoryId
                }).ToListAsync();
            
            return View(ViewsDirectoryPath + "List.cshtml", model);
            
        }

        [HttpGet]
        [Route(nameof(Categories))]
        public async Task<IActionResult> Categories()
        {

            var categoriesList = await _databaseContext.ProgrammingTaskCategories
                .AsNoTracking()
                .OrderBy(c => c.DisplayName)
                .ThenBy(c => c.Id)
                .ToListAsync();

            return View(ViewsDirectoryPath + "Categories.cshtml", categoriesList);

        }
        
    }
}