using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sirkadirov.Overtest.WebApplication.Models.Shared.Pagination;

namespace Sirkadirov.Overtest.WebApplication.ViewComponents.Shared
{

    public class PaginationViewComponent : ViewComponent
    {
        
#pragma warning disable 1998
        public async Task<IViewComponentResult> InvokeAsync(PaginationInfo paginationInfo)
        {
            
            // Do not display pagination when there are no items
            if (paginationInfo.TotalPages <= 0)
                return Content("");
            
            return View("~/Views/Shared/ViewComponents/Shared/Pagination.cshtml", paginationInfo);

        }
#pragma warning restore 1998
        
    }
    
}