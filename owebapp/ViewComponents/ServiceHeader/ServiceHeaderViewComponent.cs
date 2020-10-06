using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
#pragma warning disable 1998

namespace Sirkadirov.Overtest.WebApplication.ViewComponents.ServiceHeader
{
    public class ServiceHeaderViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(HeaderModel model)
        {
            return View("~/ViewComponents/Views/ServiceHeader/ServiceHeader.cshtml", model);
        }
        
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
                public bool Enabled { get; set; }
                public string LinkIcon { get; set; }
                public string LinkText { get; set; }
                public string LinkDestination { get; set; }
            }

            public class MenuDrawerData
            {
                public bool Enabled { get; set; }
                public Func<dynamic, object> DrawMenuItems { get; set; }
            }
        }
    }
}