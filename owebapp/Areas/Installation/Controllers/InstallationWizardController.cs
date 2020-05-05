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
        
        public const int WizardStepsTotalCount = 3;
        
        public InstallationWizardController(OvertestDatabaseContext databaseContext, UserManager<User> userManager)
        {
            _databaseContext = databaseContext;
            _userManager = userManager;
        }

        [NonAction]
        private async Task<bool> InstallationRequiredAsync() => await _databaseContext.IsInstallationRequiredAsync();

        [NonAction]
        private async Task SetInstallationKey(bool value)
        {
            
            await _databaseContext.ConfigurationStorages.AddAsync(
                new ConfigurationStorage(ConfigurationStorage.CommonKeys.OvertestInstallationFinished, value.ToString())
            );

            await _databaseContext.SaveChangesAsync();
            
        }
        
        /*
         * Step 1: Welcome to the installation wizard
         */
        
        [HttpGet]
        [Route("")]
        [Route(nameof(Welcome))]
        public async Task<IActionResult> Welcome()
        {
            
            if (!await InstallationRequiredAsync())
                return new ForbidResult();
            
            return View("~/Areas/Installation/Views/InstallationWizardController/Welcome.cshtml");
            
        }
        
        /*
         * Step 2: SuperUser creation
         */

        [HttpGet]
        [Route(nameof(CreateSuperUser))]
        public async Task<IActionResult> CreateSuperUser()
        {
            
            if (!await InstallationRequiredAsync())
                return new ForbidResult();

            return View("~/Areas/Installation/Views/InstallationWizardController/CreateSuperUser.cshtml");

        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route(nameof(CreateSuperUser))]
        public async Task<IActionResult> CreateSuperUser(SuperUserCreationModel model)
        {

            const string nextActionName = nameof(FinishInstallation);
            
            if (!await InstallationRequiredAsync())
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
        
        [HttpGet]
        [Route(nameof(FinishInstallation))]
        public async Task<IActionResult> FinishInstallation()
        {
            
            if (!await InstallationRequiredAsync())
                return new ForbidResult();
            
            await SetInstallationKey(true);
            
            return View("~/Areas/Installation/Views/InstallationWizardController/FinishInstallation.cshtml");

        }
        
    }
    
}