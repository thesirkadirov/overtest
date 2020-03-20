using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NLog;
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

            ConfigureLogging();
            
            return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
            {
                
                webBuilder.UseKestrel(options =>
                {
                    options.AddServerHeader = true;
                    options.ConfigureEndpointDefaults(endpointOptions => { endpointOptions.Protocols = HttpProtocols.Http2; });
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