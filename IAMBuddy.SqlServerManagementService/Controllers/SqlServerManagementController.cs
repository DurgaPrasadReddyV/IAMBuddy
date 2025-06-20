using IAMBuddy.SqlServerManagementService.Models;
using IAMBuddy.SqlServerManagementService.Services;
using Microsoft.AspNetCore.Mvc;

namespace IAMBuddy.SqlServerManagementService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SqlServerManagementController : ControllerBase
{
    private readonly ISqlServerManagementService _managementService;
    private readonly ISqlServerOperationService _operationService;

    public SqlServerManagementController(
        ISqlServerManagementService managementService,
        ISqlServerOperationService operationService)
    {
        _managementService = managementService;
        _operationService = operationService;
    }

    // Login endpoints
    [HttpGet("logins/{serverInstance}")]
    public async Task<ActionResult<IEnumerable<SqlServerLogin>>> GetLogins(string serverInstance)
    {
        var logins = await _managementService.GetLoginsAsync(serverInstance);
        return Ok(logins);
    }

    [HttpGet("logins/{id:int}")]
    public async Task<ActionResult<SqlServerLogin>> GetLogin(int id)
    {
        var login = await _managementService.GetLoginAsync(id);
        if (login == null)
            return NotFound();

        return Ok(login);
    }

    [HttpPost("logins")]
    public async Task<ActionResult<SqlServerLogin>> CreateLogin([FromBody] SqlServerLogin login)
    {
        var createdLogin = await _managementService.CreateLoginAsync(login);
        return CreatedAtAction(nameof(GetLogin), new { id = createdLogin.Id }, createdLogin);
    }

    // User endpoints
    [HttpGet("users/{serverInstance}")]
    public async Task<ActionResult<IEnumerable<SqlServerUser>>> GetUsers(string serverInstance, [FromQuery] string? databaseName = null)
    {
        var users = await _managementService.GetUsersAsync(serverInstance, databaseName);
        return Ok(users);
    }

    [HttpGet("users/{id:int}")]
    public async Task<ActionResult<SqlServerUser>> GetUser(int id)
    {
        var user = await _managementService.GetUserAsync(id);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPost("users")]
    public async Task<ActionResult<SqlServerUser>> CreateUser([FromBody] SqlServerUser user)
    {
        var createdUser = await _managementService.CreateUserAsync(user);
        return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
    }

    // Role endpoints
    [HttpGet("roles/{serverInstance}")]
    public async Task<ActionResult<IEnumerable<SqlServerRole>>> GetRoles(string serverInstance, [FromQuery] string? databaseName = null)
    {
        var roles = await _managementService.GetRolesAsync(serverInstance, databaseName);
        return Ok(roles);
    }

    [HttpGet("roles/{id:int}")]
    public async Task<ActionResult<SqlServerRole>> GetRole(int id)
    {
        var role = await _managementService.GetRoleAsync(id);
        if (role == null)
            return NotFound();

        return Ok(role);
    }

    [HttpPost("roles")]
    public async Task<ActionResult<SqlServerRole>> CreateRole([FromBody] SqlServerRole role)
    {
        var createdRole = await _managementService.CreateRoleAsync(role);
        return CreatedAtAction(nameof(GetRole), new { id = createdRole.Id }, createdRole);
    }

    // Role assignment endpoints
    [HttpGet("role-assignments/{serverInstance}")]
    public async Task<ActionResult<IEnumerable<SqlServerRoleAssignment>>> GetRoleAssignments(string serverInstance, [FromQuery] string? databaseName = null)
    {
        var assignments = await _managementService.GetRoleAssignmentsAsync(serverInstance, databaseName);
        return Ok(assignments);
    }

    [HttpPost("role-assignments")]
    public async Task<ActionResult<SqlServerRoleAssignment>> CreateRoleAssignment([FromBody] SqlServerRoleAssignment assignment)
    {
        var createdAssignment = await _managementService.CreateRoleAssignmentAsync(assignment);
        return CreatedAtAction(nameof(GetRoleAssignments), new { serverInstance = createdAssignment.ServerInstance }, createdAssignment);
    }

    // Operation endpoints
    [HttpGet("operations")]
    public async Task<ActionResult<IEnumerable<SqlServerOperation>>> GetOperations([FromQuery] string? status = null, [FromQuery] string? requestId = null)
    {
        var operations = await _operationService.GetOperationsAsync(status, requestId);
        return Ok(operations);
    }

    [HttpGet("operations/{id:int}")]
    public async Task<ActionResult<SqlServerOperation>> GetOperation(int id)
    {
        var operation = await _operationService.GetOperationAsync(id);
        if (operation == null)
            return NotFound();

        return Ok(operation);
    }

    [HttpPost("operations")]
    public async Task<ActionResult<SqlServerOperation>> CreateOperation([FromBody] SqlServerOperation operation)
    {
        var createdOperation = await _operationService.CreateOperationAsync(operation);
        return CreatedAtAction(nameof(GetOperation), new { id = createdOperation.Id }, createdOperation);
    }

    [HttpPut("operations/{id:int}/status")]
    public async Task<ActionResult<SqlServerOperation>> UpdateOperationStatus(int id, [FromBody] UpdateOperationStatusRequest request)
    {
        try
        {
            var operation = await _operationService.UpdateOperationStatusAsync(id, request.Status, request.ErrorMessage);
            return Ok(operation);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPut("operations/{id:int}/complete")]
    public async Task<ActionResult<SqlServerOperation>> CompleteOperation(int id, [FromBody] CompleteOperationRequest request)
    {
        try
        {
            var operation = await _operationService.CompleteOperationAsync(id, request.Success, request.ErrorMessage);
            return Ok(operation);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }
}

public record UpdateOperationStatusRequest(string Status, string? ErrorMessage = null);
public record CompleteOperationRequest(bool Success, string? ErrorMessage = null);