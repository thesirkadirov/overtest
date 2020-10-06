using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Sirkadirov.Overtest.WebApplication.Extensions.Filters
{
    
    public class DisallowAuthorizedFilterAttribute : Attribute, IAuthorizationFilter
    {
        
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
                context.Result = new ForbidResult();
        }
        
    }
    
}