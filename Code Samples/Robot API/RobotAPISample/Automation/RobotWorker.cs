using Newtonsoft.Json;
using RobotAPISample.Automation.UiPath;
using RobotAPISample.RequestsResponses;
using RobotAPISample.Workflows;
using System;
using System.Threading;

namespace RobotAPISample.Automation
{
    public interface IRobotWorker
    {
        void RequestStop();

        void SetResult(WorkflowResponse response);
    }

    public class RobotWorker<TRequest, TResponse> : IRobotWorker, IDisposable
        where TRequest : WorkflowRequest
        where TResponse : WorkflowResponse, new()
    {
        private static string _user = @"INFOTRACK\amber.weightman";

        private static int _syncExecutionTimeoutMins = 2;

        //private string _robotWorkflowPath { get; set; }
        public TRequest InputArguments { get; set; }
        public TResponse Result { get; private set; }
        public bool IsActive { get; set; }

        private Guid? _jobGuid = null;

        private readonly WorkflowType _workflowType;
        
        internal WorkflowResponse ExecuteRobotJobSynchronously<TWorkflow>(TWorkflow robotWorkflowBase, int? timeoutMins = null)
            where TWorkflow : RobotWorkflowBase<TRequest, TResponse>
        {
            var workerThread = new Thread(() => ExecuteRobotJob(robotWorkflowBase.WorkflowFile));
            workerThread.Start();

            // Loop until worker thread activates. 
            while (!workerThread.IsAlive) ;

            var mins = timeoutMins != null && timeoutMins.HasValue ? timeoutMins.Value : _syncExecutionTimeoutMins;
            workerThread.Join(TimeSpan.FromMinutes(mins));

            if (IsActive) // if the worker is still active, waiting has timed out, so stop it
            {
                RequestStop();
                Result = new TResponse();
                Result.Error = new ApplicationException($"Workflow process {robotWorkflowBase.WorkflowType} timed out");
            }

            return Result;
        }
        
        // Volatile is used as hint to the compiler that this data 
        // member will be accessed by multiple threads. 
        private volatile bool _shouldStop;

        public RobotWorker(TRequest inputArguments, WorkflowType workflowType)
        {
            InputArguments = inputArguments;
            _workflowType = workflowType;
        }
        
        public void ExecuteRobotJob(string robotWorkflowPath)
        {
            IsActive = true;
            var resp = ExecuteRobotJobAsync(InputArguments, robotWorkflowPath);

            while (!_shouldStop)
            {
                // TODO timeout? (Although the parent thread does have a timeout after which it will tell this thread to terminate)
            }
            //Console.WriteLine("worker thread: terminating gracefully.");
            IsActive = false;
        }

        public void RequestStop()
        {
            _shouldStop = true;
        }

        public void SetResult(WorkflowResponse response)
        {
            Result = WorkflowResponseFactory.Create(_workflowType, response) as TResponse;
        }

        private Guid ExecuteRobotJobAsync(TRequest inputArguments, string robotWorkflowPath)
        {
            var job = new RobotClientJob
            {
                WorkflowFile = robotWorkflowPath,
                InputArguments = inputArguments,
                User = _user,
                Type = 0
            };
            var serialisedJob = JsonConvert.SerializeObject(job);
            //Console.WriteLine(serialisedJob);
            _jobGuid = new RobotClient().StartJob(serialisedJob);
            
            Threading.RobotWorkflowThreads.Add(_jobGuid.Value, this);
            return _jobGuid.Value;
        }

        public class RobotClientJob
        {
            public string WorkflowFile { get; set; }
            public TRequest InputArguments { get; set; }
            public TResponse OutputArguments { get; set; }
            public string User { get; set; }
            public int Type { get; set; }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (_jobGuid != null && _jobGuid.HasValue && Threading.RobotWorkflowThreads.ContainsKey(_jobGuid.Value))
                    {
                        if (Threading.RobotWorkflowThreads.ContainsKey(_jobGuid.Value))
                        {
                            Threading.RobotWorkflowThreads.Remove(_jobGuid.Value);
                        }
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                
                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~RobotWorker() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }


}
