using IAMBuddy.SqlServerManagementService.Models;

namespace IAMBuddy.SqlServerManagementService.Services;

public interface IDatabaseUserService
{
    Task<SqlServerUser> CreateUserAsync(string userName, string loginName, string databaseName, string serverInstance, string? defaultSchema = null, string? createdBy = null);
    Task<SqlServerUser> CreateUserWithoutLoginAsync(string userName, string databaseName, string serverInstance, string? defaultSchema = null, string? createdBy = null);
    Task<SqlServerUser?> GetUserAsync(string userName, string databaseName, string serverInstance);
    Task<IEnumerable<SqlServerUser>> GetUsersAsync(string databaseName, string serverInstance);
    Task<IEnumerable<SqlServerUser>> GetUsersByLoginAsync(string loginName, string serverInstance);
    Task<bool> DeleteUserAsync(string userName, string databaseName, string serverInstance, string? deletedBy = null);
    Task<bool> AlterUserDefaultSchemaAsync(string userName, string defaultSchema, string databaseName, string serverInstance, string? modifiedBy = null);
    Task<bool> AddToRoleAsync(string userName, string roleName, string databaseName, string serverInstance, string? modifiedBy = null);
    Task<bool> RemoveFromRoleAsync(string userName, string roleName, string databaseName, string serverInstance, string? modifiedBy = null);
    Task<IEnumerable<string>> GetUserRolesAsync(string userName, string databaseName, string serverInstance);
    Task<bool> UserExistsAsync(string userName, string databaseName, string serverInstance);
    Task<bool> GrantPermissionAsync(string userName, string permission, string databaseName, string serverInstance, string? objectName = null, string? grantedBy = null);
    Task<bool> RevokePermissionAsync(string userName, string permission, string databaseName, string serverInstance, string? objectName = null, string? revokedBy = null);
    Task<IEnumerable<string>> GetUserPermissionsAsync(string userName, string databaseName, string serverInstance);
    Task<bool> ValidateUserNameAsync(string userName);
    Task<bool> ValidateSchemaNameAsync(string schemaName);
}