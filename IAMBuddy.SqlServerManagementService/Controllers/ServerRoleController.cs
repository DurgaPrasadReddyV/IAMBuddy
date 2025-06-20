using IAMBuddy.SqlServerManagementService.Models;
using IAMBuddy.SqlServerManagementService.Models.DTOs;
using IAMBuddy.SqlServerManagementService.Services;
using Microsoft.AspNetCore.Mvc;

namespace IAMBuddy.SqlServerManagementService.Controllers;

/// <summary>
/// Controller for managing SQL Server roles
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ServerRoleController : ControllerBase
{
    private readonly IServerRoleService _serverRoleService;

    /// <summary>
    /// Initializes a new instance of the ServerRoleController
    /// </summary>
    /// <param name="serverRoleService">The server role service</param>
    public ServerRoleController(IServerRoleService serverRoleService)
    {
        _serverRoleService = serverRoleService;
    }

    /// <summary>
    /// Gets all server roles for a specific server instance
    /// </summary>
    /// <param name="serverInstance">The SQL Server instance name</param>
    /// <returns>A list of server roles</returns>
    /// <response code="200">Returns the list of server roles</response>
    /// <response code="400">If the server instance is invalid</response>
    [HttpGet("server/{serverInstance}")]
    [ProducesResponseType(typeof(IEnumerable<ServerRoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<ServerRoleDto>>> GetServerRoles(string serverInstance)
    {
        if (string.IsNullOrWhiteSpace(serverInstance))
            return BadRequest("Server instance is required");

        var roles = await _serverRoleService.GetServerRolesAsync(serverInstance);
        var roleDtos = roles.Select(MapToDto);
        return Ok(roleDtos);
    }

    /// <summary>
    /// Gets a specific server role by name and server instance
    /// </summary>
    /// <param name="roleName">The role name</param>
    /// <param name="serverInstance">The SQL Server instance name</param>
    /// <returns>The server role</returns>
    /// <response code="200">Returns the server role</response>
    /// <response code="404">If the role is not found</response>
    /// <response code="400">If the parameters are invalid</response>
    [HttpGet("{roleName}/server/{serverInstance}")]
    [ProducesResponseType(typeof(ServerRoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ServerRoleDto>> GetServerRole(string roleName, string serverInstance)
    {
        if (string.IsNullOrWhiteSpace(roleName) || string.IsNullOrWhiteSpace(serverInstance))
            return BadRequest("Role name and server instance are required");

        var role = await _serverRoleService.GetServerRoleAsync(roleName, serverInstance);
        if (role == null)
            return NotFound($"Server role '{roleName}' not found on server '{serverInstance}'");

        return Ok(MapToDto(role));
    }

    /// <summary>
    /// Creates a new server role
    /// </summary>
    /// <param name="request">The create server role request</param>
    /// <returns>The created server role</returns>
    /// <response code="201">Returns the newly created server role</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="409">If the role already exists</response>
    [HttpPost]
    [ProducesResponseType(typeof(ServerRoleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ServerRoleDto>> CreateServerRole([FromBody] CreateServerRoleRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Check if role already exists
        if (await _serverRoleService.ServerRoleExistsAsync(request.RoleName, request.ServerInstance))
            return Conflict($"Server role '{request.RoleName}' already exists on server '{request.ServerInstance}'");

        // Validate role name
        if (!await _serverRoleService.ValidateRoleNameAsync(request.RoleName))
            return BadRequest("Invalid role name format");

        var createdRole = await _serverRoleService.CreateServerRoleAsync(
            request.RoleName, 
            request.ServerInstance);

        var roleDto = MapToDto(createdRole);
        return CreatedAtAction(nameof(GetServerRole), 
            new { roleName = createdRole.RoleName, serverInstance = createdRole.ServerInstance }, 
            roleDto);
    }

    /// <summary>
    /// Deletes a server role
    /// </summary>
    /// <param name="roleName">The role name</param>
    /// <param name="serverInstance">The SQL Server instance name</param>
    /// <returns>No content</returns>
    /// <response code="204">If the role was deleted successfully</response>
    /// <response code="400">If the parameters are invalid</response>
    /// <response code="404">If the role is not found</response>
    [HttpDelete("{roleName}/server/{serverInstance}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteServerRole(string roleName, string serverInstance)
    {
        if (string.IsNullOrWhiteSpace(roleName) || string.IsNullOrWhiteSpace(serverInstance))
            return BadRequest("Role name and server instance are required");

        if (!await _serverRoleService.ServerRoleExistsAsync(roleName, serverInstance))
            return NotFound($"Server role '{roleName}' not found on server '{serverInstance}'");

        await _serverRoleService.DeleteServerRoleAsync(roleName, serverInstance);
        return NoContent();
    }

    /// <summary>
    /// Gets all members of a server role
    /// </summary>
    /// <param name="roleName">The role name</param>
    /// <param name="serverInstance">The SQL Server instance name</param>
    /// <returns>A list of role member names</returns>
    /// <response code="200">Returns the list of role members</response>
    /// <response code="400">If the parameters are invalid</response>
    /// <response code="404">If the role is not found</response>
    [HttpGet("{roleName}/server/{serverInstance}/members")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<string>>> GetRoleMembers(string roleName, string serverInstance)
    {
        if (string.IsNullOrWhiteSpace(roleName) || string.IsNullOrWhiteSpace(serverInstance))
            return BadRequest("Role name and server instance are required");

        if (!await _serverRoleService.ServerRoleExistsAsync(roleName, serverInstance))
            return NotFound($"Server role '{roleName}' not found on server '{serverInstance}'");

        var members = await _serverRoleService.GetRoleMembersAsync(roleName, serverInstance);
        return Ok(members);
    }

    /// <summary>
    /// Adds a member to a server role
    /// </summary>
    /// <param name="request">The role member request</param>
    /// <returns>No content</returns>
    /// <response code="204">If the member was added successfully</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="404">If the role is not found</response>
    [HttpPost("members")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddRoleMember([FromBody] RoleMemberRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await _serverRoleService.ServerRoleExistsAsync(request.RoleName, request.ServerInstance))
            return NotFound($"Server role '{request.RoleName}' not found on server '{request.ServerInstance}'");

        await _serverRoleService.AddMemberToRoleAsync(request.RoleName, request.MemberName, request.ServerInstance);
        return NoContent();
    }

    /// <summary>
    /// Removes a member from a server role
    /// </summary>
    /// <param name="request">The role member request</param>
    /// <returns>No content</returns>
    /// <response code="204">If the member was removed successfully</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="404">If the role is not found</response>
    [HttpDelete("members")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveRoleMember([FromBody] RoleMemberRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await _serverRoleService.ServerRoleExistsAsync(request.RoleName, request.ServerInstance))
            return NotFound($"Server role '{request.RoleName}' not found on server '{request.ServerInstance}'");

        await _serverRoleService.RemoveMemberFromRoleAsync(request.RoleName, request.MemberName, request.ServerInstance);
        return NoContent();
    }

    /// <summary>
    /// Gets all permissions for a server role
    /// </summary>
    /// <param name="roleName">The role name</param>
    /// <param name="serverInstance">The SQL Server instance name</param>
    /// <returns>A list of permissions</returns>
    /// <response code="200">Returns the list of permissions</response>
    /// <response code="400">If the parameters are invalid</response>
    /// <response code="404">If the role is not found</response>
    [HttpGet("{roleName}/server/{serverInstance}/permissions")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<string>>> GetRolePermissions(string roleName, string serverInstance)
    {
        if (string.IsNullOrWhiteSpace(roleName) || string.IsNullOrWhiteSpace(serverInstance))
            return BadRequest("Role name and server instance are required");

        if (!await _serverRoleService.ServerRoleExistsAsync(roleName, serverInstance))
            return NotFound($"Server role '{roleName}' not found on server '{serverInstance}'");

        var permissions = await _serverRoleService.GetRolePermissionsAsync(roleName, serverInstance);
        return Ok(permissions);
    }

    /// <summary>
    /// Grants a permission to a server role
    /// </summary>
    /// <param name="request">The role permission request</param>
    /// <returns>No content</returns>
    /// <response code="204">If the permission was granted successfully</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="404">If the role is not found</response>
    [HttpPost("permissions")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GrantRolePermission([FromBody] RolePermissionRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await _serverRoleService.ServerRoleExistsAsync(request.RoleName, request.ServerInstance))
            return NotFound($"Server role '{request.RoleName}' not found on server '{request.ServerInstance}'");

        if (!await _serverRoleService.ValidatePermissionAsync(request.Permission))
            return BadRequest("Invalid permission");

        await _serverRoleService.GrantPermissionToRoleAsync(request.RoleName, request.Permission, request.ServerInstance, request.ObjectName);
        return NoContent();
    }

    /// <summary>
    /// Revokes a permission from a server role
    /// </summary>
    /// <param name="request">The role permission request</param>
    /// <returns>No content</returns>
    /// <response code="204">If the permission was revoked successfully</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="404">If the role is not found</response>
    [HttpDelete("permissions")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevokeRolePermission([FromBody] RolePermissionRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await _serverRoleService.ServerRoleExistsAsync(request.RoleName, request.ServerInstance))
            return NotFound($"Server role '{request.RoleName}' not found on server '{request.ServerInstance}'");

        if (!await _serverRoleService.ValidatePermissionAsync(request.Permission))
            return BadRequest("Invalid permission");

        await _serverRoleService.RevokePermissionFromRoleAsync(request.RoleName, request.Permission, request.ServerInstance, request.ObjectName);
        return NoContent();
    }

    /// <summary>
    /// Performs bulk operations on multiple server roles
    /// </summary>
    /// <param name="request">The bulk operation request</param>
    /// <returns>Operation results</returns>
    /// <response code="200">Returns the operation results</response>
    /// <response code="400">If the request is invalid</response>
    [HttpPost("bulk")]
    [ProducesResponseType(typeof(BulkOperationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BulkOperationResult>> BulkRoleOperation([FromBody] BulkRoleOperationRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = new BulkOperationResult();
        
        foreach (var roleName in request.RoleNames)
        {
            try
            {
                switch (request.Operation.ToLowerInvariant())
                {
                    case "delete":
                        await _serverRoleService.DeleteServerRoleAsync(roleName, request.ServerInstance);
                        result.SuccessfulItems.Add(roleName);
                        break;
                    default:
                        result.FailedItems.Add(roleName, $"Unknown operation: {request.Operation}");
                        break;
                }
            }
            catch (Exception ex)
            {
                result.FailedItems.Add(roleName, ex.Message);
            }
        }

        return Ok(result);
    }

    private static ServerRoleDto MapToDto(SqlServerRole role)
    {
        return new ServerRoleDto
        {
            Id = role.Id,
            RoleName = role.RoleName,
            RoleType = role.RoleType,
            DatabaseName = role.DatabaseName,
            ServerInstance = role.ServerInstance,
            Description = role.Description,
            IsBuiltIn = role.IsBuiltIn,
            IsEnabled = role.IsEnabled,
            CreatedDate = role.CreatedDate,
            ModifiedDate = role.ModifiedDate,
            CreatedBy = role.CreatedBy,
            ModifiedBy = role.ModifiedBy
        };
    }
}