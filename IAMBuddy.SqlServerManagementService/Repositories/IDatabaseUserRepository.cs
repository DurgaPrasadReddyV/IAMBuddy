using IAMBuddy.SqlServerManagementService.Models;

namespace IAMBuddy.SqlServerManagementService.Repositories;

public interface IDatabaseUserRepository
{
    Task<bool> CreateUserAsync(string userName, string loginName, string databaseName, string serverInstance, string? defaultSchema = null);
    Task<bool> CreateUserWithoutLoginAsync(string userName, string databaseName, string serverInstance, string? defaultSchema = null);
    Task<SqlServerUser?> GetUserAsync(string userName, string databaseName, string serverInstance);
    Task<IEnumerable<SqlServerUser>> GetUsersAsync(string databaseName, string serverInstance);
    Task<bool> DropUserAsync(string userName, string databaseName, string serverInstance);
    Task<bool> AlterUserDefaultSchemaAsync(string userName, string defaultSchema, string databaseName, string serverInstance);
    Task<bool> AddToRoleAsync(string userName, string roleName, string databaseName, string serverInstance);
    Task<bool> RemoveFromRoleAsync(string userName, string roleName, string databaseName, string serverInstance);
    Task<IEnumerable<string>> GetUserRolesAsync(string userName, string databaseName, string serverInstance);
    Task<bool> UserExistsAsync(string userName, string databaseName, string serverInstance);
    Task<bool> GrantPermissionAsync(string userName, string permission, string databaseName, string serverInstance, string? objectName = null);
    Task<bool> RevokePermissionAsync(string userName, string permission, string databaseName, string serverInstance, string? objectName = null);
    Task<IEnumerable<string>> GetUserPermissionsAsync(string userName, string databaseName, string serverInstance);
}