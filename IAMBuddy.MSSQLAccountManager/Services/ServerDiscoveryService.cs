using IAMBuddy.MSSQLAccountManager.Data;
using IAMBuddy.MSSQLAccountManager.Models;
using IAMBuddy.MSSQLAccountManager.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Dapper;

namespace IAMBuddy.MSSQLAccountManager.Services
{
    public class ServerDiscoveryService : IServerDiscoveryService
    {
        private readonly MSSQLAccountManagerContext _context;
        private readonly IAuditService _auditService;
        private readonly ILogger<ServerDiscoveryService> _logger;

        public ServerDiscoveryService(
            MSSQLAccountManagerContext context,
            IAuditService auditService,
            ILogger<ServerDiscoveryService> logger)
        {
            _context = context;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<IEnumerable<ServerInstance>> DiscoverServersAsync()
        {
            try
            {
                var servers = new List<ServerInstance>();
                
                // This would typically use SQL Server Browser Service or network discovery
                // For now, we'll return registered servers from our database
                return await _context.ServerInstances.Where(si => si.IsActive).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to discover servers");
                throw;
            }
        }

        public async Task<IEnumerable<ServerInstance>> GetAvailabilityGroupInstancesAsync(string listenerName)
        {
            try
            {
                var listener = await _context.ServerInstances
                    .FirstOrDefaultAsync(si => si.ServerName == listenerName && si.IsAvailabilityGroupListener);

                if (listener == null)
                {
                    return new List<ServerInstance>();
                }

                // Get AG replicas using the listener connection
                using var connection = new SqlConnection(listener.ConnectionString);
                var sql = @"
                    SELECT 
                        ar.replica_server_name,
                        ar.availability_mode_desc,
                        ar.failover_mode_desc,
                        ars.role_desc,
                        ars.connected_state_desc,
                        ars.synchronization_health_desc
                    FROM sys.availability_replicas ar
                    JOIN sys.dm_hadr_availability_replica_states ars ON ar.replica_id = ars.replica_id
                    WHERE ar.group_id = (
                        SELECT group_id 
                        FROM sys.availability_groups 
                        WHERE name = @AgName
                    )";

                var replicas = await connection.QueryAsync(sql, new { AgName = listener.AvailabilityGroupName });
                
                var instances = new List<ServerInstance>();
                foreach (var replica in replicas)
                {
                    var instance = new ServerInstance
                    {
                        ServerName = replica.replica_server_name,
                        IsAvailabilityGroupListener = false,
                        AvailabilityGroupName = listener.AvailabilityGroupName,
                        IsActive = replica.connected_state_desc == "CONNECTED",
                        HealthStatus = $"Role: {replica.role_desc}, Health: {replica.synchronization_health_desc}",
                        LastHealthCheck = DateTime.UtcNow,
                        Description = $"AG Replica - {replica.availability_mode_desc}, {replica.failover_mode_desc}",
                        CreatedBy = "System"
                    };
                    instances.Add(instance);
                }

                return instances;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get availability group instances for listener {ListenerName}", listenerName);
                throw;
            }
        }

        public async Task<OperationResult> RegisterServerInstanceAsync(ServerInstance serverInstance)
        {
            var operation = await _auditService.LogOperationAsync(
                OperationType.Create, "ServerInstance", serverInstance.ServerName, serverInstance.ServerName, 
                null, null, serverInstance.CreatedBy);

            try
            {
                // Check if instance already exists
                var existing = await GetServerInstanceAsync(serverInstance.ServerName, serverInstance.InstanceName);
                if (existing != null)
                {
                    await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Failed, 
                        $"Server instance '{serverInstance.ServerName}\\{serverInstance.InstanceName}' already registered");
                    return operation;
                }

                // Test connection before registering
                var testResult = await TestConnectionInternalAsync(serverInstance);
                if (!testResult.Success)
                {
                    await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Failed, 
                        $"Connection test failed: {testResult.ErrorMessage}");
                    return operation;
                }

                _context.ServerInstances.Add(serverInstance);
                await _context.SaveChangesAsync();

                await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Success, 
                    null, $"Server instance registered successfully");

                return operation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register server instance {ServerName}", serverInstance.ServerName);
                
                await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Failed, 
                    ex.Message, ex.ToString());
                
