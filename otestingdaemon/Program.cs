using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NLog;
using Sirkadirov.Overtest.Libraries.Shared.Methods;

namespace Sirkadirov.Overtest.TestingDaemon
{
    
    internal static class Program
    {
        
        private const string ConfigurationFileName = "otestingdaemon.config.json";
        private const string DefaultLoggerName = "OVERTEST_FATAL";
        
        public static async Task Main(string[] args)
        {

            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile(ConfigurationFileName).Build();
            
            ConfigureLogging(configuration);
            
            await new Stuntman(configuration).StartTheSight();

        }
        
        private static void ConfigureLogging(IConfiguration configuration)
        {
                
            LogManager.Configuration = configuration.ConfigureLogging();

            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
            {
                LogManager.GetLogger(DefaultLoggerName).Fatal(eventArgs.ExceptionObject);
                Environment.Exit(1);
            };

        }
        
    }
    
}