using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NLog;
using Sirkadirov.Overtest.Libraries.Shared.Methods;
using Sirkadirov.Overtest.TestingDaemon.TestingServices;
using Sirkadirov.Overtest.TestingDaemon.TestingServices.Skeleton;

namespace Sirkadirov.Overtest.TestingDaemon
{
    
    internal static class Program
    {
        
        private const string ConfigurationFileName = "otestingdaemon.config.json";
        private const string DefaultLoggerName = "OVERTEST_FATAL";
        
        public static async Task Main()
        {
            
            // Get program configuration from file
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile(ConfigurationFileName).Build();
            
            // Configure application logging
            ConfigureLogging(configuration);
            
            // Create and execute daemon worker
            await new TestingDaemonWorker(configuration).Execute();

        }
        
        private static void ConfigureLogging(IConfiguration configuration)
        {
                
            LogManager.Configuration = configuration.ConfigureLogging();
            
            // Handle critical exceptions
            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
            {
                LogManager.GetLogger(DefaultLoggerName).Fatal(eventArgs.ExceptionObject);
                Environment.Exit(1);
            };

        }
        
    }
    
}