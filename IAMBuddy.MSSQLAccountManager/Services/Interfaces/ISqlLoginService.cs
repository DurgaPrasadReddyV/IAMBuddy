using IAMBuddy.MSSQLAccountManager.Models;

namespace IAMBuddy.MSSQLAccountManager.Services.Interfaces
{
    public interface ISqlLoginService
    {
        Task<OperationResult> CreateLoginAsync(SqlLogin login, string? password = null);
        Task<OperationResult> UpdateLoginAsync(Guid loginId, SqlLogin updatedLogin);
        Task<OperationResult> DeleteLoginAsync(Guid loginId);
        Task<SqlLogin?> GetLoginByIdAsync(Guid loginId);
        Task<SqlLogin?> GetLoginByNameAsync(string serverName, string? instanceName, string loginName);
        Task<IEnumerable<SqlLogin>> GetAllLoginsAsync(string serverName, string? instanceName = null);
        Task<OperationResult> EnableLoginAsync(Guid loginId);
        Task<OperationResult> DisableLoginAsync(Guid loginId);
        Task<OperationResult> ChangePasswordAsync(Guid loginId, string newPassword);
    }
}