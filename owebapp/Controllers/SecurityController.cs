using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Sirkadirov.Overtest.WebApplication.Controllers
{
    
    [Route("/Security")]
    public class SecurityController : Controller
    {
        
        private const string ViewsDirectoryPath = "~/Views/SecurityController/";
        
        [AllowAnonymous, HttpGet, Route(nameof(AccessDenied))]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public IActionResult AccessDenied(string ReturnUrl = null)
        {
            return Error(403, ReturnUrl);
        }
        
        [AllowAnonymous, HttpGet, Route(nameof(Error) + "/{statusCode:int}")]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public IActionResult Error(int statusCode, string ReturnUrl = null)
        {
            ViewData["StatusCode"] = statusCode;
            
            // Return url
            if (!string.IsNullOrWhiteSpace(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                ViewData[nameof(ReturnUrl)] = ReturnUrl;
            else
                ViewData[nameof(ReturnUrl)] = null;
            
            return View(ViewsDirectoryPath + "Error.cshtml");
        }

        [AllowAnonymous, HttpGet, Route(nameof(Banned))]
        public IActionResult Banned()
        {
            throw new NotImplementedException();
        }
        
    }
    
}