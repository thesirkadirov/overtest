using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Sirkadirov.Overtest.Libraries.Shared.ProgramExecutor.DataStructures;

namespace Sirkadirov.Overtest.Libraries.Shared.ProgramExecutor
{
    
    public class ProgramExecutor : IDisposable
    {
        
        // ReSharper disable once MemberCanBePrivate.Global
        public ProgramExecutionRequest ExecutionRequest { get; }
        // ReSharper disable once MemberCanBePrivate.Global
        public ProgramExecutionResult ExecutionResult { get; private set; }
        
        private Process _process;
        
        public ProgramExecutor(ProgramExecutionRequest executionRequest)
        {
            
            ExecutionRequest = executionRequest;
            
            ConfigureProcess();
            InitializeExecutionResult();
            
            void ConfigureProcess()
            {
                
                _process = new Process
                {
                    EnableRaisingEvents = true,
                    PriorityClass = ProcessPriorityClass.BelowNormal,
                    StartInfo = new ProcessStartInfo
                    {
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                    
                        StandardInputEncoding = Encoding.UTF8,
                        StandardOutputEncoding = Encoding.UTF8,
                        StandardErrorEncoding = Encoding.UTF8,
                    
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        UseShellExecute = false,
                        ErrorDialog = false,

                        FileName = ExecutionRequest.StartInformation.Path,
                        WorkingDirectory = ExecutionRequest.StartInformation.WorkingDirectory,
                        Arguments = ExecutionRequest.StartInformation.Arguments
                    }
                };
                
                // Add custom environment variables
                _process.StartInfo.Environment.Add(ExecutionRequest.StartInformation.Environment);
                
                if (ExecutionRequest.RunAsFeature.Enabled)
                {

                    _process.StartInfo.UserName = ExecutionRequest.RunAsFeature.UserName;
                    
                    /*
                     * To workaround not implemented
                     * features in current .NET Core
                     * release.
                     */
                    
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        
                        // No implementation in other systems
                        _process.StartInfo.PasswordInClearText = ExecutionRequest.RunAsFeature.UserPasswordClearText;
                        
                        // Windows-only feature
                        _process.StartInfo.Domain = ExecutionRequest.RunAsFeature.UserDomain;
                        
                        _process.StartInfo.LoadUserProfile = false;
                        
                    }
                    else
                    {
                        /*
                         * Find a way to run as another user with
                         * password on other operation systems.
                         */
                    }
                
                }
                
            }

            void InitializeExecutionResult()
            {
                ExecutionResult = new ProgramExecutionResult
                {
                    RuntimeResourcesUsage = new ProgramExecutionResult.ProgramRuntimeResourcesUsage
                    {
                        DiskSpaceUsage = 0,
                        PeakMemoryUsage = 0,
                        ProcessorTime = 0,
                        RealExecutionTime = 0
                    }
                };
            }
            
        }

        // ReSharper disable once UnusedMember.Global
        public async Task<ProgramExecutionResult> Execute()
        {

            try
            {
                await WriteInput();
            }
            catch (Exception ex)
            {
                throw new StreamRedirectionExecutionException(ExecutionResult, ex);
            }

            try
            {
                _process.Start();
            }
            catch (Exception ex)
            {
                throw new ExecutionException("Method execution failed!", ex, ExecutionResult);
            }

            ExecuteProcessResourcesUsageWatcher().Start();

            if (ExecutionRequest.RuntimeLimitsFeature.RealTimeLimit.Enabled)
            {
                
                /*
                 * When we use [ bool Process.WaitForExit(int milliseconds) ]
                 * we need to call [ void Process.WaitForExit() ] afterwards
                 * to work around the "Intermittent: Empty Process stdout" issue.
                 * More information: https://github.com/dotnet/runtime/issues/27128
                 */
                
                if (!_process.WaitForExit(ExecutionRequest.RuntimeLimitsFeature.RealTimeLimit.Limit))
                {
                    await StopExecutionWithException(
                        new RealTimeLimitReachedExecutionException(ExecutionResult, $"Program {ExecutionRequest.StartInformation.Path} reached real time limit!")
                    );
                }
                
                _process.WaitForExit();
                
            }
            else
            {
                _process.WaitForExit();
            }
            
            // Get process information, output and resources usage
            await FinalizeExecution();
            
            // Return execution result to the caller
            return ExecutionResult;

            async Task WriteInput()
            {
                
                var inputDataSource = ExecutionRequest.StandardStreamsRedirectionOptions.InputDataSource;
                
                if (inputDataSource != ProgramExecutionRequest.ProgramStandardStreamsRedirectionOptions.InputDataSourceType.None)
                {

                    if (inputDataSource == ProgramExecutionRequest.ProgramStandardStreamsRedirectionOptions.InputDataSourceType.FromString)
                    {
                        await _process.StandardInput.WriteAsync(ExecutionRequest.StandardStreamsRedirectionOptions.InputSource);
                    }
                    else if (inputDataSource == ProgramExecutionRequest.ProgramStandardStreamsRedirectionOptions.InputDataSourceType.FromFile)
                    {
                        
                        var inputDataFile = File.OpenText(ExecutionRequest.StandardStreamsRedirectionOptions.InputSource);
                        
                        while (!inputDataFile.EndOfStream)
                        {
                            await _process.StandardInput.WriteLineAsync(await inputDataFile.ReadLineAsync());
                        }
                        
                        inputDataFile.Close();
                        inputDataFile.Dispose();
                        
                    }
                    
                }
                
            }
            
        }

