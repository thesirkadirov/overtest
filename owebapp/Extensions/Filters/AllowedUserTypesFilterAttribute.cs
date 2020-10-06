using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Sirkadirov.Overtest.Libraries.Shared.Database;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Identity;

namespace Sirkadirov.Overtest.WebApplication.Extensions.Filters
{
    public class AllowedUserTypesFilterAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly UserType[] _allowedUserTypes;
        
        public AllowedUserTypesFilterAttribute(params UserType[] allowedUserTypes)
        {
            _allowedUserTypes = allowedUserTypes;
        }
        
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
                context.Result = new ForbidResult();
            
            var databaseContext = context.HttpContext.RequestServices.GetRequiredService<OvertestDatabaseContext>();
            var currentUserId = new Guid(context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            if (!await databaseContext.UserPermissionsOperator.GetUserHasSpecifiedTypeAsync(currentUserId, _allowedUserTypes))
                context.Result = new ForbidResult();
        }
    }
}