using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.EntityFrameworkCore;
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
        private readonly IViewLocalizer _localizer;
        
        public AuthController(OvertestDatabaseContext databaseContext, SignInManager<User> signInManager, IViewLocalizer localizer)
        {
            _databaseContext = databaseContext;
            _signInManager = signInManager;
            _localizer = localizer;
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
            
            if (ModelState.IsValid)
            {
                
                var user = await _databaseContext.Users.Where(u => u.Email == authorizationModel.Email)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
            
                if (user == null)
                {
                    ModelState.AddModelError(
                        nameof(AuthorizationModel.Email),
                        _localizer["Користувача зі вказаною адресою електронної пошти не знайдено!"].Value
                    );
                }
                else if (!user.IsApprovedUser())
                {
                    if (user.IsBanned)
                    {
                        ModelState.AddModelError(
                            nameof(AuthorizationModel.Email),
                            _localizer["Ваш обліковий запис було заблоковано за порушення умов роботи з системою!"].Value
                        );
                    }
                    else
                    {
                        ModelState.AddModelError(
                            nameof(AuthorizationModel.Email),
                            _localizer["Ваш обліковий запис ще не отримав підтвердження від обраного при реєстрації в системі куратора!"].Value
                        );
                    }
                }
                else
                {
                    
                    var signInResult = await _signInManager.PasswordSignInAsync(
                        user,
                        authorizationModel.Password,
                        authorizationModel.RememberMe,
                        false
                    );

                    if (signInResult.IsLockedOut)
                    {
                        ModelState.AddModelError(
                            nameof(AuthorizationModel.Email),
                            _localizer["Ваш обліковий запис тимчасово відключено через значну кількість спроб авторизації у системі! Повторіть спробу авторизації через деякий час та проінформуйте вашого куратора."].Value
                        );
                    }
                    else if (signInResult.Succeeded)
                    {

                        if (!string.IsNullOrWhiteSpace(authorizationModel.ReturnUrl) &&
                            Url.IsLocalUrl(authorizationModel.ReturnUrl))
                            return LocalRedirect(authorizationModel.ReturnUrl);
                        
                        return RedirectToAction("Home", "Welcome");
                    }
                    
                }
                
            }
            
            ModelState.AddModelError(
                nameof(AuthorizationModel.Email),
                _localizer["Перевірте правильність введеної вами інформації! Можливо, вам слід скористатися сервісом відновлення доступу до системи?"].Value
            );
            
            return View("~/Views/AuthController/Authorization.cshtml", authorizationModel);
            
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