        private void UpdateProcessResourcesUsage()
        {
            
            /*
             * Need to find possible workarounds to do disk space usage calculations faster.
             * 
             * var diskSpaceUsage = new DirectoryInfo(ExecutionRequest.StartInformation.WorkingDirectory)
             *    .GetFiles("*", SearchOption.AllDirectories).Sum(f => f.Length);
             *
             * ExecutionResult.RuntimeResourcesUsage.DiskSpaceUsage = diskSpaceUsage;
             */
            ExecutionResult.RuntimeResourcesUsage.PeakMemoryUsage = _process.PeakWorkingSet64;
            ExecutionResult.RuntimeResourcesUsage.ProcessorTime = Convert.ToInt32(Math.Round(_process.TotalProcessorTime.TotalMilliseconds));
            ExecutionResult.RuntimeResourcesUsage.RealExecutionTime = Convert.ToInt32(Math.Round((_process.ExitTime - _process.StartTime).TotalMilliseconds));
            
        }

        private async Task ExecuteProcessResourcesUsageWatcher()
        {
            try
            {

                while (!_process.HasExited)
                {
                    
                    _process.Refresh();
                    
                    UpdateProcessResourcesUsage();
                    
                    if (ExecutionRequest.RuntimeLimitsFeature.ProcessorTimeLimit.Enabled)
                    {
                        if (ExecutionRequest.RuntimeLimitsFeature.ProcessorTimeLimit.Limit < ExecutionResult.RuntimeResourcesUsage.ProcessorTime)
                        {
                            await StopExecutionWithException(
                                new ProcessorTimeLimitReachedExecutionException(
                                    ExecutionResult, 
                                    $"Program ${ExecutionRequest.StartInformation.Path} exceeded processor time limit!"
                                )
                            );
                        }
                    }
                    
                    if (ExecutionRequest.RuntimeLimitsFeature.MemoryUsageLimit.Enabled)
                    {
                        if (ExecutionRequest.RuntimeLimitsFeature.MemoryUsageLimit.Limit < ExecutionResult.RuntimeResourcesUsage.PeakMemoryUsage)
                        {
                            await StopExecutionWithException(
                                new ProcessorTimeLimitReachedExecutionException(
                                    ExecutionResult, 
                                    $"Program ${ExecutionRequest.StartInformation.Path} exceeded processor time limit!"
                                )
                            );
                        }
                    }
                    
                }
                
            }
            catch (ExecutionException)
            {
                throw;
            }
            catch
            {
                /*  */
            }
        }
        
        private async Task FinalizeExecution(bool killProcess = false)
        {
            
            /*
             * Kill the process and get exit code
             */
            
            if (killProcess && !_process.HasExited)
            {
                _process.Kill(true);
                _process.WaitForExit();
            }
            
            UpdateProcessResourcesUsage();

            ExecutionResult.ProcessExitCode = _process.ExitCode;
            
            /*
             * Save output data
             */
            
            ExecutionResult.StandardOutputData = _process.StandardOutput.ReadToEnd();
            ExecutionResult.StandardErrorData = _process.StandardError.ReadToEnd();

            if (ExecutionRequest.StandardStreamsRedirectionOptions.OutputRedirectionToFile)
            {
                await File.WriteAllTextAsync(
                    Path.Combine(
                        ExecutionRequest.StartInformation.WorkingDirectory,
                        ExecutionRequest.StandardStreamsRedirectionOptions.OutputRedirectionFileName
                    ),
                    ExecutionResult.StandardOutputData,
                    Encoding.UTF8
                );
            }
            
        }

        private async Task StopExecutionWithException(ExecutionException executionException)
        {
            
            // Kill the process and finalize execution
            await FinalizeExecution(true);
            
            /*
             * To work around potentially dangerous situation
             * when [ executionException ] is [ null ], but
             * [ _process ] is still running.
             */
            if (executionException == null)
                throw new ArgumentNullException(nameof(executionException));
            
            // Throw an exception
            throw executionException;
            
        }

        public void Dispose()
        {
            _process?.Close();
            _process?.Dispose();
        }
        
    }
    
}