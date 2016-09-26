using RobotAPISample.RequestsResponses;

namespace RobotAPISample.Workflows
{
    public class CheckCitrixAvailabilityResponse : WorkflowResponse
    {
        public bool IsAvailable { get; set; }
    }

    public class CheckCitrixAvailabilityWorkflow : RobotWorkflowBase<WorkflowRequest, CheckCitrixAvailabilityResponse>
    {
        public  override string WorkflowFile => WorkflowFiles.CheckCitrixAvailableWorkflow;
        public override WorkflowType WorkflowType => WorkflowType.CheckCitrixAvailable;
   
    }

}
