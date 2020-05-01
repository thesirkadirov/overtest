using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sirkadirov.Overtest.Libraries.Shared.Database;

namespace Sirkadirov.Overtest.Libraries.Shared.Methods
{
    
    public static class DatabaseSharedMethods
    {
        
        public static OvertestDatabaseContext GetDbContext(this IConfiguration _, ILoggerProvider loggerProvider)
        {
            var databaseContext = new OvertestDatabaseContext();
            databaseContext.GetService<ILoggerFactory>().AddProvider(loggerProvider);
            return databaseContext;
        }
        
    }
    
}