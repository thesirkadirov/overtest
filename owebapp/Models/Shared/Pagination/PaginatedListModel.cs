using System.Collections.Generic;

namespace Sirkadirov.Overtest.WebApplication.Models.Shared.Pagination
{
    
    public class PaginatedListModel<T>
    {
        
        public List<T> ItemsList { get; set; }
        public PaginationInfo Pagination { get; set; }
        
    }
    
}