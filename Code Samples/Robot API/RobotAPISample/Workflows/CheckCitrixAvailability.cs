using System;
using RobotAPISample.RequestsResponses;

namespace RobotAPISample.Workflows
{
    public class CheckCitrixAvailabilityWorkflow : RobotWorkflowBase<WorkflowRequest, WorkflowResponse>
    {
        public override string WorkflowFile => WorkflowFiles.CheckCitrixAvailableWorkflow;

        public override WorkflowType WorkflowType => WorkflowType.CheckCitrixAvailable;

        public override int MaxWorkflowDurationMins => 2;

    }

}
