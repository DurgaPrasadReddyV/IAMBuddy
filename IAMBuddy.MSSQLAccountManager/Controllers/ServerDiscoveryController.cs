using IAMBuddy.MSSQLAccountManager.Models;
using IAMBuddy.MSSQLAccountManager.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IAMBuddy.MSSQLAccountManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServerDiscoveryController : ControllerBase
    {
        private readonly IServerDiscoveryService _serverDiscoveryService;
        private readonly ILogger<ServerDiscoveryController> _logger;

        public ServerDiscoveryController(IServerDiscoveryService serverDiscoveryService, ILogger<ServerDiscoveryController> logger)
        {
            _serverDiscoveryService = serverDiscoveryService;
            _logger = logger;
        }

        [HttpGet("discover")]
        public async Task<ActionResult<IEnumerable<ServerInstance>>> DiscoverServers()
        {
            try
            {
                var servers = await _serverDiscoveryService.DiscoverServersAsync();
                return Ok(servers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error discovering servers");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("availability-group/{listenerName}")]
        public async Task<ActionResult<IEnumerable<ServerInstance>>> GetAvailabilityGroupInstances(string listenerName)
        {
            try
            {
                var instances = await _serverDiscoveryService.GetAvailabilityGroupInstancesAsync(listenerName);
                return Ok(instances);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting availability group instances for {ListenerName}", listenerName);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<OperationResult>> RegisterServerInstance([FromBody] RegisterServerInstanceRequest request)
        {
            try
            {
                var serverInstance = new ServerInstance
                {
                    ServerName = request.ServerName,
                    InstanceName = request.InstanceName,
                    ConnectionString = request.ConnectionString,
                    Port = request.Port,
                    IsAvailabilityGroupListener = request.IsAvailabilityGroupListener,
                    AvailabilityGroupName = request.AvailabilityGroupName,
                    Description = request.Description,
                    CreatedBy = request.CreatedBy ?? "API"
                };

                var result = await _serverDiscoveryService.RegisterServerInstanceAsync(serverInstance);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering server instance {ServerName}", request.ServerName);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{instanceId}")]
        public async Task<ActionResult<OperationResult>> UpdateServerInstance(Guid instanceId, [FromBody] UpdateServerInstanceRequest request)
        {
            try
            {
                var updatedInstance = new ServerInstance
                {
                    ConnectionString = request.ConnectionString,
                    Port = request.Port,
                    IsActive = request.IsActive,
                    Description = request.Description,
                    UpdatedBy = request.UpdatedBy ?? "API"
                };

                var result = await _serverDiscoveryService.UpdateServerInstanceAsync(instanceId, updatedInstance);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating server instance {InstanceId}", instanceId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{instanceId}")]
        public async Task<ActionResult<OperationResult>> RemoveServerInstance(Guid instanceId)
        {
            try
            {
                var result = await _serverDiscoveryService.RemoveServerInstanceAsync(instanceId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing server instance {InstanceId}", instanceId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{instanceId}")]
        public async Task<ActionResult<ServerInstance>> GetServerInstance(Guid instanceId)
        {
            try
            {
                var instance = await _serverDiscoveryService.GetServerInstanceAsync("", null);
                if (instance == null)
                    return NotFound($"Server instance with ID {instanceId} not found");

                return Ok(instance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving server instance {InstanceId}", instanceId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("by-name")]
        public async Task<ActionResult<ServerInstance>> GetServerInstanceByName([FromQuery] string serverName, 
            [FromQuery] string? instanceName = null)
        {
            try
            {
                var instance = await _serverDiscoveryService.GetServerInstanceAsync(serverName, instanceName);
                if (instance == null)
                    return NotFound($"Server instance {serverName}\\{instanceName} not found");

                return Ok(instance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving server instance {ServerName}\\{InstanceName}", serverName, instanceName);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServerInstance>>> GetAllServerInstances()
        {
            try
            {
                var instances = await _serverDiscoveryService.GetAllServerInstancesAsync();
                return Ok(instances);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all server instances");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("{instanceId}/test-connection")]
        public async Task<ActionResult<OperationResult>> TestConnection(Guid instanceId)
        {
            try
            {
                var result = await _serverDiscoveryService.TestConnectionAsync(instanceId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing connection for instance {InstanceId}", instanceId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("{instanceId}/health-check")]
        public async Task<ActionResult<OperationResult>> PerformHealthCheck(Guid instanceId)
        {
            try
            {
                var result = await _serverDiscoveryService.PerformHealthCheckAsync(instanceId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error performing health check for instance {InstanceId}", instanceId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

    public class RegisterServerInstanceRequest
    {
        public string ServerName { get; set; } = string.Empty;
        public string? InstanceName { get; set; }
        public string ConnectionString { get; set; } = string.Empty;
        public int Port { get; set; } = 1433;
        public bool IsAvailabilityGroupListener { get; set; } = false;
        public string? AvailabilityGroupName { get; set; }
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class UpdateServerInstanceRequest
    {
        public string ConnectionString { get; set; } = string.Empty;
        public int Port { get; set; } = 1433;
        public bool IsActive { get; set; } = true;
        public string? Description { get; set; }
        public string? UpdatedBy { get; set; }
    }
}