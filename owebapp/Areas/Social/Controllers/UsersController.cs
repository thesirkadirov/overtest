using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sirkadirov.Overtest.Libraries.Shared.Database;

namespace Sirkadirov.Overtest.WebApplication.Areas.Social.Controllers
{
    
    [Area("Social")]
    [Route("/Social/Users")]
    public class UsersController : Controller
    {
        
        private const string ViewsDirectoryPath = "~/Areas/Social/Views/UsersController/";
        
        private readonly OvertestDatabaseContext _databaseContext;
        
        public UsersController(OvertestDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }
        
        [HttpGet]
        [Route("Profile")]
        public IActionResult Profile()
        {
            return RedirectToAction(nameof(UserProfile), new
            {
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            });
        }
        
        [HttpGet]
        [Route("{userId:Guid}")]
        [Route("Profile/{userId:Guid}")]
        public async Task<IActionResult> UserProfile(Guid userId)
        {
            
            var userInfo = await _databaseContext.Users
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();
            
            if (userInfo == null)
                return NotFound();
            
            return View(ViewsDirectoryPath + "UserProfile.cshtml", userInfo);
            
        }
        
    }
    
}