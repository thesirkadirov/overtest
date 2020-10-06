using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Sirkadirov.Overtest.WebApplication.ViewComponents.Shared
{

    public class DropleftMenuViewComponent : ViewComponent
    {
        
#pragma warning disable 1998
        public async Task<IViewComponentResult> InvokeAsync(Func<dynamic, object> menuDrawer)
        {
            
            return View("~/Views/Shared/ViewComponents/Shared/DropleftMenu.cshtml", menuDrawer);

        }
#pragma warning restore 1998
        
    }
    
}