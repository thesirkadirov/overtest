using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Sirkadirov.Overtest.Libraries.Shared.Database;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.TestingApplications;
using Sirkadirov.Overtest.TestingDaemon.TestingServices.Skeleton;

namespace Sirkadirov.Overtest.TestingDaemon.TestingServices
{
    
    public class ApplicationTestingWorker : TestingServiceWorker, IExecutable<Task>
    {
        
        public ApplicationTestingWorker(IConfiguration configuration, OvertestDatabaseContext databaseContext, TestingApplication testingApplication) : base(configuration, databaseContext, testingApplication)
        {
        }

        public async Task Execute()
        {
            
            // TODO: Verify cache
            
            // Prepare application data and files
            await PrepareApplication();
            
            // TODO: Execute compilation stage
            // TODO: Execute testing methods
            
            // Upload results to the database
            await PrepareAndUploadResults();
            
        }

        private async Task PrepareApplication()
        {
            throw new NotImplementedException();
        }
        
        private async Task PrepareAndUploadResults()
        {
            DatabaseContext.TestingApplications.Update(TestingApplication);
            await DatabaseContext.SaveChangesAsync();
        }
        
    }
    
}