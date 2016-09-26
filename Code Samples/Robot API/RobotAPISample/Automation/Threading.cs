using System;
using System.Collections.Generic;

namespace RobotAPISample.Automation
{
    public static class Threading
    {
        // TODO this is probably a really bad way to handle threading
        public volatile static Dictionary<Guid, IRobotWorker> RobotWorkflowThreads = new Dictionary<Guid, IRobotWorker>();
    }
}
