using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sirkadirov.Overtest.Libraries.Shared.Database;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Identity;

namespace Sirkadirov.Overtest.WebApplication.Extensions.Filters
{
    
    public class AllowedUserTypesFilterAttribute : Attribute, IAuthorizationFilter
    {

        private readonly UserType[] _allowedUserTypes;
        
        public AllowedUserTypesFilterAttribute(params UserType[] allowedUserTypes)
        {
            _allowedUserTypes = allowedUserTypes;
        }
        
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            
            if (!context.HttpContext.User.Identity.IsAuthenticated)
                context.Result = new ForbidResult();
            
            var databaseContext = context.HttpContext.RequestServices.GetRequiredService<OvertestDatabaseContext>();

            var currentUserId = new Guid(context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            var currentUserType = databaseContext.Users
                .AsNoTracking()
                .Where(u => u.Id == currentUserId)
                .Select(u => u.Type).First();
            
            if (!_allowedUserTypes.Contains(currentUserType))
                context.Result = new ForbidResult();
            
        }
        
    }
    
}