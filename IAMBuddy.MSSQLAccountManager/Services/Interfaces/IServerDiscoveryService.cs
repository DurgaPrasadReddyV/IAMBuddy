using IAMBuddy.MSSQLAccountManager.Models;

namespace IAMBuddy.MSSQLAccountManager.Services.Interfaces
{
    public interface IServerDiscoveryService
    {
        Task<IEnumerable<ServerInstance>> DiscoverServersAsync();
        Task<IEnumerable<ServerInstance>> GetAvailabilityGroupInstancesAsync(string listenerName);
        Task<OperationResult> RegisterServerInstanceAsync(ServerInstance serverInstance);
        Task<OperationResult> UpdateServerInstanceAsync(Guid instanceId, ServerInstance updatedInstance);
        Task<OperationResult> RemoveServerInstanceAsync(Guid instanceId);
        Task<ServerInstance?> GetServerInstanceAsync(string serverName, string? instanceName = null);
        Task<IEnumerable<ServerInstance>> GetAllServerInstancesAsync();
        Task<OperationResult> TestConnectionAsync(Guid instanceId);
        Task<OperationResult> PerformHealthCheckAsync(Guid instanceId);
    }
}