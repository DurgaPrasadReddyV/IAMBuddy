using IAMBuddy.MSSQLAccountManager.Models;
using IAMBuddy.MSSQLAccountManager.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IAMBuddy.MSSQLAccountManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServerRoleController : ControllerBase
    {
        private readonly IServerRoleService _serverRoleService;
        private readonly ILogger<ServerRoleController> _logger;

        public ServerRoleController(IServerRoleService serverRoleService, ILogger<ServerRoleController> logger)
        {
            _serverRoleService = serverRoleService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<OperationResult>> CreateRole([FromBody] CreateServerRoleRequest request)
        {
            try
            {
                var role = new ServerRole
                {
                    RoleName = request.RoleName,
                    ServerName = request.ServerName,
                    InstanceName = request.InstanceName,
                    Description = request.Description,
                    CreatedBy = request.CreatedBy ?? "API"
                };

                var result = await _serverRoleService.CreateRoleAsync(role);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating server role {RoleName}", request.RoleName);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{roleId}")]
        public async Task<ActionResult<OperationResult>> UpdateRole(Guid roleId, [FromBody] UpdateServerRoleRequest request)
        {
            try
            {
                var updatedRole = new ServerRole
                {
                    Description = request.Description,
                    UpdatedBy = request.UpdatedBy ?? "API"
                };

                var result = await _serverRoleService.UpdateRoleAsync(roleId, updatedRole);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating server role {RoleId}", roleId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{roleId}")]
        public async Task<ActionResult<OperationResult>> DeleteRole(Guid roleId)
        {
            try
            {
                var result = await _serverRoleService.DeleteRoleAsync(roleId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting server role {RoleId}", roleId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{roleId}")]
        public async Task<ActionResult<ServerRole>> GetRole(Guid roleId)
        {
            try
            {
                var role = await _serverRoleService.GetRoleByIdAsync(roleId);
                if (role == null)
                    return NotFound($"Server role with ID {roleId} not found");

                return Ok(role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving server role {RoleId}", roleId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("by-name")]
        public async Task<ActionResult<ServerRole>> GetRoleByName([FromQuery] string serverName, 
            [FromQuery] string? instanceName, [FromQuery] string roleName)
        {
            try
            {
                var role = await _serverRoleService.GetRoleByNameAsync(serverName, instanceName, roleName);
                if (role == null)
                    return NotFound($"Server role {roleName} not found on server {serverName}");

                return Ok(role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving server role {RoleName} on {ServerName}", roleName, serverName);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServerRole>>> GetAllRoles([FromQuery] string serverName, 
            [FromQuery] string? instanceName = null)
        {
            try
            {
                var roles = await _serverRoleService.GetAllRolesAsync(serverName, instanceName);
                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving server roles for server {ServerName}", serverName);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("{roleId}/assign-to-login/{loginId}")]
        public async Task<ActionResult<OperationResult>> AssignRoleToLogin(Guid roleId, Guid loginId)
        {
            try
            {
                var result = await _serverRoleService.AssignRoleToLoginAsync(loginId, roleId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning role {RoleId} to login {LoginId}", roleId, loginId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{roleId}/remove-from-login/{loginId}")]
        public async Task<ActionResult<OperationResult>> RemoveRoleFromLogin(Guid roleId, Guid loginId)
        {
            try
            {
                var result = await _serverRoleService.RemoveRoleFromLoginAsync(loginId, roleId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing role {RoleId} from login {LoginId}", roleId, loginId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("assignments/by-login/{loginId}")]
        public async Task<ActionResult<IEnumerable<ServerRoleAssignment>>> GetLoginRoleAssignments(Guid loginId)
        {
            try
            {
                var assignments = await _serverRoleService.GetLoginRoleAssignmentsAsync(loginId);
                return Ok(assignments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving role assignments for login {LoginId}", loginId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("assignments/by-role/{roleId}")]
        public async Task<ActionResult<IEnumerable<ServerRoleAssignment>>> GetRoleAssignments(Guid roleId)
        {
            try
            {
                var assignments = await _serverRoleService.GetRoleAssignmentsAsync(roleId);
                return Ok(assignments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving assignments for role {RoleId}", roleId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

    public class CreateServerRoleRequest
    {
        public string RoleName { get; set; } = string.Empty;
        public string ServerName { get; set; } = string.Empty;
        public string? InstanceName { get; set; }
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class UpdateServerRoleRequest
    {
        public string? Description { get; set; }
        public string? UpdatedBy { get; set; }
    }
}