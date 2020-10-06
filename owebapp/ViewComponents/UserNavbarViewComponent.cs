using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Identity;

namespace Sirkadirov.Overtest.WebApplication.ViewComponents.Shared
{

    public class UserNavbarViewComponent : ViewComponent
    {
        
        private readonly UserManager<User> _userManager;
        
        public UserNavbarViewComponent(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        
        public async Task<IViewComponentResult> InvokeAsync()
        {
            
            if (!HttpContext.User.Identity.IsAuthenticated)
                return Content(string.Empty);

            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            return View("~/Views/Shared/ViewComponents/Shared/UserNavbar.cshtml", currentUser);

        }
        
    }
    
}