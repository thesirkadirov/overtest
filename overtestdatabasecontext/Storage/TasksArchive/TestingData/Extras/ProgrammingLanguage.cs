using System;

namespace Sirkadirov.Overtest.Libraries.Shared.Database.Storage.TasksArchive.TestingData.Extras
{
    
    public class ProgrammingLanguage
    {
        
        public Guid Id { get; set; }
        
        public string DisplayName { get; set; }
        public string Description { get; set; }
        
        public string SyntaxHighlightingOptions { get; set; }
        
        public override bool Equals(object obj)
        {
            // ReSharper disable once PossibleNullReferenceException
            return Id == (obj as ProgrammingLanguage).Id;
        }
        
    }
    
}