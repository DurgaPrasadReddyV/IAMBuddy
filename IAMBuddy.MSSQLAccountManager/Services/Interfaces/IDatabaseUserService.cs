using IAMBuddy.MSSQLAccountManager.Models;

namespace IAMBuddy.MSSQLAccountManager.Services.Interfaces
{
    public interface IDatabaseUserService
    {
        Task<OperationResult> CreateUserAsync(DatabaseUser user);
        Task<OperationResult> UpdateUserAsync(Guid userId, DatabaseUser updatedUser);
        Task<OperationResult> DeleteUserAsync(Guid userId);
        Task<DatabaseUser?> GetUserByIdAsync(Guid userId);
        Task<DatabaseUser?> GetUserByNameAsync(string serverName, string? instanceName, string databaseName, string userName);
        Task<IEnumerable<DatabaseUser>> GetAllUsersAsync(string serverName, string? instanceName = null, string? databaseName = null);
        Task<IEnumerable<DatabaseUser>> GetUsersByLoginAsync(Guid loginId);
    }
}