using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Sirkadirov.Overtest.Libraries.Shared.Database;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Identity;
using Sirkadirov.Overtest.WebApplication.Extensions.Filters;
using Sirkadirov.Overtest.WebApplication.Models.AuthController;

namespace Sirkadirov.Overtest.WebApplication.Controllers
{
    [Route("/Auth")]
    public class AuthController : Controller
    {
        private readonly OvertestDatabaseContext _databaseContext;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IStringLocalizer<AuthController> _localizer;
        
        private const string ViewsDirectoryPath = "~/Views/AuthController/";
        
        public AuthController(OvertestDatabaseContext databaseContext, UserManager<User> userManager, SignInManager<User> signInManager, IStringLocalizer<AuthController> localizer)
        {
            _databaseContext = databaseContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _localizer = localizer;
        }
        
        [AllowAnonymous, DisallowAuthorizedFilter]
        [HttpGet, Route(nameof(Authorization) + "/{returnUrl?}")]
        public IActionResult Authorization(string returnUrl = null)
        {
            return View(ViewsDirectoryPath + "Authorization.cshtml",
                new AuthorizationModel
                {
                    RememberMe = true,
                    ReturnUrl = returnUrl
                }
            );
        }
        
        [AllowAnonymous, DisallowAuthorizedFilter]
        [HttpPost, ValidateAntiForgeryToken, Route(nameof(Authorization))]
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
                else if (user.IsBanned)
                {
                    ModelState.AddModelError(
                        nameof(AuthorizationModel.Email),
                        _localizer["Ваш обліковий запис було заблоковано за порушення умов роботи з системою!"].Value
                    );
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
                    else
                    {
                        ModelState.AddModelError(
                            nameof(AuthorizationModel.Email),
                            _localizer["Перевірте правильність введеної вами інформації! Можливо, вам слід скористатися сервісом відновлення доступу до системи?"].Value
                        );
                    }
                    
                }
                
            }
            
            return View(ViewsDirectoryPath + "Authorization.cshtml", authorizationModel);
            
        }
        
        [AllowAnonymous, DisallowAuthorizedFilter]
        [HttpGet, Route("/Invite/{securityToken}")]
        public IActionResult JoinByInviteSecurityToken(string securityToken)
        {
            return RedirectToAction(nameof(Registration), new { securityToken });
        }
        
        [AllowAnonymous, DisallowAuthorizedFilter]
        [HttpGet, Route(nameof(Registration))]
        public IActionResult Registration(string securityToken = null)
        {
            
            const string actionViewPath = ViewsDirectoryPath + nameof(Registration) + ".cshtml";
            
            return View(actionViewPath, new RegistrationModel
            {
                SecurityToken = securityToken ?? string.Empty
            });
            
        }
        
        [AllowAnonymous, DisallowAuthorizedFilter]
        [HttpPost, ValidateAntiForgeryToken, Route(nameof(Registration))]
        public async Task<IActionResult> Registration(RegistrationModel registrationModel, string securityToken = null)
        {
            
            const string actionViewPath = ViewsDirectoryPath + nameof(Registration) + ".cshtml";
            
            if (ModelState.IsValid)
            {
                
                if (string.IsNullOrWhiteSpace(securityToken))
                    securityToken = registrationModel.SecurityToken;

                if (!await _databaseContext.UserGroups.AnyAsync(g => g.AccessToken == securityToken))
                {
                    ModelState.AddModelError(
                        nameof(registrationModel.SecurityToken),
                        _localizer["Введено недійсний код доступу! Рекомендуємо вам зв'язатися з куратором, який надав цей код."]
                    );
                    return View(actionViewPath, registrationModel);
                }
                
                if (await _databaseContext.Users.AnyAsync(u => u.Email == registrationModel.Email))
                {
                    ModelState.AddModelError(
                        nameof(registrationModel.Email),
                        _localizer["Користувач зі вказаною адресою Email вже зареєстрований! Можливо, вам слід відновити пароль до свого облікового запису?"]
                    );
                    return View(actionViewPath, registrationModel);
                }

                var userGroupInfo = await _databaseContext.UserGroups
                    .Where(g => g.AccessToken == securityToken)
                    .Include(i => i.GroupCurator)
                    .Select(s => new
                    {
                        UserGroupId = s.Id,
                        CuratorInstitutionName = s.GroupCurator.InstitutionName
                    })
                    .FirstAsync();
                
                var newUser = new User
                {
                    Email = registrationModel.Email,
                    EmailConfirmed = true,
                    UserName = registrationModel.Email,
                    
                    FullName = registrationModel.FullName,
                    InstitutionName = userGroupInfo.CuratorInstitutionName,

                    UserGroupId = userGroupInfo.UserGroupId,
                    Type = UserType.Student,
                    
                    Registered = DateTime.UtcNow,
                    LastSeen = DateTime.UtcNow
                };

                var registrationResult = await _userManager.CreateAsync(newUser, registrationModel.Password);

                if (registrationResult.Succeeded)
                {
                    await _signInManager.SignInAsync(newUser, true);
                    return RedirectToAction("Welcome", "Welcome");
                }
                
                foreach (var identityError in registrationResult.Errors)
                {
                    ModelState.AddModelError(
                        nameof(registrationModel.Email),
                        $"Error #{identityError.Code}: {identityError.Description}"
                    );
                }
                
            }
            
            return View(actionViewPath, registrationModel);
            
        }
        
        [HttpPost, ValidateAntiForgeryToken, Route(nameof(LogOut))]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Welcome", "Welcome");
        }
    }
}