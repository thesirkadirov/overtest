using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Sirkadirov.Overtest.WebApplication.Controllers
{
    
    [Route("/Security")]
    public class SecurityController : Controller
    {
        
        private const string ViewsDirectoryPath = "~/Views/SecurityController/";
        
        [AllowAnonymous, HttpGet, Route(nameof(AccessDenied))]
        public IActionResult AccessDenied()
        {
            return Error(403);
        }
        
        [AllowAnonymous, HttpGet, Route(nameof(Error) + "/{statusCode:int}")]
        public IActionResult Error(int statusCode)
        {
            ViewData["StatusCode"] = statusCode;
            return View(ViewsDirectoryPath + "Error.cshtml");
        }

        [AllowAnonymous, HttpGet, Route(nameof(Banned))]
        public IActionResult Banned()
        {
            throw new NotImplementedException();
        }
        
    }
    
}