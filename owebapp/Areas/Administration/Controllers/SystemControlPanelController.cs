using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sirkadirov.Overtest.Libraries.Shared.Database;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Identity;
using Sirkadirov.Overtest.WebApplication.Extensions.Filters;

namespace Sirkadirov.Overtest.WebApplication.Areas.Administration.Controllers
{
    
    [Area("Administration")]
    [Route("/Administration/ControlPanel")]
    public class SystemControlPanelController : Controller
    {
        private const string ViewsDirectoryPath = "~/Areas/Administration/Views/SystemControlPanelController/";
        
        private readonly OvertestDatabaseContext _databaseContext;
        private readonly UserManager<User> _userManager;

        public SystemControlPanelController(OvertestDatabaseContext databaseContext, UserManager<User> userManager)
        {
            _databaseContext = databaseContext;
            _userManager = userManager;
        }
        
        [HttpGet, Route(""), AllowedUserTypesFilter(UserType.Curator, UserType.SuperUser)]
        public async Task<IActionResult> ControlPanel()
        {
            var currentUserId = new Guid(_userManager.GetUserId(HttpContext.User));
            switch (await _databaseContext.UserPermissionsOperator.GetUserTypeByIdAsync(currentUserId))
            {
                case UserType.Curator:
                    return RedirectToAction(nameof(Curator));
                case UserType.SuperUser:
                    return RedirectToAction(nameof(SuperUser));
            }
            return new ForbidResult();
        }
        
        [HttpGet, Route(nameof(Curator)), AllowedUserTypesFilter(UserType.Curator)]
        public IActionResult Curator()
        {
            throw new NotImplementedException();
        }
        
        [HttpGet, Route(nameof(SuperUser)), AllowedUserTypesFilter(UserType.SuperUser)]
        public IActionResult SuperUser()
        {
            return View(ViewsDirectoryPath + "SuperUser.cshtml");
        }
    }
}