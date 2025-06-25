using IAMBuddy.MSSQLAccountManager.Models;
using IAMBuddy.MSSQLAccountManager.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IAMBuddy.MSSQLAccountManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditController : ControllerBase
    {
        private readonly IAuditService _auditService;
        private readonly ILogger<AuditController> _logger;

        public AuditController(IAuditService auditService, ILogger<AuditController> logger)
        {
            _auditService = auditService;
            _logger = logger;
        }

        [HttpGet("operations")]
        public async Task<ActionResult<IEnumerable<OperationResult>>> GetOperationHistory(
            [FromQuery] string? resourceType = null,
            [FromQuery] string? serverName = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var operations = await _auditService.GetOperationHistoryAsync(resourceType, serverName, fromDate, toDate);
                return Ok(operations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving operation history");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("operations/{operationId}")]
        public async Task<ActionResult<OperationResult>> GetOperation(Guid operationId)
        {
            try
            {
                var operation = await _auditService.GetOperationByIdAsync(operationId);
                if (operation == null)
                    return NotFound($"Operation with ID {operationId} not found");

                return Ok(operation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving operation {OperationId}", operationId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("operations/failed")]
        public async Task<ActionResult<IEnumerable<OperationResult>>> GetFailedOperations([FromQuery] DateTime? fromDate = null)
        {
            try
            {
                var failedOperations = await _auditService.GetFailedOperationsAsync(fromDate);
                return Ok(failedOperations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving failed operations");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}