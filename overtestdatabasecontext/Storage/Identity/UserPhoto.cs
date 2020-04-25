using System;
using System.ComponentModel.DataAnnotations;

namespace Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Identity
{
    
    public class UserPhoto
    {
        
        public Guid Id { get; set; }
        
        public User User { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PublicationDate { get; set; }
        
        public byte[] Source { get; set; }
        
    }
    
}