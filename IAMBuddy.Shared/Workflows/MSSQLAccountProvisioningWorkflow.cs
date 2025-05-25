using IAMBuddy.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Temporalio.Workflows;

namespace IAMBuddy.Shared.Workflows
{
    [Workflow]
    public class MSSQLAccountProvisioningWorkflow
    {
        [WorkflowRun]
        public async Task<string> SubmitRequestAsync(MSSQLAccountRequest request)
        {
            return await Workflow.ExecuteActivityAsync(
                (MSSQLAccountProvisioningWorkflowActivities act) => act.RequestIntakeNotification(request),
                new() { ScheduleToCloseTimeout = TimeSpan.FromMinutes(5) });
        }
    }
}
