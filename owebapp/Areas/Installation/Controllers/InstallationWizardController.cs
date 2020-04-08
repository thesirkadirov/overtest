using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sirkadirov.Overtest.Libraries.Shared.Database;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Identity;

namespace Sirkadirov.Overtest.WebApplication.Areas.Installation.Controllers
{
    
    [AllowAnonymous]
    [Area("Installation")]
    [Route("/Installation/Wizard")]
    public class InstallationWizardController : Controller
    {

        private readonly OvertestDatabaseContext _databaseContext;
        private readonly UserManager<User> _userManager;
        
        public const int WizardStepsTotalCount = 3;
        
        public InstallationWizardController(OvertestDatabaseContext databaseContext, UserManager<User> userManager)
        {
            _databaseContext = databaseContext;
            _userManager = userManager;
        }

        private async Task<bool> InstallationRequired()
        {
            return !await _databaseContext.Users.AnyAsync();
        }
        
        /*
         * Step 0: Welcome to the installation wizard
         */
        
        [HttpGet]
        [Route(nameof(Welcome))]
        public async Task<IActionResult> Welcome()
        {
            
            if (!await InstallationRequired())
                return new ForbidResult();
            
            return View("~/Areas/Installation/Views/InstallationWizardController/Wizard.cshtml");
            
        }
        
    }
    
}