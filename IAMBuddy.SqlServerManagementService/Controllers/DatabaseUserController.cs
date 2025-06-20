using IAMBuddy.SqlServerManagementService.Models;
using IAMBuddy.SqlServerManagementService.Models.DTOs;
using IAMBuddy.SqlServerManagementService.Services;
using Microsoft.AspNetCore.Mvc;

namespace IAMBuddy.SqlServerManagementService.Controllers;

/// <summary>
/// Controller for managing SQL Server database users
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DatabaseUserController : ControllerBase
{
    private readonly IDatabaseUserService _databaseUserService;

    /// <summary>
    /// Initializes a new instance of the DatabaseUserController
    /// </summary>
    /// <param name="databaseUserService">The database user service</param>
    public DatabaseUserController(IDatabaseUserService databaseUserService)
    {
        _databaseUserService = databaseUserService;
    }

    /// <summary>
    /// Gets all database users for a specific database and server instance
    /// </summary>
    /// <param name="databaseName">The database name</param>
    /// <param name="serverInstance">The SQL Server instance name</param>
    /// <returns>A list of database users</returns>
    /// <response code="200">Returns the list of database users</response>
    /// <response code="400">If the parameters are invalid</response>
    [HttpGet("database/{databaseName}/server/{serverInstance}")]
    [ProducesResponseType(typeof(IEnumerable<DatabaseUserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<DatabaseUserDto>>> GetDatabaseUsers(string databaseName, string serverInstance)
    {
        if (string.IsNullOrWhiteSpace(databaseName) || string.IsNullOrWhiteSpace(serverInstance))
            return BadRequest("Database name and server instance are required");

        var users = await _databaseUserService.GetUsersAsync(databaseName, serverInstance);
        var userDtos = users.Select(MapToDto);
        return Ok(userDtos);
    }

    /// <summary>
    /// Gets database users associated with a specific login
    /// </summary>
    /// <param name="loginName">The login name</param>
    /// <param name="serverInstance">The SQL Server instance name</param>
    /// <returns>A list of database users</returns>
    /// <response code="200">Returns the list of database users</response>
    /// <response code="400">If the parameters are invalid</response>
    [HttpGet("login/{loginName}/server/{serverInstance}")]
    [ProducesResponseType(typeof(IEnumerable<DatabaseUserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<DatabaseUserDto>>> GetUsersByLogin(string loginName, string serverInstance)
    {
        if (string.IsNullOrWhiteSpace(loginName) || string.IsNullOrWhiteSpace(serverInstance))
            return BadRequest("Login name and server instance are required");

        var users = await _databaseUserService.GetUsersByLoginAsync(loginName, serverInstance);
        var userDtos = users.Select(MapToDto);
        return Ok(userDtos);
    }

    /// <summary>
    /// Gets a specific database user
    /// </summary>
    /// <param name="userName">The user name</param>
    /// <param name="databaseName">The database name</param>
    /// <param name="serverInstance">The SQL Server instance name</param>
    /// <returns>The database user</returns>
    /// <response code="200">Returns the database user</response>
    /// <response code="404">If the user is not found</response>
    /// <response code="400">If the parameters are invalid</response>
    [HttpGet("{userName}/database/{databaseName}/server/{serverInstance}")]
    [ProducesResponseType(typeof(DatabaseUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DatabaseUserDto>> GetDatabaseUser(string userName, string databaseName, string serverInstance)
    {
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(databaseName) || string.IsNullOrWhiteSpace(serverInstance))
            return BadRequest("User name, database name, and server instance are required");

        var user = await _databaseUserService.GetUserAsync(userName, databaseName, serverInstance);
        if (user == null)
            return NotFound($"Database user '{userName}' not found in database '{databaseName}' on server '{serverInstance}'");

        return Ok(MapToDto(user));
    }

    /// <summary>
    /// Creates a new database user
    /// </summary>
    /// <param name="request">The create database user request</param>
    /// <returns>The created database user</returns>
    /// <response code="201">Returns the newly created database user</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="409">If the user already exists</response>
    [HttpPost]
    [ProducesResponseType(typeof(DatabaseUserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<DatabaseUserDto>> CreateDatabaseUser([FromBody] CreateDatabaseUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Check if user already exists
        if (await _databaseUserService.UserExistsAsync(request.UserName, request.DatabaseName, request.ServerInstance))
            return Conflict($"Database user '{request.UserName}' already exists in database '{request.DatabaseName}' on server '{request.ServerInstance}'");

        // Validate user name
        if (!await _databaseUserService.ValidateUserNameAsync(request.UserName))
            return BadRequest("Invalid user name format");

        // Validate schema name if provided
        if (!string.IsNullOrEmpty(request.DefaultSchema) && !await _databaseUserService.ValidateSchemaNameAsync(request.DefaultSchema))
            return BadRequest("Invalid schema name format");

        SqlServerUser createdUser;
        
        if (string.IsNullOrEmpty(request.LoginName))
        {
            createdUser = await _databaseUserService.CreateUserWithoutLoginAsync(
                request.UserName, 
                request.DatabaseName, 
                request.ServerInstance, 
                request.DefaultSchema);
        }
        else
        {
            createdUser = await _databaseUserService.CreateUserAsync(
                request.UserName, 
                request.LoginName, 
                request.DatabaseName, 
                request.ServerInstance, 
                request.DefaultSchema);
        }

        var userDto = MapToDto(createdUser);
        return CreatedAtAction(nameof(GetDatabaseUser), 
            new { userName = createdUser.UserName, databaseName = createdUser.DatabaseName, serverInstance = createdUser.ServerInstance }, 
            userDto);
    }

    /// <summary>
    /// Updates an existing database user
    /// </summary>
    /// <param name="userName">The user name</param>
    /// <param name="databaseName">The database name</param>
    /// <param name="serverInstance">The SQL Server instance name</param>
    /// <param name="request">The update database user request</param>
    /// <returns>No content</returns>
    /// <response code="204">If the user was updated successfully</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="404">If the user is not found</response>
    [HttpPut("{userName}/database/{databaseName}/server/{serverInstance}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDatabaseUser(string userName, string databaseName, string serverInstance, [FromBody] UpdateDatabaseUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(databaseName) || string.IsNullOrWhiteSpace(serverInstance))
            return BadRequest("User name, database name, and server instance are required");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Check if user exists
        if (!await _databaseUserService.UserExistsAsync(userName, databaseName, serverInstance))
            return NotFound($"Database user '{userName}' not found in database '{databaseName}' on server '{serverInstance}'");

        // Handle default schema change
        if (!string.IsNullOrEmpty(request.DefaultSchema))
        {
            if (!await _databaseUserService.ValidateSchemaNameAsync(request.DefaultSchema))
                return BadRequest("Invalid schema name format");
            
            await _databaseUserService.AlterUserDefaultSchemaAsync(userName, request.DefaultSchema, databaseName, serverInstance);
        }

        return NoContent();
    }

    /// <summary>
    /// Deletes a database user
    /// </summary>
    /// <param name="userName">The user name</param>
    /// <param name="databaseName">The database name</param>
    /// <param name="serverInstance">The SQL Server instance name</param>
    /// <returns>No content</returns>
    /// <response code="204">If the user was deleted successfully</response>
    /// <response code="400">If the parameters are invalid</response>
    /// <response code="404">If the user is not found</response>
    [HttpDelete("{userName}/database/{databaseName}/server/{serverInstance}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDatabaseUser(string userName, string databaseName, string serverInstance)
    {
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(databaseName) || string.IsNullOrWhiteSpace(serverInstance))
            return BadRequest("User name, database name, and server instance are required");

        if (!await _databaseUserService.UserExistsAsync(userName, databaseName, serverInstance))
            return NotFound($"Database user '{userName}' not found in database '{databaseName}' on server '{serverInstance}'");

        await _databaseUserService.DeleteUserAsync(userName, databaseName, serverInstance);
        return NoContent();
    }

    /// <summary>
    /// Gets all database roles assigned to a user
    /// </summary>
    /// <param name="userName">The user name</param>
    /// <param name="databaseName">The database name</param>
    /// <param name="serverInstance">The SQL Server instance name</param>
    /// <returns>A list of database role names</returns>
    /// <response code="200">Returns the list of database roles</response>
    /// <response code="400">If the parameters are invalid</response>
    /// <response code="404">If the user is not found</response>
    [HttpGet("{userName}/database/{databaseName}/server/{serverInstance}/roles")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<string>>> GetUserRoles(string userName, string databaseName, string serverInstance)
    {
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(databaseName) || string.IsNullOrWhiteSpace(serverInstance))
            return BadRequest("User name, database name, and server instance are required");

        if (!await _databaseUserService.UserExistsAsync(userName, databaseName, serverInstance))
            return NotFound($"Database user '{userName}' not found in database '{databaseName}' on server '{serverInstance}'");

        var roles = await _databaseUserService.GetUserRolesAsync(userName, databaseName, serverInstance);
        return Ok(roles);
    }

    /// <summary>
    /// Adds a user to a database role
    /// </summary>
    /// <param name="request">The database role member request</param>
    /// <returns>No content</returns>
    /// <response code="204">If the user was added to the role successfully</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="404">If the user is not found</response>
    [HttpPost("roles")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddUserToRole([FromBody] DatabaseRoleMemberRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await _databaseUserService.UserExistsAsync(request.MemberName, request.DatabaseName, request.ServerInstance))
            return NotFound($"Database user '{request.MemberName}' not found in database '{request.DatabaseName}' on server '{request.ServerInstance}'");

        await _databaseUserService.AddToRoleAsync(request.MemberName, request.RoleName, request.DatabaseName, request.ServerInstance);
        return NoContent();
    }

    /// <summary>
    /// Removes a user from a database role
    /// </summary>
    /// <param name="request">The database role member request</param>
    /// <returns>No content</returns>
    /// <response code="204">If the user was removed from the role successfully</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="404">If the user is not found</response>
    [HttpDelete("roles")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveUserFromRole([FromBody] DatabaseRoleMemberRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await _databaseUserService.UserExistsAsync(request.MemberName, request.DatabaseName, request.ServerInstance))
            return NotFound($"Database user '{request.MemberName}' not found in database '{request.DatabaseName}' on server '{request.ServerInstance}'");

        await _databaseUserService.RemoveFromRoleAsync(request.MemberName, request.RoleName, request.DatabaseName, request.ServerInstance);
        return NoContent();
    }

    /// <summary>
    /// Gets all permissions for a database user
    /// </summary>
    /// <param name="userName">The user name</param>
    /// <param name="databaseName">The database name</param>
    /// <param name="serverInstance">The SQL Server instance name</param>
    /// <returns>A list of permissions</returns>
    /// <response code="200">Returns the list of permissions</response>
    /// <response code="400">If the parameters are invalid</response>
    /// <response code="404">If the user is not found</response>
    [HttpGet("{userName}/database/{databaseName}/server/{serverInstance}/permissions")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<string>>> GetUserPermissions(string userName, string databaseName, string serverInstance)
    {
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(databaseName) || string.IsNullOrWhiteSpace(serverInstance))
            return BadRequest("User name, database name, and server instance are required");

        if (!await _databaseUserService.UserExistsAsync(userName, databaseName, serverInstance))
            return NotFound($"Database user '{userName}' not found in database '{databaseName}' on server '{serverInstance}'");

        var permissions = await _databaseUserService.GetUserPermissionsAsync(userName, databaseName, serverInstance);
        return Ok(permissions);
    }

    /// <summary>
    /// Grants a permission to a database user
    /// </summary>
    /// <param name="request">The database role permission request</param>
    /// <returns>No content</returns>
    /// <response code="204">If the permission was granted successfully</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="404">If the user is not found</response>
    [HttpPost("permissions")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GrantUserPermission([FromBody] DatabaseRolePermissionRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Note: Using MemberName from the request as the user name for permission operations
        var userName = request.RoleName; // This should be changed to a dedicated user permission request DTO
        
        if (!await _databaseUserService.UserExistsAsync(userName, request.DatabaseName, request.ServerInstance))
            return NotFound($"Database user '{userName}' not found in database '{request.DatabaseName}' on server '{request.ServerInstance}'");

        await _databaseUserService.GrantPermissionAsync(userName, request.Permission, request.DatabaseName, request.ServerInstance, request.ObjectName);
        return NoContent();
    }

    /// <summary>
    /// Revokes a permission from a database user
    /// </summary>
    /// <param name="request">The database role permission request</param>
    /// <returns>No content</returns>
    /// <response code="204">If the permission was revoked successfully</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="404">If the user is not found</response>
    [HttpDelete("permissions")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevokeUserPermission([FromBody] DatabaseRolePermissionRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Note: Using MemberName from the request as the user name for permission operations
        var userName = request.RoleName; // This should be changed to a dedicated user permission request DTO
        
        if (!await _databaseUserService.UserExistsAsync(userName, request.DatabaseName, request.ServerInstance))
            return NotFound($"Database user '{userName}' not found in database '{request.DatabaseName}' on server '{request.ServerInstance}'");

        await _databaseUserService.RevokePermissionAsync(userName, request.Permission, request.DatabaseName, request.ServerInstance, request.ObjectName);
        return NoContent();
    }

    /// <summary>
    /// Performs bulk operations on multiple database users
    /// </summary>
    /// <param name="request">The bulk operation request</param>
    /// <returns>Operation results</returns>
    /// <response code="200">Returns the operation results</response>
    /// <response code="400">If the request is invalid</response>
    [HttpPost("bulk")]
    [ProducesResponseType(typeof(BulkOperationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BulkOperationResult>> BulkUserOperation([FromBody] BulkUserOperationRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = new BulkOperationResult();
        
        foreach (var userName in request.UserNames)
        {
            try
            {
                switch (request.Operation.ToLowerInvariant())
                {
                    case "delete":
                        await _databaseUserService.DeleteUserAsync(userName, request.DatabaseName, request.ServerInstance);
                        result.SuccessfulItems.Add(userName);
                        break;
                    default:
                        result.FailedItems.Add(userName, $"Unknown operation: {request.Operation}");
                        break;
                }
            }
            catch (Exception ex)
            {
                result.FailedItems.Add(userName, ex.Message);
            }
        }

        return Ok(result);
    }

    private static DatabaseUserDto MapToDto(SqlServerUser user)
    {
        return new DatabaseUserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            DatabaseName = user.DatabaseName,
            ServerInstance = user.ServerInstance,
            Sid = user.Sid,
            IsEnabled = user.IsEnabled,
            UserType = user.UserType,
            DefaultSchema = user.DefaultSchema,
            LoginId = user.LoginId,
            LoginName = user.Login?.LoginName,
            CreatedDate = user.CreatedDate,
            ModifiedDate = user.ModifiedDate,
            CreatedBy = user.CreatedBy,
            ModifiedBy = user.ModifiedBy
        };
    }
}