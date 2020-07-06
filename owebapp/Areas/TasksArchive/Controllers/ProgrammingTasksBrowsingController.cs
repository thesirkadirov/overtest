using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Sirkadirov.Overtest.WebApplication.Areas.TasksArchive.Controllers
{
    [Area("TasksArchive")]
    [Route("/TasksArchive/ProgrammingTasks")]
    public class ProgrammingTasksBrowsingController : Controller
    {
        [HttpGet, Route(nameof(View) + "/{programmingTaskId:guid")]
        public async Task<IActionResult> View(Guid programmingTaskId)
        {
            throw new NotImplementedException();
        }
    }
}