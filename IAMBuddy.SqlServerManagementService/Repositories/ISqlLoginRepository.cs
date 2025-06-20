using IAMBuddy.SqlServerManagementService.Models;

namespace IAMBuddy.SqlServerManagementService.Repositories;

public interface ISqlLoginRepository
{
    Task<bool> CreateLoginAsync(string loginName, string password, string serverInstance, string? defaultDatabase = null);
    Task<bool> CreateWindowsLoginAsync(string loginName, string serverInstance, string? defaultDatabase = null);
    Task<SqlServerLogin?> GetLoginAsync(string loginName, string serverInstance);
    Task<IEnumerable<SqlServerLogin>> GetLoginsAsync(string serverInstance);
    Task<bool> DropLoginAsync(string loginName, string serverInstance);
    Task<bool> EnableLoginAsync(string loginName, string serverInstance);
    Task<bool> DisableLoginAsync(string loginName, string serverInstance);
    Task<bool> ChangePasswordAsync(string loginName, string newPassword, string serverInstance);
    Task<bool> AddToServerRoleAsync(string loginName, string roleName, string serverInstance);
    Task<bool> RemoveFromServerRoleAsync(string loginName, string roleName, string serverInstance);
    Task<IEnumerable<string>> GetServerRolesForLoginAsync(string loginName, string serverInstance);
    Task<bool> LoginExistsAsync(string loginName, string serverInstance);
}