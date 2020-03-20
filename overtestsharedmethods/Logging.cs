using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Sirkadirov.Overtest.Libraries.Shared.Methods
{
    
    public static class Logging
    {
        
        public static LoggingConfiguration ConfigureLogging(this IConfiguration configuration)
        {
            
            var loggingConfiguration = new LoggingConfiguration();
            IConfiguration userLoggingConfiguration = configuration.GetSection("diagnostics:logging");

            if (!userLoggingConfiguration.GetValue<bool>("enabled")) return loggingConfiguration;
            
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
            
                loggingConfiguration.AddRule(
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
                                    
                loggingConfiguration.AddRule(
                    LogLevel.FromString(filesTarget["levels:minimum"]),
                    LogLevel.FromString(filesTarget["levels:maximum"]),
                    nLogFileTarget
                );
            
            }
            
            return loggingConfiguration;

        }
        
    }
    
}