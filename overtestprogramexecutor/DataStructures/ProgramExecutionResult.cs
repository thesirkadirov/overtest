namespace Sirkadirov.Overtest.Libraries.Shared.ProgramExecutor.DataStructures
{
    
    public class ProgramExecutionResult
    {
        
        public ProgramRuntimeResourcesUsage RuntimeResourcesUsage { get; set; }
        
        public class ProgramRuntimeResourcesUsage
        {
            public int ProcessorTime { get; set; }
            public int RealExecutionTime { get; set; }
            
            public long PeakMemoryUsage { get; set; }
            public long DiskSpaceUsage { get; set; }
        }
        
        public int ProcessExitCode { get; set; }
        
        public string StandardOutputData { get; set; }
        public string StandardErrorData { get; set; }
        
    }
    
}