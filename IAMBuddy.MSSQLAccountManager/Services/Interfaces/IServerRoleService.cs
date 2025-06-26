using IAMBuddy.MSSQLAccountManager.Models;

namespace IAMBuddy.MSSQLAccountManager.Services.Interfaces
{
    public interface IServerRoleService
    {
        Task<OperationResult> CreateRoleAsync(ServerRole role);
        Task<OperationResult> UpdateRoleAsync(Guid roleId, ServerRole updatedRole);
        Task<OperationResult> DeleteRoleAsync(Guid roleId);
        Task<ServerRole?> GetRoleByIdAsync(Guid roleId);
        Task<ServerRole?> GetRoleByNameAsync(string serverName, string? instanceName, string roleName);
        Task<IEnumerable<ServerRole>> GetAllRolesAsync(string serverName, string? instanceName = null);
        Task<OperationResult> AssignRoleToLoginAsync(Guid loginId, Guid roleId);
        Task<OperationResult> RemoveRoleFromLoginAsync(Guid loginId, Guid roleId);
        Task<IEnumerable<ServerRoleAssignment>> GetLoginRoleAssignmentsAsync(Guid loginId);
        Task<IEnumerable<ServerRoleAssignment>> GetRoleAssignmentsAsync(Guid roleId);
    }
}