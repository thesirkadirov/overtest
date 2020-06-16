using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using Sirkadirov.Overtest.Libraries.Shared.Methods;

namespace Sirkadirov.Overtest.WebApplication
{
    
    public static class Program
    {
        
        private const string ConfigurationFileName = "owebapplication.config.json";
        private const string DefaultLoggerName = "OVERTEST_FATAL";
        
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(ConfigurationFileName)
                .Build();
            
            // Configure logging methods
            ConfigureLogging();
            
            return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
            {
                
                webBuilder.UseKestrel(options =>
                {
                    options.AddServerHeader = true;
                    options.ConfigureEndpointDefaults(endpointOptions => { endpointOptions.Protocols = HttpProtocols.Http1AndHttp2; });
                });
                
                webBuilder.UseConfiguration(configuration);
                webBuilder.UseStartup<Startup>();
                
                webBuilder.UseUrls(configuration.GetSection("server:endpoints").Get<string[]>());

                webBuilder.ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                }).UseNLog();
                
                webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddConfiguration(configuration);
                });

            });

            void ConfigureLogging()
            {
                
                LogManager.Configuration = configuration.ConfigureLogging();

                AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
                {
                    LogManager.GetLogger(DefaultLoggerName).Fatal(eventArgs.ExceptionObject);
                };

            }
            
        }
        
    }
    
}