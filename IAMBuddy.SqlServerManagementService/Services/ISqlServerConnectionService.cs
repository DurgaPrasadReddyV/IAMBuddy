using System.Data.SqlClient;

namespace IAMBuddy.SqlServerManagementService.Services;

public interface ISqlServerConnectionService
{
    Task<SqlConnection> GetConnectionAsync(string serverInstance, string? databaseName = null, int timeoutSeconds = 30);
    Task<SqlConnection> GetConnectionWithCredentialsAsync(string serverInstance, string databaseName, string? userId = null, string? password = null, int timeoutSeconds = 30);
    Task<bool> TestConnectionAsync(string serverInstance, string? databaseName = null);
    Task<IEnumerable<string>> DiscoverAvailabilityGroupListenersAsync(string serverInstance);
    Task<string> GetPrimaryReplicaAsync(string serverInstance);
    Task<bool> IsAvailabilityGroupPrimaryAsync(string serverInstance);
    string BuildConnectionString(string serverInstance, string? databaseName = null, string? userId = null, string? password = null, int timeoutSeconds = 30, bool integratedSecurity = true);
}