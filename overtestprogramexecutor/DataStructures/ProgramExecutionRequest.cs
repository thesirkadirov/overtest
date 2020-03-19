using System.Collections.Generic;

namespace Sirkadirov.Overtest.Libraries.Shared.ProgramExecutor.DataStructures
{
    
    public class ProgramExecutionRequest
    {

        public class ProgramFeature
        {
            public bool Enabled { get; set; } = false;
        }
        
        public ProgramStartInformation StartInformation { get; set; }
        
        public class ProgramStartInformation
        {
            
            public string Path { get; set; }
            public string Arguments { get; set; }
            public string WorkingDirectory { get; set; }
            
            public KeyValuePair<string, string> Environment { get; set; }
            
        }
        
        public ProgramRunAsFeature RunAsFeature { get; set; }
        
        public class ProgramRunAsFeature : ProgramFeature
        {
            public string UserName { get; set; }
            public string UserPasswordClearText { get; set; }
            public string UserDomain { get; set; }
        }
        
        public ProgramRuntimeLimitsFeature RuntimeLimitsFeature { get; set; }
        
        public class ProgramRuntimeLimitsFeature : ProgramFeature
        {
            public RuntimeLimit<int> ProcessorTimeLimit { get; set; }
            public RuntimeLimit<int> RealTimeLimit { get; set; }
            
            public RuntimeLimit<long> MemoryUsageLimit { get; set; }
            public RuntimeLimit<long> DiskSpaceUsageLimit { get; set; }
            
            public RuntimeLimit<int> StdOutLengthLimit { get; set; }

            public class RuntimeLimit<T>
            {
                public bool Enabled { get; set; } = false;
                public T Limit { get; set; }
            }
        }
        
        public ProgramStandardStreamsRedirectionOptions StandardStreamsRedirectionOptions { get; set; }
        
        public class ProgramStandardStreamsRedirectionOptions
        {
            public InputDataSourceType InputDataSource { get; set; } = InputDataSourceType.None;
            public string InputSource { get; set; } = null;
            
            public enum InputDataSourceType
            {
                None,
                FromString,
                FromFile
            }

            public bool OutputRedirectionToFile { get; set; } = false;
            public string OutputRedirectionFileName { get; set; } = null;
        }

    }
    
}