using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Sirkadirov.Overtest.WebApplication.Controllers
{
    
    [Route("/")]
    [Route("/Welcome")]
    public class WelcomeController : Controller
    {
        
        private const string ViewsDirectoryPath = "~/Views/WelcomeController/";
        
        [AllowAnonymous, HttpGet, Route("")]
        public IActionResult Welcome()
        {
            return View(ViewsDirectoryPath + "Welcome.cshtml");
        }
        
        [AllowAnonymous, HttpGet, Route(nameof(Home))]
        public IActionResult Home()
        {
            return View(ViewsDirectoryPath + "Home.cshtml");
        }
        
    }
    
}