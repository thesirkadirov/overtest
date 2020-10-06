using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
#pragma warning disable 1998

namespace Sirkadirov.Overtest.WebApplication.ViewComponents.GlobalModalDialogs
{
    public class GlobalLogoutModalDialogViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
                return Content(string.Empty);
            
            return View("~/ViewComponents/Views/GlobalModalDialogs/GlobalLogoutModalDialog.cshtml");
        }
    }
}