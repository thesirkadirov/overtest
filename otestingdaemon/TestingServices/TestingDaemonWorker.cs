using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.TestingApplications;
using Sirkadirov.Overtest.Libraries.Shared.Methods;
using Sirkadirov.Overtest.TestingDaemon.Services.Storage;
using Sirkadirov.Overtest.TestingDaemon.TestingServices.Skeleton;
using ILogger = NLog.ILogger;

namespace Sirkadirov.Overtest.TestingDaemon.TestingServices
{
    
    public class TestingDaemonWorker : IExecutable<Task>
    {

        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly ILoggerProvider _loggerProvider;
        
        public TestingDaemonWorker(IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = LogManager.GetCurrentClassLogger();
            _loggerProvider = new NLogLoggerProvider();
        }

        public async Task Execute()
        {
            
            await Initialize();
            
            CreateSightParticlesAndWait();
            
        }

        private async Task Initialize()
        {
            
            await using var databaseContext = _configuration.GetDbContext(_loggerProvider);
            
            // Initialize the database
            await databaseContext.InitializeDatabaseAsync();

            await new TestingDaemonStorageProvider(_configuration, databaseContext).ActualizeTestingData();
            
        }
        
        private void CreateSightParticlesAndWait()
        {
            
            var tasks = new List<Task>();
            
            for (var i = 0; i < _configuration.GetValue<int>("general:parallelism"); i++)
            {

                var task = new Task(() => ExecuteSightParticle().Wait());
                
                task.Start();
                tasks.Add(task);
                
            }

            Task.WaitAll(tasks.ToArray());

        }
        
        private async Task ExecuteSightParticle()
        {
            
            while (true)
            {
                        
                try
                {
                    
                    // Using database context and transaction
                    await using var databaseContext = _configuration.GetDbContext(_loggerProvider);
                    await using var transaction = await databaseContext.Database.BeginTransactionAsync();
                    
                    // Get a list of IDs of compilers, supported by tis OTestingDaemon instance
                    var supportedCompilers = _configuration
                        .GetSection("compilers")
                        .GetChildren()
                        .Select(c => c.GetValue<string>("id"))
                        .ToList();
                    
                    // Try to get single testing application with waiting status
                    var testingApplication = await databaseContext.TestingApplications
                        .Where(a => 
                            a.Status == TestingApplication.ApplicationStatus.Waiting
                            && supportedCompilers.Contains(a.SourceCode.ProgrammingLanguageId.ToString())
                        )
                        .OrderBy(a => a.Created)
                        .FirstOrDefaultAsync();
                    
                    if (testingApplication != null)
                    {
                        
                        // Change application status to "selected"
                        testingApplication.Status = TestingApplication.ApplicationStatus.Selected;
                        await databaseContext.SaveChangesAsync();
                        await transaction.CommitAsync();
                        
                        // Initialize and execute testing worker
                        await new ApplicationTestingWorker(_configuration, databaseContext, testingApplication).Execute();

                    }
                    else
                    {
                        await transaction.RollbackAsync();
                        await Task.Delay(_configuration.GetValue<int>("general:empty_delay"));
                    }
                    
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"Got an exception of type {ex.GetType()} during TestingApplication processing in {nameof(ExecuteSightParticle)} method!");
                }
                    
            }
            
            // ReSharper disable once FunctionNeverReturns
            
        }
        
    }
    
}