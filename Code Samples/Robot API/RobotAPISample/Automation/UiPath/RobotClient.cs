using Newtonsoft.Json;
using RobotAPISample.RequestsResponses;
using System;
using System.IO;
using System.ServiceModel;

namespace RobotAPISample.Automation.UiPath
{
    /// <summary>
    /// Wrapper around the UIPath Robot Service
    /// </summary>
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class RobotClient : UiPathREST.IUiPathRemoteDuplexContractCallback
    {
        // UiPath Remote Channel
        protected UiPathREST.IUiPathRemoteDuplexContract Channel = null;
        protected DuplexChannelFactory<UiPathREST.IUiPathRemoteDuplexContract> DuplexChannelFactory = null;

        public RobotClient()
        {
            DuplexChannelFactory = new DuplexChannelFactory<UiPathREST.IUiPathRemoteDuplexContract>(new InstanceContext(this), "DefaultDuplexEndpoint");
            DuplexChannelFactory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            Channel = DuplexChannelFactory.CreateChannel();
        }

        #region Service methods

        public bool IsAlive()
        {
            return Channel.IsAlive();
        }

        public Guid StartJob(string serializedJob)
        {
            var startJobResponse = Channel.StartJob(SerializeStringToStream(serializedJob));

            return Guid.Parse(startJobResponse);
        }

        public Guid StartJobAsync(string serializedJob)
        {
            var startJobResponse = Channel.StartJobAsync(SerializeStringToStream(serializedJob));
            return Guid.Parse(startJobResponse.Result);
        }

        public static Stream SerializeStringToStream(string jobValue)
        {
            if (jobValue == null) return null;

            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(jobValue);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public void CancelJob(Guid jobId)
        {
            Channel.CancelJob(jobId.ToString());
        }

        public WorkflowResponse QueryJob(Guid jobId)
        {
            var response = Channel.QueryJob(jobId.ToString());

            Console.WriteLine($"Queried job: {response}");
            var completedResult = JsonConvert.DeserializeObject<WorkflowResponse>(response, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None });
            return completedResult;
        }

        public WorkflowResponse QueryJobAsync(Guid jobId)
        {
            var response = Channel.QueryJobAsync(jobId.ToString());

            var status = response.Status;

            Console.WriteLine($"Queried job: {response} with status {status}");
            var completedResult = JsonConvert.DeserializeObject<WorkflowResponse>(response.Result, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None });
            return completedResult;
        }
        
        #endregion

        #region Duplex callbacks

        public bool OnTrackReceived(string serializedTrackingRecord)
        {
            return false;
        }

        // Implementation can allow for invokeCompletedInfo to be anything that is serialisable
        public void OnJobCompleted(string invokeCompletedInfo)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None
            };

            Console.WriteLine($"CompletedInfo: {invokeCompletedInfo}");
            var completedResult = JsonConvert.DeserializeObject<WorkflowResponse>(invokeCompletedInfo, settings);
            if(completedResult.State == System.Activities.ActivityInstanceState.Faulted)
            {
                Console.WriteLine(completedResult.Error.Message);
                return;
            }
            else if(completedResult.State ==  System.Activities.ActivityInstanceState.Canceled)
            {
                Console.WriteLine("Process cancelled");
            }
            else
            {
                Console.WriteLine("Completed without errors");
                
                var worker = Threading.RobotWorkflowThreads[completedResult.Token];
                worker.SetResult(completedResult);
                worker.RequestStop();
            }
        }

        public void OnLog(string logMessage)
        {
        }

        public void OnPackagesUpdated()
        {
            throw new NotImplementedException();
        }

        #endregion Duplex callbacks
    }
}
