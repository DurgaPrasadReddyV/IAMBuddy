using IAMBuddy.SqlServerManagementService.Models;

namespace IAMBuddy.SqlServerManagementService.Services;

public interface IServerRoleService
{
    Task<SqlServerRole> CreateServerRoleAsync(string roleName, string serverInstance, string? createdBy = null);
    Task<SqlServerRole?> GetServerRoleAsync(string roleName, string serverInstance);
    Task<IEnumerable<SqlServerRole>> GetServerRolesAsync(string serverInstance);
    Task<bool> DeleteServerRoleAsync(string roleName, string serverInstance, string? deletedBy = null);
    Task<bool> AddMemberToRoleAsync(string roleName, string memberName, string serverInstance, string? modifiedBy = null);
    Task<bool> RemoveMemberFromRoleAsync(string roleName, string memberName, string serverInstance, string? modifiedBy = null);
    Task<IEnumerable<string>> GetRoleMembersAsync(string roleName, string serverInstance);
    Task<bool> ServerRoleExistsAsync(string roleName, string serverInstance);
    Task<bool> GrantPermissionToRoleAsync(string roleName, string permission, string serverInstance, string? objectName = null, string? grantedBy = null);
    Task<bool> RevokePermissionFromRoleAsync(string roleName, string permission, string serverInstance, string? objectName = null, string? revokedBy = null);
    Task<IEnumerable<string>> GetRolePermissionsAsync(string roleName, string serverInstance);
    Task<bool> ValidateRoleNameAsync(string roleName);
    Task<bool> ValidatePermissionAsync(string permission);
}