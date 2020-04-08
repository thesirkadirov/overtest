using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Sirkadirov.Overtest.WebApplication.Controllers
{
    
    [Route("/Security")]
    public class SecurityController : Controller
    {
        
        [HttpGet]
        [AllowAnonymous]
        [Route(nameof(AccessDenied))]
        public IActionResult AccessDenied()
        {
            throw new NotImplementedException();
        }
        
        [HttpGet]
        [AllowAnonymous]
        [Route(nameof(Error) + "/{statusCode:int}")]
        public IActionResult Error(int statusCode)
        {
            throw new NotImplementedException();
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