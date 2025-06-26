using IAMBuddy.MSSQLAccountManager.Models;

namespace IAMBuddy.MSSQLAccountManager.Services.Interfaces
{
    public interface IAuditService
    {
        Task<OperationResult> LogOperationAsync(OperationType operationType, string resourceType, string resourceName, 
            string serverName, string? databaseName = null, string? details = null, string? createdBy = null);
        Task<OperationResult> UpdateOperationResultAsync(Guid operationId, OperationStatus status, 
            string? errorMessage = null, string? details = null);
        Task<IEnumerable<OperationResult>> GetOperationHistoryAsync(string? resourceType = null, 
            string? serverName = null, DateTime? fromDate = null, DateTime? toDate = null);
        Task<OperationResult?> GetOperationByIdAsync(Guid operationId);
        Task<IEnumerable<OperationResult>> GetFailedOperationsAsync(DateTime? fromDate = null);
    }
}