using IAMBuddy.SqlServerManagementService.Models;

namespace IAMBuddy.SqlServerManagementService.Services;

public interface ISqlLoginService
{
    Task<SqlServerLogin> CreateLoginAsync(string loginName, string password, string serverInstance, string? defaultDatabase = null, string? createdBy = null);
    Task<SqlServerLogin> CreateWindowsLoginAsync(string loginName, string serverInstance, string? defaultDatabase = null, string? createdBy = null);
    Task<SqlServerLogin?> GetLoginAsync(string loginName, string serverInstance);
    Task<IEnumerable<SqlServerLogin>> GetLoginsAsync(string serverInstance);
    Task<bool> DeleteLoginAsync(string loginName, string serverInstance, string? deletedBy = null);
    Task<bool> EnableLoginAsync(string loginName, string serverInstance, string? modifiedBy = null);
    Task<bool> DisableLoginAsync(string loginName, string serverInstance, string? modifiedBy = null);
    Task<bool> ChangePasswordAsync(string loginName, string newPassword, string serverInstance, string? modifiedBy = null);
    Task<bool> AddToServerRoleAsync(string loginName, string roleName, string serverInstance, string? modifiedBy = null);
    Task<bool> RemoveFromServerRoleAsync(string loginName, string roleName, string serverInstance, string? modifiedBy = null);
    Task<IEnumerable<string>> GetServerRolesForLoginAsync(string loginName, string serverInstance);
    Task<bool> LoginExistsAsync(string loginName, string serverInstance);
    Task<bool> ValidateLoginNameAsync(string loginName);
    Task<bool> ValidatePasswordAsync(string password);
}