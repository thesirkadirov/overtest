using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sirkadirov.Overtest.Libraries.Shared.Database;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Identity;
using Sirkadirov.Overtest.WebApplication.Areas.Installation.Models.InstallationWizardController;

namespace Sirkadirov.Overtest.WebApplication.Areas.Installation.Controllers
{
    
    [AllowAnonymous]
    [Area("Installation")]
    [Route("/Installation/Wizard")]
    public class InstallationWizardController : Controller
    {
        
        private readonly OvertestDatabaseContext _databaseContext;
        private readonly UserManager<User> _userManager;
        //private ILogger _logger;
        
        public const int WizardStepsTotalCount = 3;
        
        public InstallationWizardController(OvertestDatabaseContext databaseContext, UserManager<User> userManager)
        {
            _databaseContext = databaseContext;
            _userManager = userManager;
            //_logger = LogManager.GetCurrentClassLogger();
        }

        [NonAction]
        private bool InstallationRequiredAsync() => !_databaseContext.SystemConfiguration[ConfigurationStorage.CommonKeys.OvertestInstallationFinished].BoolValue;
        
        /*
         * Step 1: Welcome to the installation wizard
         */
        
        [HttpGet, Route(""), Route(nameof(Welcome))]
        public ActionResult Welcome()
        {
            
            if (!InstallationRequiredAsync())
                return new ForbidResult();
            
            return View("~/Areas/Installation/Views/InstallationWizardController/Welcome.cshtml");
            
        }
        
        /*
         * Step 2: SuperUser creation
         */
        
        [HttpGet, Route(nameof(CreateSuperUser))]
        public ActionResult CreateSuperUser()
        {
            
            if (!InstallationRequiredAsync())
                return new ForbidResult();

            return View("~/Areas/Installation/Views/InstallationWizardController/CreateSuperUser.cshtml");

        }
        
        [HttpPost, ValidateAntiForgeryToken, Route(nameof(CreateSuperUser))]
        public async Task<IActionResult> CreateSuperUser(SuperUserCreationModel model)
        {

            const string nextActionName = nameof(FinishInstallation);
            
            if (!InstallationRequiredAsync())
                return new ForbidResult();
            
            if (await _databaseContext.Users.AnyAsync())
                return RedirectToAction(nextActionName);
            
            if (ModelState.IsValid)
            {

                var identityResult = await _userManager.CreateAsync(new User
                {
                    Type = UserType.SuperUser,
                    IsBanned = false,
                    
                    FullName = model.FullName,
                    InstitutionName = model.InstitutionName,

                    UserName = model.Email,
                    Email = model.Email,
                    
                    UserGroupId = null,
                    
                    Registered = DateTime.UtcNow,
                    LastSeen = DateTime.UtcNow
                }, model.Password);
                
                if (identityResult.Succeeded)
                    return RedirectToAction(nextActionName);

                foreach (var identityError in identityResult.Errors)
                {
                    ModelState.AddModelError("", $"{identityError.Code}: {identityError.Description}");
                }

            }
            
            return View("~/Areas/Installation/Views/InstallationWizardController/CreateSuperUser.cshtml", model);
            
        }
        
        /*
         * Step 3: Finish installation
         */
        
        [HttpGet, Route(nameof(FinishInstallation))]
        public IActionResult FinishInstallation()
        {
            
            if (!InstallationRequiredAsync())
                return new ForbidResult();
            
            _databaseContext.SystemConfiguration.Update(ConfigurationStorage.CommonKeys.OvertestInstallationFinished, true);
            
            return View("~/Areas/Installation/Views/InstallationWizardController/FinishInstallation.cshtml");

        }
        
    }
    
}