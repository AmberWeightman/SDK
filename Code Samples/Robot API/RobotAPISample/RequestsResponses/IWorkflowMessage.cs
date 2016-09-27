namespace RobotAPISample.RequestsResponses
{
    public interface IWorkflowRequest
    {
        bool Validate();
    }

    public interface IWorkflowResponse
    {
        bool Validate(IWorkflowRequest request);
    }
}
