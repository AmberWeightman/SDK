using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RobotAPISample.RequestsResponses;
using RobotAPISample.Workflows;

namespace RobotAPISample.RequestsResponses
{
    public static class WorkflowResponseFactory
    {
        // TODO this is being called from 2 different places, one of which is probably redundant...
        public static WorkflowResponse Create(WorkflowType type, WorkflowResponse copyFrom = null)
        {
            WorkflowResponse workflowResponse;
            switch (type)
            {
                case WorkflowType.CheckCitrixAvailable:
                    {
                        workflowResponse = new CheckCitrixAvailabilityResponse();
                        CopyFrom(workflowResponse, copyFrom);

                        if (copyFrom != null && copyFrom.Output.ContainsKey("IsAvailable"))
                        {
                            bool isAvailable;
                            Boolean.TryParse(copyFrom.Output["IsAvailable"].ToString(), out isAvailable);
                            ((CheckCitrixAvailabilityResponse)workflowResponse).IsAvailable = isAvailable;
                        }

                        break;
                    }
                case WorkflowType.CitrixDemo:
                    {
                        workflowResponse = new CitrixDemoResponse();
                        CopyFrom(workflowResponse, copyFrom);

                        if (copyFrom != null && copyFrom.Output.ContainsKey("SampleTextOutput"))
                        {
                            ((CitrixDemoResponse)workflowResponse).SampleTextOutput = copyFrom.Output["SampleTextOutput"].ToString();
                        }
                        
                        break;
                    }
                case WorkflowType.ChromeDownloadCitrix:
                    {
                        workflowResponse = new WorkflowResponse(); // Does not have its own response type
                        CopyFrom(workflowResponse, copyFrom);
                        break;
                    }
                case WorkflowType.LINZTitleSearch:
                    {
                        workflowResponse = new LINZTitleSearchWorkflowResponse();
                        CopyFrom(workflowResponse, copyFrom);

                        if (copyFrom != null)
                        {
                            if (copyFrom.Output.ContainsKey("SearchResults"))
                            {
                                var searchResults = JsonConvert.DeserializeObject<List<string>>(copyFrom.Output["SearchResults"].ToString(), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None });
                                if (searchResults.Any())
                                {
                                    ((LINZTitleSearchWorkflowResponse)workflowResponse).SearchResults = new List<SearchResult>();
                                    foreach (var searchResult in searchResults)
                                    {
                                        ((LINZTitleSearchWorkflowResponse)workflowResponse).SearchResults.Add(new SearchResult(searchResult));
                                    }
                                }
                            }

                            if (copyFrom.Output.ContainsKey("FileNames"))
                            {
                                ((LINZTitleSearchWorkflowResponse)workflowResponse).FileNames = JsonConvert.DeserializeObject<List<string>>(copyFrom.Output["FileNames"].ToString(), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None });
                            }
                        }
                        break;
                    }

                default:
                    throw new ApplicationException($"Workflow type {type} is not recognised.");
            }

            return workflowResponse;
        }

        private static void CopyFrom(WorkflowResponse copyToTarget, WorkflowResponse copyFromSource = null)
        {
            if (copyFromSource == null) return;
            copyToTarget.State = copyFromSource.State;
            copyToTarget.Error = copyFromSource.Error;
            copyToTarget.Output = copyFromSource.Output;
            copyToTarget.Token = copyFromSource.Token;
            copyToTarget.WorkflowFile = copyFromSource.WorkflowFile;

            if (copyToTarget is WorkflowSearchResponse && copyFromSource.Output.ContainsKey("OrderIds"))
            {
                ((WorkflowSearchResponse)copyToTarget).OrderIds = JsonConvert.DeserializeObject<string[]>(copyFromSource.Output.ContainsKey("OrderIds").ToString(), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None });
            }
        }
    }
}
