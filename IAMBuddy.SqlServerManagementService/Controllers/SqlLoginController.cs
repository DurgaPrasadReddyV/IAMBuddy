using IAMBuddy.SqlServerManagementService.Models;
using IAMBuddy.SqlServerManagementService.Models.DTOs;
using IAMBuddy.SqlServerManagementService.Services;
using Microsoft.AspNetCore.Mvc;

namespace IAMBuddy.SqlServerManagementService.Controllers;

/// <summary>
/// Controller for managing SQL Server logins
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SqlLoginController : ControllerBase
{
    private readonly ISqlLoginService _sqlLoginService;

    /// <summary>
    /// Initializes a new instance of the SqlLoginController
    /// </summary>
    /// <param name="sqlLoginService">The SQL login service</param>
    public SqlLoginController(ISqlLoginService sqlLoginService)
    {
        _sqlLoginService = sqlLoginService;
    }

    /// <summary>
    /// Gets all logins for a specific server instance
    /// </summary>
    /// <param name="serverInstance">The SQL Server instance name</param>
    /// <returns>A list of SQL Server logins</returns>
    /// <response code="200">Returns the list of logins</response>
    /// <response code="400">If the server instance is invalid</response>
    [HttpGet("server/{serverInstance}")]
    [ProducesResponseType(typeof(IEnumerable<SqlLoginDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<SqlLoginDto>>> GetLogins(string serverInstance)
    {
        if (string.IsNullOrWhiteSpace(serverInstance))
            return BadRequest("Server instance is required");

        var logins = await _sqlLoginService.GetLoginsAsync(serverInstance);
        var loginDtos = logins.Select(MapToDto);
        return Ok(loginDtos);
    }

    /// <summary>
    /// Gets a specific login by name and server instance
    /// </summary>
    /// <param name="loginName">The login name</param>
    /// <param name="serverInstance">The SQL Server instance name</param>
    /// <returns>The SQL Server login</returns>
    /// <response code="200">Returns the login</response>
    /// <response code="404">If the login is not found</response>
    /// <response code="400">If the parameters are invalid</response>
    [HttpGet("{loginName}/server/{serverInstance}")]
    [ProducesResponseType(typeof(SqlLoginDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SqlLoginDto>> GetLogin(string loginName, string serverInstance)
    {
        if (string.IsNullOrWhiteSpace(loginName) || string.IsNullOrWhiteSpace(serverInstance))
            return BadRequest("Login name and server instance are required");

        var login = await _sqlLoginService.GetLoginAsync(loginName, serverInstance);
        if (login == null)
            return NotFound($"Login '{loginName}' not found on server '{serverInstance}'");

        return Ok(MapToDto(login));
    }

    /// <summary>
    /// Creates a new SQL Server login
    /// </summary>
    /// <param name="request">The create login request</param>
    /// <returns>The created login</returns>
    /// <response code="201">Returns the newly created login</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="409">If the login already exists</response>
    [HttpPost]
    [ProducesResponseType(typeof(SqlLoginDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SqlLoginDto>> CreateLogin([FromBody] CreateSqlLoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Check if login already exists
        if (await _sqlLoginService.LoginExistsAsync(request.LoginName, request.ServerInstance))
            return Conflict($"Login '{request.LoginName}' already exists on server '{request.ServerInstance}'");

        // Validate login name
        if (!await _sqlLoginService.ValidateLoginNameAsync(request.LoginName))
            return BadRequest("Invalid login name format");

        // Validate password for SQL logins
        if (request.LoginType.Equals("SQL", StringComparison.OrdinalIgnoreCase) && 
            !string.IsNullOrEmpty(request.Password) && 
            !await _sqlLoginService.ValidatePasswordAsync(request.Password))
            return BadRequest("Password does not meet complexity requirements");

        SqlServerLogin createdLogin;
        
        if (request.LoginType.Equals("Windows", StringComparison.OrdinalIgnoreCase))
        {
            createdLogin = await _sqlLoginService.CreateWindowsLoginAsync(
                request.LoginName, 
                request.ServerInstance, 
                request.DefaultDatabase);
        }
        else
        {
            createdLogin = await _sqlLoginService.CreateLoginAsync(
                request.LoginName, 
                request.Password ?? string.Empty, 
                request.ServerInstance, 
                request.DefaultDatabase);
        }

        var loginDto = MapToDto(createdLogin);
        return CreatedAtAction(nameof(GetLogin), 
            new { loginName = createdLogin.LoginName, serverInstance = createdLogin.ServerInstance }, 
            loginDto);
    }

    /// <summary>
    /// Updates an existing SQL Server login
    /// </summary>
    /// <param name="loginName">The login name</param>
    /// <param name="serverInstance">The SQL Server instance name</param>
    /// <param name="request">The update login request</param>
    /// <returns>No content</returns>
    /// <response code="204">If the login was updated successfully</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="404">If the login is not found</response>
    [HttpPut("{loginName}/server/{serverInstance}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateLogin(string loginName, string serverInstance, [FromBody] UpdateSqlLoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(loginName) || string.IsNullOrWhiteSpace(serverInstance))
            return BadRequest("Login name and server instance are required");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Check if login exists
        if (!await _sqlLoginService.LoginExistsAsync(loginName, serverInstance))
            return NotFound($"Login '{loginName}' not found on server '{serverInstance}'");

        // Handle enable/disable
        if (request.IsEnabled.HasValue)
        {
            if (request.IsEnabled.Value)
                await _sqlLoginService.EnableLoginAsync(loginName, serverInstance);
            else
                await _sqlLoginService.DisableLoginAsync(loginName, serverInstance);
        }

        // Handle password change
        if (!string.IsNullOrEmpty(request.NewPassword))
        {
            if (!await _sqlLoginService.ValidatePasswordAsync(request.NewPassword))
                return BadRequest("Password does not meet complexity requirements");
            
            await _sqlLoginService.ChangePasswordAsync(loginName, request.NewPassword, serverInstance);
        }

        return NoContent();
    }

    /// <summary>
    /// Deletes a SQL Server login
    /// </summary>
    /// <param name="loginName">The login name</param>
    /// <param name="serverInstance">The SQL Server instance name</param>
    /// <returns>No content</returns>
    /// <response code="204">If the login was deleted successfully</response>
    /// <response code="400">If the parameters are invalid</response>
    /// <response code="404">If the login is not found</response>
    [HttpDelete("{loginName}/server/{serverInstance}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteLogin(string loginName, string serverInstance)
    {
        if (string.IsNullOrWhiteSpace(loginName) || string.IsNullOrWhiteSpace(serverInstance))
            return BadRequest("Login name and server instance are required");

        if (!await _sqlLoginService.LoginExistsAsync(loginName, serverInstance))
            return NotFound($"Login '{loginName}' not found on server '{serverInstance}'");

        await _sqlLoginService.DeleteLoginAsync(loginName, serverInstance);
        return NoContent();
    }

    /// <summary>
    /// Gets all server roles assigned to a login
    /// </summary>
    /// <param name="loginName">The login name</param>
    /// <param name="serverInstance">The SQL Server instance name</param>
    /// <returns>A list of server role names</returns>
    /// <response code="200">Returns the list of server roles</response>
    /// <response code="400">If the parameters are invalid</response>
    /// <response code="404">If the login is not found</response>
    [HttpGet("{loginName}/server/{serverInstance}/roles")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<string>>> GetLoginServerRoles(string loginName, string serverInstance)
    {
        if (string.IsNullOrWhiteSpace(loginName) || string.IsNullOrWhiteSpace(serverInstance))
            return BadRequest("Login name and server instance are required");

        if (!await _sqlLoginService.LoginExistsAsync(loginName, serverInstance))
            return NotFound($"Login '{loginName}' not found on server '{serverInstance}'");

        var roles = await _sqlLoginService.GetServerRolesForLoginAsync(loginName, serverInstance);
        return Ok(roles);
    }

    /// <summary>
    /// Adds a login to a server role
    /// </summary>
    /// <param name="loginName">The login name</param>
    /// <param name="roleName">The server role name</param>
    /// <param name="serverInstance">The SQL Server instance name</param>
    /// <returns>No content</returns>
    /// <response code="204">If the login was added to the role successfully</response>
    /// <response code="400">If the parameters are invalid</response>
    /// <response code="404">If the login is not found</response>
    [HttpPost("{loginName}/server/{serverInstance}/roles/{roleName}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddLoginToServerRole(string loginName, string roleName, string serverInstance)
    {
        if (string.IsNullOrWhiteSpace(loginName) || string.IsNullOrWhiteSpace(roleName) || string.IsNullOrWhiteSpace(serverInstance))
            return BadRequest("Login name, role name, and server instance are required");

        if (!await _sqlLoginService.LoginExistsAsync(loginName, serverInstance))
            return NotFound($"Login '{loginName}' not found on server '{serverInstance}'");

        await _sqlLoginService.AddToServerRoleAsync(loginName, roleName, serverInstance);
        return NoContent();
    }

    /// <summary>
    /// Removes a login from a server role
    /// </summary>
    /// <param name="loginName">The login name</param>
    /// <param name="roleName">The server role name</param>
    /// <param name="serverInstance">The SQL Server instance name</param>
    /// <returns>No content</returns>
    /// <response code="204">If the login was removed from the role successfully</response>
    /// <response code="400">If the parameters are invalid</response>
    /// <response code="404">If the login is not found</response>
    [HttpDelete("{loginName}/server/{serverInstance}/roles/{roleName}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveLoginFromServerRole(string loginName, string roleName, string serverInstance)
    {
        if (string.IsNullOrWhiteSpace(loginName) || string.IsNullOrWhiteSpace(roleName) || string.IsNullOrWhiteSpace(serverInstance))
            return BadRequest("Login name, role name, and server instance are required");

        if (!await _sqlLoginService.LoginExistsAsync(loginName, serverInstance))
            return NotFound($"Login '{loginName}' not found on server '{serverInstance}'");

        await _sqlLoginService.RemoveFromServerRoleAsync(loginName, roleName, serverInstance);
        return NoContent();
    }

    /// <summary>
    /// Performs bulk operations on multiple logins
    /// </summary>
    /// <param name="request">The bulk operation request</param>
    /// <returns>Operation results</returns>
    /// <response code="200">Returns the operation results</response>
    /// <response code="400">If the request is invalid</response>
    [HttpPost("bulk")]
    [ProducesResponseType(typeof(BulkOperationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BulkOperationResult>> BulkLoginOperation([FromBody] BulkLoginOperationRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = new BulkOperationResult();
        
        foreach (var loginName in request.LoginNames)
        {
            try
            {
                switch (request.Operation.ToLowerInvariant())
                {
                    case "enable":
                        await _sqlLoginService.EnableLoginAsync(loginName, request.ServerInstance);
                        result.SuccessfulItems.Add(loginName);
                        break;
                    case "disable":
                        await _sqlLoginService.DisableLoginAsync(loginName, request.ServerInstance);
                        result.SuccessfulItems.Add(loginName);
                        break;
                    case "delete":
                        await _sqlLoginService.DeleteLoginAsync(loginName, request.ServerInstance);
                        result.SuccessfulItems.Add(loginName);
                        break;
                    default:
                        result.FailedItems.Add(loginName, $"Unknown operation: {request.Operation}");
                        break;
                }
            }
            catch (Exception ex)
            {
                result.FailedItems.Add(loginName, ex.Message);
            }
        }

        return Ok(result);
    }

    private static SqlLoginDto MapToDto(SqlServerLogin login)
    {
        return new SqlLoginDto
        {
            Id = login.Id,
            LoginName = login.LoginName,
            LoginType = login.LoginType,
            Sid = login.Sid,
            IsEnabled = login.IsEnabled,
            IsLocked = login.IsLocked,
            PasswordExpiryDate = login.PasswordExpiryDate,
            LastLoginDate = login.LastLoginDate,
            ServerInstance = login.ServerInstance,
            CreatedDate = login.CreatedDate,
            ModifiedDate = login.ModifiedDate,
            CreatedBy = login.CreatedBy,
            ModifiedBy = login.ModifiedBy
        };
    }
}

/// <summary>
/// Result of a bulk operation
/// </summary>
public class BulkOperationResult
{
    /// <summary>
    /// Gets or sets the list of items that were processed successfully
    /// </summary>
    public List<string> SuccessfulItems { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the dictionary of failed items with their error messages
    /// </summary>
    public Dictionary<string, string> FailedItems { get; set; } = new();
    
    /// <summary>
    /// Gets the total number of items processed
    /// </summary>
    public int TotalItems => SuccessfulItems.Count + FailedItems.Count;
    
    /// <summary>
    /// Gets the number of successful items
    /// </summary>
    public int SuccessCount => SuccessfulItems.Count;
    
    /// <summary>
    /// Gets the number of failed items
    /// </summary>
    public int FailureCount => FailedItems.Count;
}