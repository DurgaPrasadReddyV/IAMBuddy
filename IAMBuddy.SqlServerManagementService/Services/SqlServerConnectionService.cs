using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;

namespace IAMBuddy.SqlServerManagementService.Services;

public class SqlServerConnectionService : ISqlServerConnectionService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SqlServerConnectionService> _logger;

    public SqlServerConnectionService(IConfiguration configuration, ILogger<SqlServerConnectionService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<SqlConnection> GetConnectionAsync(string serverInstance, string? databaseName = null, int timeoutSeconds = 30)
    {
        var connectionString = BuildConnectionString(serverInstance, databaseName, timeoutSeconds: timeoutSeconds);
        var connection = new SqlConnection(connectionString);
        
        try
        {
            await connection.OpenAsync();
            return connection;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to SQL Server instance {ServerInstance}, Database: {Database}", 
                serverInstance, databaseName ?? "master");
            connection.Dispose();
            throw;
        }
    }

    public async Task<SqlConnection> GetConnectionWithCredentialsAsync(string serverInstance, string databaseName, string? userId = null, string? password = null, int timeoutSeconds = 30)
    {
        var connectionString = BuildConnectionString(serverInstance, databaseName, userId, password, timeoutSeconds, integratedSecurity: userId == null);
        var connection = new SqlConnection(connectionString);
        
        try
        {
            await connection.OpenAsync();
            return connection;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to SQL Server instance {ServerInstance}, Database: {Database}, User: {UserId}", 
                serverInstance, databaseName, userId ?? "Integrated Security");
            connection.Dispose();
            throw;
        }
    }

    public async Task<bool> TestConnectionAsync(string serverInstance, string? databaseName = null)
    {
        try
        {
            using var connection = await GetConnectionAsync(serverInstance, databaseName);
            var result = await connection.QuerySingleAsync<int>("SELECT 1");
            return result == 1;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Connection test failed for {ServerInstance}, Database: {Database}", 
                serverInstance, databaseName ?? "master");
            return false;
        }
    }

    public async Task<IEnumerable<string>> DiscoverAvailabilityGroupListenersAsync(string serverInstance)
    {
        try
        {
            using var connection = await GetConnectionAsync(serverInstance);
            
            const string sql = @"
                SELECT 
                    agl.dns_name 
                FROM sys.availability_group_listeners agl
                    INNER JOIN sys.availability_groups ag ON agl.group_id = ag.group_id
                WHERE ag.is_primary_replica = 1";

            var listeners = await connection.QueryAsync<string>(sql);
            return listeners;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to discover AG listeners for {ServerInstance}", serverInstance);
            return Enumerable.Empty<string>();
        }
    }

    public async Task<string> GetPrimaryReplicaAsync(string serverInstance)
    {
        try
        {
            using var connection = await GetConnectionAsync(serverInstance);
            
            const string sql = @"
                SELECT 
                    ar.replica_server_name
                FROM sys.availability_replicas ar
                    INNER JOIN sys.dm_hadr_availability_replica_states ars ON ar.replica_id = ars.replica_id
                WHERE ars.role = 1"; // 1 = Primary

            var primaryReplica = await connection.QuerySingleOrDefaultAsync<string>(sql);
            return primaryReplica ?? serverInstance;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get primary replica for {ServerInstance}", serverInstance);
            return serverInstance;
        }
    }

    public async Task<bool> IsAvailabilityGroupPrimaryAsync(string serverInstance)
    {
        try
        {
            using var connection = await GetConnectionAsync(serverInstance);
            
            const string sql = @"
                SELECT COUNT(*)
                FROM sys.dm_hadr_availability_replica_states
                WHERE role = 1"; // 1 = Primary

            var count = await connection.QuerySingleAsync<int>(sql);
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to check if {ServerInstance} is AG primary, assuming standalone", serverInstance);
            return true; // Assume standalone instance
        }
    }

    public string BuildConnectionString(string serverInstance, string? databaseName = null, string? userId = null, string? password = null, int timeoutSeconds = 30, bool integratedSecurity = true)
    {
        var builder = new SqlConnectionStringBuilder
        {
            DataSource = serverInstance,
            InitialCatalog = databaseName ?? "master",
            ConnectTimeout = timeoutSeconds,
            TrustServerCertificate = true,
            ApplicationName = "IAMBuddy-SqlServerManagement"
        };

        if (integratedSecurity && string.IsNullOrEmpty(userId))
        {
            builder.IntegratedSecurity = true;
        }
        else if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(password))
        {
            builder.UserID = userId;
            builder.Password = password;
            builder.IntegratedSecurity = false;
        }
        else
        {
            // Fallback to configuration or environment variables
            var connectionStringFromConfig = _configuration.GetConnectionString($"SqlServer_{serverInstance.Replace("\\", "_")}");
            if (!string.IsNullOrEmpty(connectionStringFromConfig))
            {
                return connectionStringFromConfig;
            }
            
            builder.IntegratedSecurity = true;
        }

        return builder.ConnectionString;
    }
}