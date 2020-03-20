using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NLog;
using Sirkadirov.Overtest.Libraries.Shared.Database;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.TestingApplications;
using Sirkadirov.Overtest.Libraries.Shared.Methods;

namespace Sirkadirov.Overtest.TestingDaemon
{
    
    public class Stuntman
    {

        private readonly IConfiguration _configuration;
        private readonly Logger _logger;
        
        
        public Stuntman(IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public void StartTheSight()
        {
            CreateSightParticlesAndWait();
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
                    
                    var dbContextOptionsBuilder = new DbContextOptionsBuilder().GetDbContextOptions(_configuration);

                    using (var databaseContext = new OvertestDatabaseContext(dbContextOptionsBuilder.Options))
                    {
                        
                        using (var transaction = await databaseContext.Database.BeginTransactionAsync())
                        {
                            
                            var testingApplication = await databaseContext.TestingApplications
                                .Where(a => a.Status == TestingApplication.ApplicationStatus.Waiting)
                                .FirstOrDefaultAsync();
                            
                            if (testingApplication != null)
                            {
                                
                                testingApplication.Status = TestingApplication.ApplicationStatus.Selected;
                                await databaseContext.SaveChangesAsync();
                                
                                await transaction.CommitAsync();
                                
                                // TODO: Call Waiter method
                                
                            }
                            else
                            {
                                await transaction.RollbackAsync();
                                await Task.Delay(_configuration.GetValue<int>("general:empty_delay"));
                            }
                            
                            await databaseContext.DisposeAsync();
                            
                        }
                        
                    }

                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
                    
            }
            
            // ReSharper disable once FunctionNeverReturns
            
        }
        
    }
    
}