                return operation;
            }
        }

        public async Task<OperationResult> UpdateServerInstanceAsync(Guid instanceId, ServerInstance updatedInstance)
        {
            var operation = await _auditService.LogOperationAsync(
                OperationType.Update, "ServerInstance", updatedInstance.ServerName, updatedInstance.ServerName, 
                null, null, updatedInstance.UpdatedBy);

            try
            {
                var existing = await _context.ServerInstances.FindAsync(instanceId);
                if (existing == null)
                {
                    await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Failed, 
                        $"Server instance with ID '{instanceId}' not found");
                    return operation;
                }

                existing.ConnectionString = updatedInstance.ConnectionString;
                existing.Port = updatedInstance.Port;
                existing.IsActive = updatedInstance.IsActive;
                existing.Description = updatedInstance.Description;
                existing.UpdatedBy = updatedInstance.UpdatedBy;
                existing.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Success, 
                    null, "Server instance updated successfully");

                return operation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update server instance {InstanceId}", instanceId);
                
                await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Failed, 
                    ex.Message, ex.ToString());
                
                return operation;
            }
        }

        public async Task<OperationResult> RemoveServerInstanceAsync(Guid instanceId)
        {
            var instance = await _context.ServerInstances.FindAsync(instanceId);
            if (instance == null)
            {
                var operation = await _auditService.LogOperationAsync(
                    OperationType.Delete, "ServerInstance", "Unknown", "Unknown");
                await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Failed, 
                    $"Server instance with ID '{instanceId}' not found");
                return operation;
            }

            var deleteOperation = await _auditService.LogOperationAsync(
                OperationType.Delete, "ServerInstance", instance.ServerName, instance.ServerName);

            try
            {
                _context.ServerInstances.Remove(instance);
                await _context.SaveChangesAsync();

                await _auditService.UpdateOperationResultAsync(deleteOperation.Id, OperationStatus.Success, 
                    null, "Server instance removed successfully");

                return deleteOperation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove server instance {InstanceId}", instanceId);
                
                await _auditService.UpdateOperationResultAsync(deleteOperation.Id, OperationStatus.Failed, 
                    ex.Message, ex.ToString());
                
                return deleteOperation;
            }
        }

        public async Task<ServerInstance?> GetServerInstanceAsync(string serverName, string? instanceName = null)
        {
            return await _context.ServerInstances
                .FirstOrDefaultAsync(si => si.ServerName == serverName && si.InstanceName == instanceName);
        }

        public async Task<IEnumerable<ServerInstance>> GetAllServerInstancesAsync()
        {
            return await _context.ServerInstances.ToListAsync();
        }

        public async Task<OperationResult> TestConnectionAsync(Guid instanceId)
        {
            var instance = await _context.ServerInstances.FindAsync(instanceId);
            if (instance == null)
            {
                var operation = await _auditService.LogOperationAsync(
                    OperationType.Get, "ServerInstance", "Unknown", "Unknown");
                await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Failed, 
                    $"Server instance with ID '{instanceId}' not found");
                return operation;
            }

            var testOperation = await _auditService.LogOperationAsync(
                OperationType.Get, "ServerInstance", instance.ServerName, instance.ServerName, 
                null, "Connection test");

            try
            {
                var result = await TestConnectionInternalAsync(instance);
                
                if (result.Success)
                {
                    await _auditService.UpdateOperationResultAsync(testOperation.Id, OperationStatus.Success, 
                        null, "Connection test successful");
                }
                else
                {
                    await _auditService.UpdateOperationResultAsync(testOperation.Id, OperationStatus.Failed, 
                        result.ErrorMessage, result.Details);
                }

                return testOperation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to test connection for instance {InstanceId}", instanceId);
                
                await _auditService.UpdateOperationResultAsync(testOperation.Id, OperationStatus.Failed, 
                    ex.Message, ex.ToString());
                
                return testOperation;
            }
        }

        public async Task<OperationResult> PerformHealthCheckAsync(Guid instanceId)
        {
            var instance = await _context.ServerInstances.FindAsync(instanceId);
            if (instance == null)
            {
                var operation = await _auditService.LogOperationAsync(
                    OperationType.Get, "ServerInstance", "Unknown", "Unknown");
                await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Failed, 
                    $"Server instance with ID '{instanceId}' not found");
                return operation;
            }

            var healthOperation = await _auditService.LogOperationAsync(
                OperationType.Get, "ServerInstance", instance.ServerName, instance.ServerName, 
                null, "Health check");

            try
            {
                using var connection = new SqlConnection(instance.ConnectionString);
                
                // Basic health check queries
                var serverInfo = await connection.QueryFirstOrDefaultAsync(@"
                    SELECT 
                        @@SERVERNAME as ServerName,
                        @@VERSION as Version,
                        @@SERVICENAME as ServiceName,
                        GETDATE() as CurrentTime
                ");

                var healthStatus = $"Server: {serverInfo.ServerName}, Version: {serverInfo.Version}";
                
                // Update health status
                instance.LastHealthCheck = DateTime.UtcNow;
                instance.HealthStatus = healthStatus;
                await _context.SaveChangesAsync();

                await _auditService.UpdateOperationResultAsync(healthOperation.Id, OperationStatus.Success, 
                    null, healthStatus);

                return healthOperation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed for instance {InstanceId}", instanceId);
                
                instance.LastHealthCheck = DateTime.UtcNow;
                instance.HealthStatus = $"Health check failed: {ex.Message}";
                await _context.SaveChangesAsync();

                await _auditService.UpdateOperationResultAsync(healthOperation.Id, OperationStatus.Failed, 
                    ex.Message, ex.ToString());
                
                return healthOperation;
            }
        }

        private async Task<(bool Success, string? ErrorMessage, string? Details)> TestConnectionInternalAsync(ServerInstance instance)
        {
            try
            {
                using var connection = new SqlConnection(instance.ConnectionString);
                await connection.OpenAsync();
                
                var result = await connection.QueryFirstOrDefaultAsync("SELECT 1");
                return (true, null, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, ex.ToString());
            }
        }
    }
}