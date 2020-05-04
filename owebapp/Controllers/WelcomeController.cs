using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Sirkadirov.Overtest.WebApplication.Controllers
{
    
    [Route("/")]
    [Route("/Welcome")]
    public class WelcomeController : Controller
    {
        
        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public IActionResult Welcome()
        {
            return View("~/Views/WelcomeController/Welcome.cshtml");
        }
        
        [HttpGet]
        [Route(nameof(Home))]
        public IActionResult Home()
        {
            return View("~/Views/WelcomeController/Home.cshtml");
        }
        
    }
    
}