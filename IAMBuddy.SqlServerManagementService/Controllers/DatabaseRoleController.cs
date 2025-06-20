using IAMBuddy.SqlServerManagementService.Models;
using IAMBuddy.SqlServerManagementService.Models.DTOs;
using IAMBuddy.SqlServerManagementService.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace IAMBuddy.SqlServerManagementService.Controllers;

/// <summary>
/// Controller for managing SQL Server database roles
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DatabaseRoleController : ControllerBase
{
    private readonly IDatabaseRoleService _databaseRoleService;

    /// <summary>
    /// Initializes a new instance of the DatabaseRoleController
    /// </summary>
    /// <param name="databaseRoleService">The database role service</param>
    public DatabaseRoleController(IDatabaseRoleService databaseRoleService)
    {
        _databaseRoleService = databaseRoleService;
    }

    /// <summary>
    /// Gets all database roles for a specific database and server instance
    /// </summary>
    /// <param name="databaseName">The database name</param>
    /// <param name="serverInstance">The SQL Server instance name</param>
    /// <returns>A list of database roles</returns>
    /// <response code="200">Returns the list of database roles</response>
    /// <response code="400">If the parameters are invalid</response>
    [HttpGet("database/{databaseName}/server/{serverInstance}")]
    [ProducesResponseType(typeof(IEnumerable<DatabaseRoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<DatabaseRoleDto>>> GetDatabaseRoles(string databaseName, string serverInstance)
    {
        if (string.IsNullOrWhiteSpace(databaseName) || string.IsNullOrWhiteSpace(serverInstance))
            return BadRequest("Database name and server instance are required");

        var roles = await _databaseRoleService.GetRolesAsync(databaseName, serverInstance);
        var roleDtos = roles.Select(MapToDto);
        return Ok(roleDtos);
    }

    /// <summary>
    /// Gets a specific database role
    /// </summary>
    /// <param name="roleName">The role name</param>
    /// <param name="databaseName">The database name</param>
    /// <param name="serverInstance">The SQL Server instance name</param>
    /// <returns>The database role</returns>
    /// <response code="200">Returns the database role</response>
    /// <response code="404">If the role is not found</response>
    /// <response code="400">If the parameters are invalid</response>
    [HttpGet("{roleName}/database/{databaseName}/server/{serverInstance}")]
    [ProducesResponseType(typeof(DatabaseRoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DatabaseRoleDto>> GetDatabaseRole(string roleName, string databaseName, string serverInstance)
    {
        if (string.IsNullOrWhiteSpace(roleName) || string.IsNullOrWhiteSpace(databaseName) || string.IsNullOrWhiteSpace(serverInstance))
            return BadRequest("Role name, database name, and server instance are required");

        var role = await _databaseRoleService.GetRoleAsync(roleName, databaseName, serverInstance);
        if (role == null)
            return NotFound($"Database role '{roleName}' not found in database '{databaseName}' on server '{serverInstance}'");

        return Ok(MapToDto(role));
    }

    /// <summary>
    /// Creates a new database role
    /// </summary>
    /// <param name="request">The create database role request</param>
    /// <returns>The created database role</returns>
    /// <response code="201">Returns the newly created database role</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="409">If the role already exists</response>
    [HttpPost]
    [ProducesResponseType(typeof(DatabaseRoleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<DatabaseRoleDto>> CreateDatabaseRole([FromBody] CreateDatabaseRoleRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Check if role already exists
        if (await _databaseRoleService.RoleExistsAsync(request.RoleName, request.DatabaseName, request.ServerInstance))
            return Conflict($"Database role '{request.RoleName}' already exists in database '{request.DatabaseName}' on server '{request.ServerInstance}'");

        // Validate role name
        if (!await _databaseRoleService.ValidateRoleNameAsync(request.RoleName))
            return BadRequest("Invalid role name format");

        var createdRole = await _databaseRoleService.CreateRoleAsync(
            request.RoleName, 
            request.DatabaseName, 
            request.ServerInstance);

        var roleDto = MapToDto(createdRole);
        return CreatedAtAction(nameof(GetDatabaseRole), 
            new { roleName = createdRole.RoleName, databaseName = createdRole.DatabaseName, serverInstance = createdRole.ServerInstance }, 
            roleDto);
    }

    /// <summary>
    /// Creates a new database role with initial members in a transaction
    /// </summary>
    /// <param name="request">The create database role request with initial members</param>
    /// <returns>The created database role</returns>
    /// <response code="201">Returns the newly created database role</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="409">If the role already exists</response>
    [HttpPost("with-members")]
    [ProducesResponseType(typeof(DatabaseRoleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<DatabaseRoleDto>> CreateDatabaseRoleWithMembers([FromBody] CreateDatabaseRoleWithMembersRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Check if role already exists
        if (await _databaseRoleService.RoleExistsAsync(request.RoleName, request.DatabaseName, request.ServerInstance))
            return Conflict($"Database role '{request.RoleName}' already exists in database '{request.DatabaseName}' on server '{request.ServerInstance}'");

        // Validate role name
        if (!await _databaseRoleService.ValidateRoleNameAsync(request.RoleName))
            return BadRequest("Invalid role name format");

        var createdRole = await _databaseRoleService.CreateRoleWithTransactionAsync(
            request.RoleName, 
            request.DatabaseName, 
            request.ServerInstance,
            request.InitialMembers);

        var roleDto = MapToDto(createdRole);
        return CreatedAtAction(nameof(GetDatabaseRole), 
            new { roleName = createdRole.RoleName, databaseName = createdRole.DatabaseName, serverInstance = createdRole.ServerInstance }, 
            roleDto);
    }

    /// <summary>
    /// Deletes a database role
    /// </summary>
    /// <param name="roleName">The role name</param>
    /// <param name="databaseName">The database name</param>
    /// <param name="serverInstance">The SQL Server instance name</param>
    /// <param name="forceDelete">Whether to force delete the role even if it has members</param>
    /// <returns>No content</returns>
    /// <response code="204">If the role was deleted successfully</response>
    /// <response code="400">If the parameters are invalid</response>
    /// <response code="404">If the role is not found</response>
    [HttpDelete("{roleName}/database/{databaseName}/server/{serverInstance}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDatabaseRole(string roleName, string databaseName, string serverInstance, [FromQuery] bool forceDelete = false)
    {
        if (string.IsNullOrWhiteSpace(roleName) || string.IsNullOrWhiteSpace(databaseName) || string.IsNullOrWhiteSpace(serverInstance))
            return BadRequest("Role name, database name, and server instance are required");

        if (!await _databaseRoleService.RoleExistsAsync(roleName, databaseName, serverInstance))
            return NotFound($"Database role '{roleName}' not found in database '{databaseName}' on server '{serverInstance}'");

        await _databaseRoleService.DeleteRoleWithTransactionAsync(roleName, databaseName, serverInstance, forceDelete);
        return NoContent();
    }

    /// <summary>
    /// Gets all members of a database role
    /// </summary>
    /// <param name="roleName">The role name</param>
    /// <param name="databaseName">The database name</param>
    /// <param name="serverInstance">The SQL Server instance name</param>
    /// <returns>A list of role member names</returns>
    /// <response code="200">Returns the list of role members</response>
    /// <response code="400">If the parameters are invalid</response>
    /// <response code="404">If the role is not found</response>
    [HttpGet("{roleName}/database/{databaseName}/server/{serverInstance}/members")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<string>>> GetRoleMembers(string roleName, string databaseName, string serverInstance)
    {
        if (string.IsNullOrWhiteSpace(roleName) || string.IsNullOrWhiteSpace(databaseName) || string.IsNullOrWhiteSpace(serverInstance))
            return BadRequest("Role name, database name, and server instance are required");

        if (!await _databaseRoleService.RoleExistsAsync(roleName, databaseName, serverInstance))
            return NotFound($"Database role '{roleName}' not found in database '{databaseName}' on server '{serverInstance}'");

        var members = await _databaseRoleService.GetRoleMembersAsync(roleName, databaseName, serverInstance);
        return Ok(members);
    }

    /// <summary>
    /// Adds a member to a database role
    /// </summary>
    /// <param name="request">The database role member request</param>
    /// <returns>No content</returns>
    /// <response code="204">If the member was added successfully</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="404">If the role is not found</response>
    [HttpPost("members")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddRoleMember([FromBody] DatabaseRoleMemberRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await _databaseRoleService.RoleExistsAsync(request.RoleName, request.DatabaseName, request.ServerInstance))
            return NotFound($"Database role '{request.RoleName}' not found in database '{request.DatabaseName}' on server '{request.ServerInstance}'");

        await _databaseRoleService.AddMemberToRoleAsync(request.RoleName, request.MemberName, request.DatabaseName, request.ServerInstance);
        return NoContent();
    }

    /// <summary>
    /// Removes a member from a database role
    /// </summary>
    /// <param name="request">The database role member request</param>
    /// <returns>No content</returns>
    /// <response code="204">If the member was removed successfully</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="404">If the role is not found</response>
    [HttpDelete("members")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveRoleMember([FromBody] DatabaseRoleMemberRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await _databaseRoleService.RoleExistsAsync(request.RoleName, request.DatabaseName, request.ServerInstance))
            return NotFound($"Database role '{request.RoleName}' not found in database '{request.DatabaseName}' on server '{request.ServerInstance}'");

        await _databaseRoleService.RemoveMemberFromRoleAsync(request.RoleName, request.MemberName, request.DatabaseName, request.ServerInstance);
        return NoContent();
    }

    /// <summary>
    /// Gets all permissions for a database role
    /// </summary>
    /// <param name="roleName">The role name</param>
    /// <param name="databaseName">The database name</param>
    /// <param name="serverInstance">The SQL Server instance name</param>
    /// <returns>A list of permissions</returns>
    /// <response code="200">Returns the list of permissions</response>
    /// <response code="400">If the parameters are invalid</response>
    /// <response code="404">If the role is not found</response>
    [HttpGet("{roleName}/database/{databaseName}/server/{serverInstance}/permissions")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<string>>> GetRolePermissions(string roleName, string databaseName, string serverInstance)
    {
        if (string.IsNullOrWhiteSpace(roleName) || string.IsNullOrWhiteSpace(databaseName) || string.IsNullOrWhiteSpace(serverInstance))
            return BadRequest("Role name, database name, and server instance are required");

        if (!await _databaseRoleService.RoleExistsAsync(roleName, databaseName, serverInstance))
            return NotFound($"Database role '{roleName}' not found in database '{databaseName}' on server '{serverInstance}'");

        var permissions = await _databaseRoleService.GetRolePermissionsAsync(roleName, databaseName, serverInstance);
        return Ok(permissions);
    }

    /// <summary>
    /// Grants a permission to a database role
    /// </summary>
    /// <param name="request">The database role permission request</param>
    /// <returns>No content</returns>
    /// <response code="204">If the permission was granted successfully</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="404">If the role is not found</response>
    [HttpPost("permissions")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GrantRolePermission([FromBody] DatabaseRolePermissionRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await _databaseRoleService.RoleExistsAsync(request.RoleName, request.DatabaseName, request.ServerInstance))
            return NotFound($"Database role '{request.RoleName}' not found in database '{request.DatabaseName}' on server '{request.ServerInstance}'");

        await _databaseRoleService.GrantPermissionToRoleAsync(request.RoleName, request.Permission, request.DatabaseName, request.ServerInstance, request.ObjectName);
        return NoContent();
    }

    /// <summary>
    /// Revokes a permission from a database role
    /// </summary>
    /// <param name="request">The database role permission request</param>
    /// <returns>No content</returns>
    /// <response code="204">If the permission was revoked successfully</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="404">If the role is not found</response>
    [HttpDelete("permissions")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevokeRolePermission([FromBody] DatabaseRolePermissionRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await _databaseRoleService.RoleExistsAsync(request.RoleName, request.DatabaseName, request.ServerInstance))
            return NotFound($"Database role '{request.RoleName}' not found in database '{request.DatabaseName}' on server '{request.ServerInstance}'");

        await _databaseRoleService.RevokePermissionFromRoleAsync(request.RoleName, request.Permission, request.DatabaseName, request.ServerInstance, request.ObjectName);
        return NoContent();
    }

    /// <summary>
    /// Changes the owner of a database role
    /// </summary>
    /// <param name="roleName">The role name</param>
    /// <param name="databaseName">The database name</param>
    /// <param name="serverInstance">The SQL Server instance name</param>
    /// <param name="request">The change owner request</param>
    /// <returns>No content</returns>
    /// <response code="204">If the owner was changed successfully</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="404">If the role is not found</response>
    [HttpPut("{roleName}/database/{databaseName}/server/{serverInstance}/owner")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeRoleOwner(string roleName, string databaseName, string serverInstance, [FromBody] ChangeRoleOwnerRequest request)
    {
        if (string.IsNullOrWhiteSpace(roleName) || string.IsNullOrWhiteSpace(databaseName) || string.IsNullOrWhiteSpace(serverInstance))
            return BadRequest("Role name, database name, and server instance are required");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await _databaseRoleService.RoleExistsAsync(roleName, databaseName, serverInstance))
            return NotFound($"Database role '{roleName}' not found in database '{databaseName}' on server '{serverInstance}'");

        if (!await _databaseRoleService.ValidateOwnerNameAsync(request.NewOwner))
            return BadRequest("Invalid owner name");

        await _databaseRoleService.AlterRoleOwnerAsync(roleName, request.NewOwner, databaseName, serverInstance);
        return NoContent();
    }

    /// <summary>
    /// Performs bulk operations on multiple database roles
    /// </summary>
    /// <param name="request">The bulk operation request</param>
    /// <returns>Operation results</returns>
    /// <response code="200">Returns the operation results</response>
    /// <response code="400">If the request is invalid</response>
    [HttpPost("bulk")]
    [ProducesResponseType(typeof(BulkOperationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BulkOperationResult>> BulkRoleOperation([FromBody] BulkDatabaseRoleOperationRequest request)
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
                        await _databaseRoleService.DeleteRoleAsync(roleName, request.DatabaseName, request.ServerInstance);
                        result.SuccessfulItems.Add(roleName);
                        break;
                    case "forcedelete":
                        await _databaseRoleService.DeleteRoleWithTransactionAsync(roleName, request.DatabaseName, request.ServerInstance, true);
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

    private static DatabaseRoleDto MapToDto(SqlServerRole role)
    {
        return new DatabaseRoleDto
        {
            Id = role.Id,
            RoleName = role.RoleName,
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

/// <summary>
/// Request model for creating a database role with initial members
/// </summary>
public class CreateDatabaseRoleWithMembersRequest : CreateDatabaseRoleRequest
{
    /// <summary>
    /// Gets or sets the initial members to add to the role
    /// </summary>
    public List<string>? InitialMembers { get; set; }
}

/// <summary>
/// Request model for changing role owner
/// </summary>
public class ChangeRoleOwnerRequest
{
    /// <summary>
    /// Gets or sets the new owner name
    /// </summary>
    [Required]
    [MaxLength(128)]
    public string NewOwner { get; set; } = string.Empty;
}