namespace RobotAPISample.RequestsResponses
{
    public class WorkflowRequest : IWorkflowMessage
    {
        public virtual bool Validate()
        {
            return true;
        }
    }
}
