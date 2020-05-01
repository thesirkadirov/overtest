using System;
using System.Collections.Generic;

namespace Sirkadirov.Overtest.WebApplication.Models.Shared
{
    
    public class PaginatedListModel<T>
    {
        
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }

        public int PreviousItemsCount => (CurrentPage - 1) * ItemsPerPage;
        public int FirstItemId => PreviousItemsCount + 1;
        public int TotalPages => (int) Math.Ceiling(TotalItems / (double) ItemsPerPage);
        
        public List<T> ItemsList { get; set; }
        
    }
    
}