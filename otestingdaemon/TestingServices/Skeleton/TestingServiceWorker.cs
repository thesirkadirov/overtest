using Microsoft.Extensions.Configuration;
using Sirkadirov.Overtest.Libraries.Shared.Database;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.TestingApplications;

namespace Sirkadirov.Overtest.TestingDaemon.TestingServices.Skeleton
{
    
    public abstract class TestingServiceWorker
    {
        
        protected readonly IConfiguration Configuration;
        protected readonly OvertestDatabaseContext DatabaseContext;
        protected readonly TestingApplication TestingApplication;
        
        protected TestingServiceWorker(IConfiguration configuration, OvertestDatabaseContext databaseContext, TestingApplication testingApplication)
        {
            Configuration = configuration;
            DatabaseContext = databaseContext;
            TestingApplication = testingApplication;
        }
        
    }
    
}