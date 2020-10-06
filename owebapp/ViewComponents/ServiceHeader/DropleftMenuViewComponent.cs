using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
#pragma warning disable 1998

namespace Sirkadirov.Overtest.WebApplication.ViewComponents.ServiceHeader
{
    public class DropleftMenuViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(Func<dynamic, object> menuDrawer)
        {
            return View("~/ViewComponents/Views/ServiceHeader/DropleftMenu.cshtml", menuDrawer);
        }
    }
}