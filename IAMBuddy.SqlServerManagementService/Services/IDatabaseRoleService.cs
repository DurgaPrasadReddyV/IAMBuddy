using IAMBuddy.SqlServerManagementService.Models;

namespace IAMBuddy.SqlServerManagementService.Services;

public interface IDatabaseRoleService
{
    Task<SqlServerRole> CreateRoleAsync(string roleName, string databaseName, string serverInstance, string? ownerName = null, string? createdBy = null);
    Task<SqlServerRole?> GetRoleAsync(string roleName, string databaseName, string serverInstance);
    Task<IEnumerable<SqlServerRole>> GetRolesAsync(string databaseName, string serverInstance);
    Task<bool> DeleteRoleAsync(string roleName, string databaseName, string serverInstance, string? deletedBy = null);
    Task<bool> AddMemberToRoleAsync(string roleName, string memberName, string databaseName, string serverInstance, string? modifiedBy = null);
    Task<bool> RemoveMemberFromRoleAsync(string roleName, string memberName, string databaseName, string serverInstance, string? modifiedBy = null);
    Task<IEnumerable<string>> GetRoleMembersAsync(string roleName, string databaseName, string serverInstance);
    Task<bool> RoleExistsAsync(string roleName, string databaseName, string serverInstance);
    Task<bool> GrantPermissionToRoleAsync(string roleName, string permission, string databaseName, string serverInstance, string? objectName = null, string? grantedBy = null);
    Task<bool> RevokePermissionFromRoleAsync(string roleName, string permission, string databaseName, string serverInstance, string? objectName = null, string? revokedBy = null);
    Task<IEnumerable<string>> GetRolePermissionsAsync(string roleName, string databaseName, string serverInstance);
    Task<bool> AlterRoleOwnerAsync(string roleName, string newOwner, string databaseName, string serverInstance, string? modifiedBy = null);
    Task<bool> ValidateRoleNameAsync(string roleName);
    Task<bool> ValidateOwnerNameAsync(string ownerName);
    Task<SqlServerRole> CreateRoleWithTransactionAsync(string roleName, string databaseName, string serverInstance, IEnumerable<string>? initialMembers = null, string? ownerName = null, string? createdBy = null);
    Task<bool> DeleteRoleWithTransactionAsync(string roleName, string databaseName, string serverInstance, bool forceDelete = false, string? deletedBy = null);
}