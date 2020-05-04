using System;

namespace Sirkadirov.Overtest.WebApplication.Models.Shared.Pagination
{
    
    public class PaginationInfo
    {
            
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }

        public int PreviousItemsCount => (CurrentPage - 1) * ItemsPerPage;
        public int FirstItemId => PreviousItemsCount + 1;
        public int TotalPages => (int) Math.Ceiling(TotalItems / (double) ItemsPerPage);

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
            
    }
    
}