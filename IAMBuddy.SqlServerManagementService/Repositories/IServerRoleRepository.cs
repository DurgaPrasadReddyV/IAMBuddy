using IAMBuddy.SqlServerManagementService.Models;

namespace IAMBuddy.SqlServerManagementService.Repositories;

public interface IServerRoleRepository
{
    Task<bool> CreateServerRoleAsync(string roleName, string serverInstance);
    Task<SqlServerRole?> GetServerRoleAsync(string roleName, string serverInstance);
    Task<IEnumerable<SqlServerRole>> GetServerRolesAsync(string serverInstance);
    Task<bool> DropServerRoleAsync(string roleName, string serverInstance);
    Task<bool> AddMemberToRoleAsync(string roleName, string memberName, string serverInstance);
    Task<bool> RemoveMemberFromRoleAsync(string roleName, string memberName, string serverInstance);
    Task<IEnumerable<string>> GetRoleMembersAsync(string roleName, string serverInstance);
    Task<bool> ServerRoleExistsAsync(string roleName, string serverInstance);
    Task<bool> GrantPermissionToRoleAsync(string roleName, string permission, string serverInstance, string? objectName = null);
    Task<bool> RevokePermissionFromRoleAsync(string roleName, string permission, string serverInstance, string? objectName = null);
    Task<IEnumerable<string>> GetRolePermissionsAsync(string roleName, string serverInstance);
}