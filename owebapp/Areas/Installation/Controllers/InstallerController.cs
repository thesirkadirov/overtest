using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sirkadirov.Overtest.Libraries.Shared.Database;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Identity;

namespace Sirkadirov.Overtest.WebApplication.Areas.Installation.Controllers
{
    
    [AllowAnonymous]
    [Area("Installation")]
    [Route("/Installation/Installer")]
    public class InstallerController : Controller
    {

        private readonly OvertestDatabaseContext _databaseContext;
        private readonly UserManager<User> _userManager;
        
        public InstallerController(OvertestDatabaseContext databaseContext, UserManager<User> userManager)
        {
            _databaseContext = databaseContext;
            _userManager = userManager;
        }
        
        /*
         * Step 0: Welcome to the installation wizard
         */
        
        [HttpGet]
        [Route(nameof(Welcome))]
        public IActionResult Welcome()
        {
            throw new NotImplementedException();
        }
        
        /*
         * Step 1: Create super user
         */
        
        [HttpGet]
        [Route(nameof(CreateSuperUser))]
        public IActionResult CreateSuperUser()
        {
            throw new NotImplementedException();
        }
        
        [HttpPost]
        [Route(nameof(CreateSuperUser))]
        public IActionResult CreateSuperUser(User user)
        {
            throw new NotImplementedException();
        }
        
        /*
         * Step 2: Initialize tasks archive
         */
        
        [HttpGet]
        [Route(nameof(InitializeTasksArchive))]
        public IActionResult InitializeTasksArchive()
        {
            throw new NotImplementedException();
        }
        
        [HttpPost]
        [Route(nameof(InitializeTasksArchive))]
        public IActionResult InitializeTasksArchive(string repositoryUrl)
        {
            throw new NotImplementedException();
        }
        
        /*
         * Step 3: Finish installation
         */
        
    }
    
}