using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Sirkadirov.Overtest.WebApplication.Controllers
{
    
    [Route("/Security")]
    public class SecurityController : Controller
    {
        
        private const string ViewsDirectoryPath = "~/Views/SecurityController/";
        
        [HttpGet]
        [AllowAnonymous]
        [Route(nameof(AccessDenied))]
        public IActionResult AccessDenied()
        {
            return Error(403);
        }
        
        [HttpGet]
        [AllowAnonymous]
        [Route(nameof(Error) + "/{statusCode:int}")]
        public IActionResult Error(int statusCode)
        {
            ViewData["StatusCode"] = statusCode;
            return View(ViewsDirectoryPath + "Error.cshtml");
        }

        [HttpGet]
        [AllowAnonymous]
        [Route(nameof(Banned))]
        public IActionResult Banned()
        {
            throw new NotImplementedException();
        }
        
    }
    
}