using IAMBuddy.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Temporalio.Activities;
using Temporalio.Workflows;

namespace IAMBuddy.Shared.Workflows
{
    public class MSSQLAccountProvisioningWorkflowActivities
    {
        [Activity]
        public string RequestIntakeNotification(MSSQLAccountRequest request) => $"Hello, {request.DatabaseName}!";
    }
}
