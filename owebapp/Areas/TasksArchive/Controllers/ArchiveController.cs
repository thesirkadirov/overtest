using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sirkadirov.Overtest.Libraries.Shared.Database;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.TasksArchive;
using Sirkadirov.Overtest.WebApplication.Models.Shared;

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
        [Route(nameof(List) + "/{page:int:min(1)?}")]
        public async Task<IActionResult> List(int page = 1, string category = "", string searchQuery = "")
        {

            var model = new PaginatedListModel<ProgrammingTask>
            {
                CurrentPage = page,
                ItemsPerPage = ItemsPerPage
            };

            var databaseQuery = _databaseContext.ProgrammingTasks
                .Where(t =>
                    t.Enabled &&
                    (!string.IsNullOrWhiteSpace(category) && t.CategoryId.ToString() == category) &&
                    EF.Functions.Like(t.Title, $"%{searchQuery}%")
                )
                .OrderBy(t => t.Difficulty)
                .ThenBy(t => t.CategoryId)
                .ThenBy(t => t.Title);

            model.TotalItems = await databaseQuery.CountAsync();
            
            model.ItemsList = await databaseQuery
                .Skip(model.PreviousItemsCount).Take(ItemsPerPage)
                .Select(t => new ProgrammingTask
                {
                    Id = t.Id,
                    Title = t.Title,
                    Difficulty = t.Difficulty,
                    CategoryId = t.CategoryId
                }).ToListAsync();
            
            return View(ViewsDirectoryPath + "List.cshtml", model);
        }
        
    }
    
}