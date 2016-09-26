using RobotAPISample.RequestsResponses;

namespace RobotAPISample.Workflows
{
    public class ChromeDownloadCitrixWorkflow : RobotWorkflowBase<WorkflowRequest, WorkflowResponse>
    {
        public  override string WorkflowFile => WorkflowFiles.ChromeDownloadCitrixWorkflow;
        public override WorkflowType WorkflowType => WorkflowType.ChromeDownloadCitrix;
    }

}
