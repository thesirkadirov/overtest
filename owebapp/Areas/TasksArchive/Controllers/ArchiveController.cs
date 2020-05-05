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
    public class ArchiveController : Controller
    {

        private const string ViewsDirectoryPath = "~/Areas/TasksArchive/Views/ArchiveController/";
        private const int ItemsPerPage = 20;
        
        private readonly OvertestDatabaseContext _databaseContext;
        
        public ArchiveController(OvertestDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }
        
        [HttpGet]
        [Route("")]
        [Route(nameof(List))]
        [Route("/TasksArchive")]
        public async Task<IActionResult> List(int page = 1, string category = "", string searchQuery = "")
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

            var databaseQuery = _databaseContext.ProgrammingTasks
                .Where(t =>
                    t.Enabled &&
                    (!string.IsNullOrWhiteSpace(category) && t.CategoryId.ToString() == category) &&
                    (t.Id.ToString() == searchQuery || EF.Functions.Like(t.Title, $"%{searchQuery}%"))
                )
                .OrderBy(t => t.Difficulty)
                .ThenBy(t => t.CategoryId)
                .ThenBy(t => t.Title);

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
                .OrderBy(c => c.DisplayName)
                .ThenBy(c => c.Id)
                .ToListAsync();

            return View(ViewsDirectoryPath + "Categories.cshtml", categoriesList);

        }
        
    }
    
}