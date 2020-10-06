using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Sirkadirov.Overtest.WebApplication.ViewComponents.Shared
{

    public class ServiceHeaderViewComponent : ViewComponent
    {
        
#pragma warning disable 1998
        public async Task<IViewComponentResult> InvokeAsync(HeaderModel model)
        {
            return View("~/Views/Shared/ViewComponents/Shared/ServiceHeader.cshtml", model);
        }
#pragma warning restore 1998

        public class HeaderModel
        {

            public HeaderModel()
            {
                SubtitleLink = new IconLinkData();
                MenuDrawer = new MenuDrawerData();
            }
            
            public string PageTitle { get; set; }
            
            public IconLinkData SubtitleLink { get; set; }
            public MenuDrawerData MenuDrawer { get; set; }

            public class IconLinkData
            {
                public bool Enabled { get; set; } = false;
                public string LinkIcon { get; set; }
                public string LinkText { get; set; }
                public string LinkDestination { get; set; }
            }

            public class MenuDrawerData
            {
                public bool Enabled { get; set; } = false;
                public Func<dynamic, object> DrawMenuItems { get; set; }
            }
            
        }
        
    }
    
}