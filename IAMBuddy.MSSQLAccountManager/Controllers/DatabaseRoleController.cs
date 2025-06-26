using IAMBuddy.MSSQLAccountManager.Models;
using IAMBuddy.MSSQLAccountManager.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IAMBuddy.MSSQLAccountManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatabaseRoleController : ControllerBase
    {
        private readonly IDatabaseRoleService _databaseRoleService;
        private readonly ILogger<DatabaseRoleController> _logger;

        public DatabaseRoleController(IDatabaseRoleService databaseRoleService, ILogger<DatabaseRoleController> logger)
        {
            _databaseRoleService = databaseRoleService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<OperationResult>> CreateRole([FromBody] CreateDatabaseRoleRequest request)
        {
            try
            {
                var role = new DatabaseRole
                {
                    RoleName = request.RoleName,
                    ServerName = request.ServerName,
                    InstanceName = request.InstanceName,
                    DatabaseName = request.DatabaseName,
                    Description = request.Description,
                    CreatedBy = request.CreatedBy ?? "API"
                };

                var result = await _databaseRoleService.CreateRoleAsync(role);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating database role {RoleName}", request.RoleName);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{roleId}")]
        public async Task<ActionResult<OperationResult>> UpdateRole(Guid roleId, [FromBody] UpdateDatabaseRoleRequest request)
        {
            try
            {
                var updatedRole = new DatabaseRole
                {
                    Description = request.Description,
                    UpdatedBy = request.UpdatedBy ?? "API"
                };

                var result = await _databaseRoleService.UpdateRoleAsync(roleId, updatedRole);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating database role {RoleId}", roleId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{roleId}")]
        public async Task<ActionResult<OperationResult>> DeleteRole(Guid roleId)
        {
            try
            {
                var result = await _databaseRoleService.DeleteRoleAsync(roleId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting database role {RoleId}", roleId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{roleId}")]
        public async Task<ActionResult<DatabaseRole>> GetRole(Guid roleId)
        {
            try
            {
                var role = await _databaseRoleService.GetRoleByIdAsync(roleId);
                if (role == null)
                    return NotFound($"Database role with ID {roleId} not found");

                return Ok(role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving database role {RoleId}", roleId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("by-name")]
        public async Task<ActionResult<DatabaseRole>> GetRoleByName([FromQuery] string serverName, 
            [FromQuery] string? instanceName, [FromQuery] string databaseName, [FromQuery] string roleName)
        {
            try
            {
                var role = await _databaseRoleService.GetRoleByNameAsync(serverName, instanceName, databaseName, roleName);
                if (role == null)
                    return NotFound($"Database role {roleName} not found in database {databaseName}");

                return Ok(role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving database role {RoleName} in {DatabaseName}", roleName, databaseName);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DatabaseRole>>> GetAllRoles([FromQuery] string serverName, 
            [FromQuery] string? instanceName = null, [FromQuery] string? databaseName = null)
        {
            try
            {
                var roles = await _databaseRoleService.GetAllRolesAsync(serverName, instanceName, databaseName);
                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving database roles for server {ServerName}", serverName);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("{roleId}/assign-to-user/{userId}")]
        public async Task<ActionResult<OperationResult>> AssignRoleToUser(Guid roleId, Guid userId)
        {
            try
            {
                var result = await _databaseRoleService.AssignRoleToUserAsync(userId, roleId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning role {RoleId} to user {UserId}", roleId, userId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{roleId}/remove-from-user/{userId}")]
        public async Task<ActionResult<OperationResult>> RemoveRoleFromUser(Guid roleId, Guid userId)
        {
            try
            {
                var result = await _databaseRoleService.RemoveRoleFromUserAsync(userId, roleId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing role {RoleId} from user {UserId}", roleId, userId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("assignments/by-user/{userId}")]
        public async Task<ActionResult<IEnumerable<DatabaseRoleAssignment>>> GetUserRoleAssignments(Guid userId)
        {
            try
            {
                var assignments = await _databaseRoleService.GetUserRoleAssignmentsAsync(userId);
                return Ok(assignments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving role assignments for user {UserId}", userId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("assignments/by-role/{roleId}")]
        public async Task<ActionResult<IEnumerable<DatabaseRoleAssignment>>> GetRoleAssignments(Guid roleId)
        {
            try
            {
                var assignments = await _databaseRoleService.GetRoleAssignmentsAsync(roleId);
                return Ok(assignments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving assignments for role {RoleId}", roleId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

    public class CreateDatabaseRoleRequest
    {
        public string RoleName { get; set; } = string.Empty;
        public string ServerName { get; set; } = string.Empty;
        public string? InstanceName { get; set; }
        public string DatabaseName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class UpdateDatabaseRoleRequest
    {
        public string? Description { get; set; }
        public string? UpdatedBy { get; set; }
    }
}