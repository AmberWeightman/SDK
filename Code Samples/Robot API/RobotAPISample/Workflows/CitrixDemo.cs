using RobotAPISample.RequestsResponses;

namespace RobotAPISample.Workflows
{
    public class CitrixDemoRequest : WorkflowRequest
    {
        public string SurveyNumber { get; set; }
        public string District { get; set; }
    }

    public class CitrixDemoResponse : WorkflowResponse
    {
        public string SampleTextOutput { get; set; }
    }

    public class CitrixDemoWorkflow : RobotWorkflowBase<CitrixDemoRequest, CitrixDemoResponse>
    {
        public  override string WorkflowFile => WorkflowFiles.CitrixDemoWorkflow;
        public override WorkflowType WorkflowType => WorkflowType.CitrixDemo;
        
    }

}
