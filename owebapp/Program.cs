using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Sirkadirov.Overtest.WebApplication
{
    
    public static class Program
    {
        
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("owebapp_config.json")
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
                
                var defaultLoggingConfiguration = new LoggingConfiguration();
                var userLoggingConfiguration = configuration.GetSection("diagnostics:logging");

                if (userLoggingConfiguration.GetValue<bool>("enabled"))
                {

                    var userLoggingTargets = userLoggingConfiguration.GetSection("targets");

                    ConfigureConsoleLoggingTarget();
                    ConfigureFilesLoggingTarget();

                    void ConfigureConsoleLoggingTarget()
                    {

                        var consoleTarget = userLoggingTargets.GetSection("console");

                        if (!consoleTarget.GetValue<bool>("enabled"))
                            return;

                        var nLogColoredConsoleTarget = new ColoredConsoleTarget("console")
                        {
                            Encoding = Encoding.UTF8,
                            DetectConsoleAvailable = true,
                            DetectOutputRedirected = true
                        };

                        defaultLoggingConfiguration.AddRule(
                            LogLevel.FromString(consoleTarget["levels:minimum"]),
                            LogLevel.FromString(consoleTarget["levels:maximum"]),
                            nLogColoredConsoleTarget
                        );

                    }

                    void ConfigureFilesLoggingTarget()
                    {

                        var filesTarget = userLoggingTargets.GetSection("files");
                        
                        if (!filesTarget.GetValue<bool>("enabled"))
                            return;

                        var nLogFileTarget = new FileTarget
                        {
                            FileName = Path.Combine(filesTarget["path"], "${shortdate}.log"),
                            CreateDirs = true,
                            
                            KeepFileOpen = true,
                            Encoding = Encoding.UTF8,
                            WriteBom = false,
                            LineEnding = LineEndingMode.CRLF,

                            ArchiveEvery = FileArchivePeriod.Day,
                            ArchiveNumbering = ArchiveNumberingMode.DateAndSequence
                        };
                        
                        defaultLoggingConfiguration.AddRule(
                            LogLevel.FromString(filesTarget["levels:minimum"]),
                            LogLevel.FromString(filesTarget["levels:maximum"]),
                            nLogFileTarget
                        );

                    }
                    
                    /*
                     * Save information about all unhandled exceptions
                     */
                    
                    AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
                    {

                        // Pass an exception object to global logger
                        LogManager.GetLogger("OWEBAPP.GLOBAL").Fatal(e.ExceptionObject);

                        // Exit application
                        Environment.Exit(1);

                    };
                    
                }

                LogManager.Configuration = defaultLoggingConfiguration;

            }
            
        }
        
    }
    
}