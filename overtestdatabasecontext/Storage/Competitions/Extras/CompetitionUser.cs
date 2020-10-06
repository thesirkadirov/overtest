using System;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Identity;

namespace Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Competitions.Extras
{
    
    public class CompetitionUser
    {
        
        public Competition Competition { get; set; }
        public Guid CompetitionId { get; set; }
        
        public User User { get; set; }
        public Guid UserId { get; set; }
        
    }
    
}