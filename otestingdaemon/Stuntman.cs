using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sirkadirov.Overtest.Libraries.Shared.Database;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.TestingApplications;
using Sirkadirov.Overtest.Libraries.Shared.Methods;

namespace Sirkadirov.Overtest.TestingDaemon
{
    
    public class Stuntman
    {

        private readonly IConfiguration _configuration;
        
        
        public Stuntman(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task StartTheSight()
        {

            await CreateSightParticlesAndWait();
            
        }
        
        private async Task CreateSightParticlesAndWait()
        {
            var tasks = new List<Task>();
            
            for (var i = 0; i < _configuration.GetValue<int>("general:parallelism"); i++)
            {
                
                var particle = SightParticle();
                particle.Start();
                
                tasks.Add(particle);
                
            }
            
            await Task.WhenAll(tasks);
            
            async Task SightParticle()
            {
                while (true)
                {
                    
                    var dbContextOptionsBuilder = new DbContextOptionsBuilder().GetDbContextOptions(_configuration);
                    
                    await using var databaseContext = new OvertestDatabaseContext(dbContextOptionsBuilder.Options);
                    await using var transaction = await databaseContext.Database.BeginTransactionAsync();
                    
                    try
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
                        
                    }
                    catch (Exception)
                    {
                        /* TODO! */
                    }
                    
                    await databaseContext.DisposeAsync();
                    
                }
                
                // ReSharper disable once FunctionNeverReturns
                
            }
            
        }
        
    }
    
}