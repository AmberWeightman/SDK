using System;
using System.Collections.Generic;
using System.Linq;

namespace RobotAPISample.RequestsResponses
{
    public abstract class WorkflowSearchRequest : WorkflowRequest
    {
        public string[] OrderIds { get; set; }

        public const int DefaultTimeoutMS = 2000;

        public const string OutputDirectory = @"C:\LandonlineOutputDirectory\";

        public override bool Validate()
        {
            if (OrderIds == null || !OrderIds.Any())
            {
                throw new ApplicationException("OrderId is mandatory");
            }
            return base.Validate();
        }
    }

    public abstract class WorkflowSearchResponse : WorkflowResponse
    {
        public string[] OrderIds { get; set; }

        /// <summary>
        /// Was the search completed successfully with no errors?
        /// </summary>
        public bool Success { get; set; }

        public override bool Validate()
        {
            if (OrderIds == null || !OrderIds.Any())
            {
                throw new ApplicationException("OrderId is mandatory");
            }
            return base.Validate();
        }
    }
}
