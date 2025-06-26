using IAMBuddy.MSSQLAccountManager.Models;

namespace IAMBuddy.MSSQLAccountManager.Services.Interfaces
{
    public interface IDatabaseRoleService
    {
        Task<OperationResult> CreateRoleAsync(DatabaseRole role);
        Task<OperationResult> UpdateRoleAsync(Guid roleId, DatabaseRole updatedRole);
        Task<OperationResult> DeleteRoleAsync(Guid roleId);
        Task<DatabaseRole?> GetRoleByIdAsync(Guid roleId);
        Task<DatabaseRole?> GetRoleByNameAsync(string serverName, string? instanceName, string databaseName, string roleName);
        Task<IEnumerable<DatabaseRole>> GetAllRolesAsync(string serverName, string? instanceName = null, string? databaseName = null);
        Task<OperationResult> AssignRoleToUserAsync(Guid userId, Guid roleId);
        Task<OperationResult> RemoveRoleFromUserAsync(Guid userId, Guid roleId);
        Task<IEnumerable<DatabaseRoleAssignment>> GetUserRoleAssignmentsAsync(Guid userId);
        Task<IEnumerable<DatabaseRoleAssignment>> GetRoleAssignmentsAsync(Guid roleId);
    }
}