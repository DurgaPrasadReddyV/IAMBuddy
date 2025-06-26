using IAMBuddy.MSSQLAccountManager.Data;
using IAMBuddy.MSSQLAccountManager.Models;
using IAMBuddy.MSSQLAccountManager.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IAMBuddy.MSSQLAccountManager.Services
{
    public class AuditService : IAuditService
    {
        private readonly MSSQLAccountManagerContext _context;
        private readonly ILogger<AuditService> _logger;

        public AuditService(MSSQLAccountManagerContext context, ILogger<AuditService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<OperationResult> LogOperationAsync(OperationType operationType, string resourceType, 
            string resourceName, string serverName, string? databaseName = null, string? details = null, 
            string? createdBy = null)
        {
            var operation = new OperationResult
            {
                OperationType = operationType,
                Status = OperationStatus.InProgress,
                ResourceType = resourceType,
                ResourceName = resourceName,
                ServerName = serverName,
                DatabaseName = databaseName,
                Details = details,
                CreatedBy = createdBy ?? "System",
                StartTime = DateTime.UtcNow
            };

            _context.OperationResults.Add(operation);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Logged operation {OperationType} for {ResourceType} '{ResourceName}' on {ServerName}", 
                operationType, resourceType, resourceName, serverName);

            return operation;
        }

        public async Task<OperationResult> UpdateOperationResultAsync(Guid operationId, OperationStatus status, 
            string? errorMessage = null, string? details = null)
        {
            var operation = await _context.OperationResults.FindAsync(operationId);
            if (operation == null)
            {
                throw new ArgumentException($"Operation with ID '{operationId}' not found");
            }

            operation.Status = status;
            operation.EndTime = DateTime.UtcNow;
            operation.ErrorMessage = errorMessage;
            
            if (!string.IsNullOrEmpty(details))
            {
                operation.Details = string.IsNullOrEmpty(operation.Details) 
                    ? details 
                    : $"{operation.Details}; {details}";
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated operation {OperationId} status to {Status}", operationId, status);

            return operation;
        }

        public async Task<IEnumerable<OperationResult>> GetOperationHistoryAsync(string? resourceType = null, 
            string? serverName = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.OperationResults.AsQueryable();

            if (!string.IsNullOrEmpty(resourceType))
                query = query.Where(o => o.ResourceType == resourceType);

            if (!string.IsNullOrEmpty(serverName))
                query = query.Where(o => o.ServerName == serverName);

            if (fromDate.HasValue)
                query = query.Where(o => o.StartTime >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(o => o.StartTime <= toDate.Value);

            return await query.OrderByDescending(o => o.StartTime).ToListAsync();
        }

        public async Task<OperationResult?> GetOperationByIdAsync(Guid operationId)
        {
            return await _context.OperationResults.FindAsync(operationId);
        }

        public async Task<IEnumerable<OperationResult>> GetFailedOperationsAsync(DateTime? fromDate = null)
        {
            var query = _context.OperationResults
                .Where(o => o.Status == OperationStatus.Failed);

            if (fromDate.HasValue)
                query = query.Where(o => o.StartTime >= fromDate.Value);

            return await query.OrderByDescending(o => o.StartTime).ToListAsync();
        }
    }
}