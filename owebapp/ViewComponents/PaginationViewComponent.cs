using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sirkadirov.Overtest.WebApplication.Models.Shared.Pagination;
#pragma warning disable 1998

namespace Sirkadirov.Overtest.WebApplication.ViewComponents
{
    public class PaginationViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(PaginationInfo paginationInfo)
        {
            // Do not display pagination when there are no items
            if (paginationInfo.TotalPages <= 0)
                return Content("");
            
            return View("~/ViewComponents/Views/Pagination.cshtml", paginationInfo);
        }
    }
}