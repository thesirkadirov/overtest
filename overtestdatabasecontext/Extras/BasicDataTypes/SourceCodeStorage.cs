using System;
using System.ComponentModel.DataAnnotations;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.TasksArchive.Extras;

namespace Sirkadirov.Overtest.Libraries.Shared.Database.Extras.BasicDataTypes
{
    public abstract class SourceCodeStorage
    {
        [Required]
        [DataType(DataType.Text)]
        public byte[] SourceCode { get; set; }
        
        [Required]
        public Guid ProgrammingLanguageId { get; set; }
        
        public ProgrammingLanguage ProgrammingLanguage { get; set; }
    }
}