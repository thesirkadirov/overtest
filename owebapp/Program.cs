using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Extensions.Logging;
using Sirkadirov.Overtest.Libraries.Shared.Methods;

namespace Sirkadirov.Overtest.WebApplication
{
    
    public static class Program
    {
        
        private const string ConfigurationFileName = "owebapplication.config.json";
        private const string DefaultLoggerName = "OVERTEST_FATAL";
        
        public static async Task Main(string[] args)
        {
            (await CreateHostBuilder(args)).Build().Run();
        }

        private static async Task<IHostBuilder> CreateHostBuilder(string[] args)
        {
            
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(ConfigurationFileName)
                .Build();
            
            // Configure logging methods
            ConfigureLogging();
            
            // Initialize the database
            await configuration.GetDbContext(new NLogLoggerProvider()).InitializeDatabaseAsync();
            
            return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
            {
                
                webBuilder.UseKestrel(options =>
                {
                    options.AddServerHeader = true;
                    options.ConfigureEndpointDefaults(endpointOptions => { endpointOptions.Protocols = HttpProtocols.Http1AndHttp2; });
                });
                webBuilder.UseIISIntegration();
                
                webBuilder.UseConfiguration(configuration);
                webBuilder.UseStartup<Startup>();
                
                webBuilder.UseUrls(configuration.GetSection("server:endpoints").Get<string[]>());
                
            });

            void ConfigureLogging()
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
    
}