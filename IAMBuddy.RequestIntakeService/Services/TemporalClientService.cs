using IAMBuddy.Shared.Models;
using IAMBuddy.Shared.Workflows;
using Temporalio.Client;

namespace IAMBuddy.RequestIntakeService.Services
{
    public class TemporalClientService
    {
        private readonly ITemporalClient _temporalClient;
        private readonly ILogger<TemporalClientService> _logger;

        public TemporalClientService(ILogger<TemporalClientService> logger, ITemporalClient temporalClient)
        {
            _logger = logger;
            _temporalClient = temporalClient;
        }

        public async Task StartAccountProvisioningWorkflowAsync(MSSQLAccountRequest request)
        {
            try
            {
                var handle = await _temporalClient.StartWorkflowAsync<MSSQLAccountProvisioningWorkflow>(
                    workflow => workflow.SubmitRequestAsync(request),
                    new WorkflowOptions
                    {
                        Id = request.Id.ToString(),
                        TaskQueue = "mssqlaccount-provisioning"
                    });

                _logger.LogInformation("Started workflow {WorkflowId} for account request {RequestId}",
                    request.Id, request.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start workflow for account request {RequestId}", request.Id);
                throw;
            }
        }

        public async Task<WorkflowState?> GetWorkflowStatusAsync(string workflowId)
        {
            try
            {
                var handle = _temporalClient.GetWorkflowHandle(workflowId);
                var result = await handle.QueryAsync<WorkflowState>("GetCurrentState", Array.Empty<object>());
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get workflow status for {WorkflowId}", workflowId);
                return null;
            }
        }
    }
}
