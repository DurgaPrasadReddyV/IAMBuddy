using IAMBuddy.SqlServerManagementService.Models;

namespace IAMBuddy.SqlServerManagementService.Services;

public interface ISqlServerOperationService
{
    Task<SqlServerOperation> CreateOperationAsync(SqlServerOperation operation);
    Task<SqlServerOperation?> GetOperationAsync(int id);
    Task<IEnumerable<SqlServerOperation>> GetOperationsAsync(string? status = null, string? requestId = null);
    Task<IEnumerable<SqlServerOperation>> GetOperationsByServerAsync(string serverInstance);
    Task<SqlServerOperation> UpdateOperationAsync(SqlServerOperation operation);
    Task<SqlServerOperation> UpdateOperationStatusAsync(int id, string status, string? errorMessage = null);
    Task<SqlServerOperation> CompleteOperationAsync(int id, bool success, string? errorMessage = null);
    Task<IEnumerable<SqlServerOperation>> GetPendingOperationsAsync();
    Task<IEnumerable<SqlServerOperation>> GetFailedOperationsAsync();
}