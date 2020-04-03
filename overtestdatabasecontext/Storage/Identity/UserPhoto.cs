using System;

namespace Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Identity
{
    
    public class UserPhoto
    {
        
        public Guid Id { get; set; }
        
        public User User { get; set; }
        
        public byte[] Source { get; set; }
        
    }
    
}