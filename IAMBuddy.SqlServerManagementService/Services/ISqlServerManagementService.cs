using IAMBuddy.SqlServerManagementService.Models;

namespace IAMBuddy.SqlServerManagementService.Services;

public interface ISqlServerManagementService
{
    // Login management
    Task<SqlServerLogin> CreateLoginAsync(SqlServerLogin login);
    Task<SqlServerLogin?> GetLoginAsync(int id);
    Task<SqlServerLogin?> GetLoginByNameAsync(string loginName, string serverInstance);
    Task<IEnumerable<SqlServerLogin>> GetLoginsAsync(string serverInstance);
    Task<SqlServerLogin> UpdateLoginAsync(SqlServerLogin login);
    Task<bool> DeleteLoginAsync(int id);

    // User management
    Task<SqlServerUser> CreateUserAsync(SqlServerUser user);
    Task<SqlServerUser?> GetUserAsync(int id);
    Task<SqlServerUser?> GetUserByNameAsync(string userName, string databaseName, string serverInstance);
    Task<IEnumerable<SqlServerUser>> GetUsersAsync(string serverInstance, string? databaseName = null);
    Task<SqlServerUser> UpdateUserAsync(SqlServerUser user);
    Task<bool> DeleteUserAsync(int id);

    // Role management
    Task<SqlServerRole> CreateRoleAsync(SqlServerRole role);
    Task<SqlServerRole?> GetRoleAsync(int id);
    Task<SqlServerRole?> GetRoleByNameAsync(string roleName, string serverInstance, string? databaseName = null);
    Task<IEnumerable<SqlServerRole>> GetRolesAsync(string serverInstance, string? databaseName = null);
    Task<SqlServerRole> UpdateRoleAsync(SqlServerRole role);
    Task<bool> DeleteRoleAsync(int id);

    // Role assignment management
    Task<SqlServerRoleAssignment> CreateRoleAssignmentAsync(SqlServerRoleAssignment assignment);
    Task<SqlServerRoleAssignment?> GetRoleAssignmentAsync(int id);
    Task<IEnumerable<SqlServerRoleAssignment>> GetRoleAssignmentsAsync(string serverInstance, string? databaseName = null);
    Task<IEnumerable<SqlServerRoleAssignment>> GetRoleAssignmentsByLoginAsync(int loginId);
    Task<IEnumerable<SqlServerRoleAssignment>> GetRoleAssignmentsByUserAsync(int userId);
    Task<SqlServerRoleAssignment> UpdateRoleAssignmentAsync(SqlServerRoleAssignment assignment);
    Task<bool> DeleteRoleAssignmentAsync(int id);
}