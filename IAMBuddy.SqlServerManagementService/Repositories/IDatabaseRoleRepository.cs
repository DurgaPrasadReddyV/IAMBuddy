using IAMBuddy.SqlServerManagementService.Models;

namespace IAMBuddy.SqlServerManagementService.Repositories;

public interface IDatabaseRoleRepository
{
    Task<bool> CreateRoleAsync(string roleName, string databaseName, string serverInstance, string? ownerName = null);
    Task<SqlServerRole?> GetRoleAsync(string roleName, string databaseName, string serverInstance);
    Task<IEnumerable<SqlServerRole>> GetRolesAsync(string databaseName, string serverInstance);
    Task<bool> DropRoleAsync(string roleName, string databaseName, string serverInstance);
    Task<bool> AddMemberToRoleAsync(string roleName, string memberName, string databaseName, string serverInstance);
    Task<bool> RemoveMemberFromRoleAsync(string roleName, string memberName, string databaseName, string serverInstance);
    Task<IEnumerable<string>> GetRoleMembersAsync(string roleName, string databaseName, string serverInstance);
    Task<bool> RoleExistsAsync(string roleName, string databaseName, string serverInstance);
    Task<bool> GrantPermissionToRoleAsync(string roleName, string permission, string databaseName, string serverInstance, string? objectName = null);
    Task<bool> RevokePermissionFromRoleAsync(string roleName, string permission, string databaseName, string serverInstance, string? objectName = null);
    Task<IEnumerable<string>> GetRolePermissionsAsync(string roleName, string databaseName, string serverInstance);
    Task<bool> AlterRoleOwnerAsync(string roleName, string newOwner, string databaseName, string serverInstance);
}