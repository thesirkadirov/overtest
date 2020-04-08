using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sirkadirov.Overtest.Libraries.Shared.Database;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Identity;
using Sirkadirov.Overtest.WebApplication.Models.AuthController;

namespace Sirkadirov.Overtest.WebApplication.Controllers
{
    
    [Route("/Auth")]
    public class AuthController : Controller
    {

        private readonly OvertestDatabaseContext _databaseContext;
        private readonly SignInManager<User> _signInManager;
        
        public AuthController(OvertestDatabaseContext databaseContext, SignInManager<User> signInManager)
        {
            _databaseContext = databaseContext;
            _signInManager = signInManager;
        }
        
        [HttpGet]
        [AllowAnonymous]
        [Route(nameof(Authorization) + "/{returnUrl?}")]
        public IActionResult Authorization(string returnUrl = null)
        {
            return View("~/Views/AuthController/Authorization.cshtml",
                new AuthorizationModel
                {
                    RememberMe = true,
                    ReturnUrl = returnUrl
                }
            );
        }
        
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route(nameof(Authorization))]
        public async Task<IActionResult> Authorization(AuthorizationModel authorizationModel)
        {
            throw new NotImplementedException();
        }
        
        [HttpGet]
        [AllowAnonymous]
        [Route(nameof(Registration))]
        public IActionResult Registration()
        {
            throw new NotImplementedException();
        }
        
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route(nameof(Registration))]
        public async Task<IActionResult> Registration(RegistrationModel registrationModel)
        {
            throw new NotImplementedException();
        }
        
        [HttpGet]
        [Route(nameof(LogOut))]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Welcome", "Welcome");
        }
        
    }
    
}