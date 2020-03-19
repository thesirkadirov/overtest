using System;

namespace Sirkadirov.Overtest.Libraries.Shared.ProgramExecutor.DataStructures
{

    public class ExecutionException : Exception
    {
        public ExecutionException(ProgramExecutionResult executionResult, string message) : base(message)
        {
            ExecutionResult = executionResult;
        }

        public ExecutionException(string message, Exception innerException, ProgramExecutionResult executionResult = null) : base(message, innerException)
        {
            ExecutionResult = executionResult;
        }
        
        public ProgramExecutionResult ExecutionResult { get; set; }
    }
    
    #region Runtime limits
    
    public class ProcessorTimeLimitReachedExecutionException : ExecutionException
    {
        public ProcessorTimeLimitReachedExecutionException(ProgramExecutionResult executionResult, string message) : base(executionResult, message) {  }
    }
    
    public class RealTimeLimitReachedExecutionException : ExecutionException
    {
        public RealTimeLimitReachedExecutionException(ProgramExecutionResult executionResult, string message) : base(executionResult, message) {  }
    }
    
    public class PeakMemoryUsageLimitReachedExecutionException : ExecutionException
    {
        public PeakMemoryUsageLimitReachedExecutionException(ProgramExecutionResult executionResult, string message) : base(executionResult, message) {  }
    }
    
    public class PeakDiskSpaceUsageLimitReachedExecutionException : ExecutionException
    {
        public PeakDiskSpaceUsageLimitReachedExecutionException(ProgramExecutionResult executionResult, string message) : base(executionResult, message) {  }
    }
    
    public class StdOutLengthLimitReachedExecutionException : ExecutionException
    {
        public StdOutLengthLimitReachedExecutionException(ProgramExecutionResult executionResult, string message) : base(executionResult, message) {  }
    }
    
    #endregion
    
    #region RuntimeExceptions
    
    public class ProgramExecutionException : ExecutionException
    {
        public ProgramExecutionException(ProgramExecutionResult executionResult, Exception innerException) : base("An exception catched during program execution!", innerException)
        {
            ExecutionResult = executionResult;
        }
    }
    
    public class StreamRedirectionExecutionException : ExecutionException
    {
        public StreamRedirectionExecutionException(ProgramExecutionResult executionResult, Exception innerException) : base("One of stream redirection methods has failed!", innerException)
        {
            ExecutionResult = executionResult;
        }
    }
    
    #endregion
    
}