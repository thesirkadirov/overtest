using System;
using System.ComponentModel.DataAnnotations;

namespace Sirkadirov.Overtest.Libraries.Shared.Database.Storage.TasksArchive.Extras
{
    
    public class ProgrammingLanguage
    {
        
        public Guid Id { get; set; }
        
        [Required, MinLength(1), MaxLength(100)]
        public string DisplayName { get; set; }
        
        [MinLength(0), MaxLength(500)]
        public string Description { get; set; }
        
        [Required]
        public string SyntaxHighlightingOptions { get; set; }
        
    }
    
}