using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Sirkadirov.Overtest.WebApplication.Extensions.HtmlHelpers
{
    
    public static class RoutingHtmlHelpers
    {
        
        public static bool IsActiveLink(this IHtmlHelper htmlHelper, string areaName, string controllerName, string actionName)
        {
            return IsSameAreaName(htmlHelper, areaName) &&
                   IsSameControllerName(htmlHelper, controllerName) &&
                   IsSameActionName(htmlHelper, actionName);
        }

        public static bool IsSameAreaName(this IHtmlHelper htmlHelper, string area)
        {
            return IsSameRoutePart(htmlHelper, nameof(area), area);
        }

        public static bool IsSameControllerName(this IHtmlHelper htmlHelper, string controller)
        {
            return IsSameRoutePart(htmlHelper, nameof(controller), controller);
        }

        public static bool IsSameActionName(this IHtmlHelper htmlHelper, string action)
        {
            return IsSameRoutePart(htmlHelper, nameof(action), action);
        }
        
        private static bool IsSameRoutePart(IHtmlHelper htmlHelper, string routeDataType, string interestingValue)
        {
            var routeValue = htmlHelper.ViewContext.RouteData.Values[routeDataType];
            return (routeValue == null && string.IsNullOrWhiteSpace(interestingValue)) || (routeValue != null && routeValue.ToString() == interestingValue);
        }

        public static IHtmlContent EnsureRequestedClassAdded(this IHtmlHelper _, bool request, string className = "active")
        {
            return new HtmlString(request ? className : string.Empty);
        }
        
    }
    
}