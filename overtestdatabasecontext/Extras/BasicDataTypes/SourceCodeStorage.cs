using System;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.TasksArchive.TestingData.Extras;

namespace Sirkadirov.Overtest.Libraries.Shared.Database.Extras.BasicDataTypes
{
    
    public abstract class SourceCodeStorage
    {
        
        public byte[] SourceCode { get; set; }
        
        public ProgrammingLanguage ProgrammingLanguage { get; set; }
        public Guid ProgrammingLanguageId { get; set; }
        
    }
    
}