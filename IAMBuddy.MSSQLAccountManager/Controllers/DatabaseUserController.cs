using IAMBuddy.MSSQLAccountManager.Models;
using IAMBuddy.MSSQLAccountManager.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IAMBuddy.MSSQLAccountManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatabaseUserController : ControllerBase
    {
        private readonly IDatabaseUserService _databaseUserService;
        private readonly ILogger<DatabaseUserController> _logger;

        public DatabaseUserController(IDatabaseUserService databaseUserService, ILogger<DatabaseUserController> logger)
        {
            _databaseUserService = databaseUserService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<OperationResult>> CreateUser([FromBody] CreateDatabaseUserRequest request)
        {
            try
            {
                var user = new DatabaseUser
                {
                    UserName = request.UserName,
                    ServerName = request.ServerName,
                    InstanceName = request.InstanceName,
                    DatabaseName = request.DatabaseName,
                    UserType = request.UserType,
                    SqlLoginId = request.SqlLoginId,
                    LoginName = request.LoginName,
                    DefaultSchema = request.DefaultSchema ?? "dbo",
                    Description = request.Description,
                    CreatedBy = request.CreatedBy ?? "API"
                };

                var result = await _databaseUserService.CreateUserAsync(user);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating database user {UserName}", request.UserName);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{userId}")]
        public async Task<ActionResult<OperationResult>> UpdateUser(Guid userId, [FromBody] UpdateDatabaseUserRequest request)
        {
            try
            {
                var updatedUser = new DatabaseUser
                {
                    DefaultSchema = request.DefaultSchema,
                    Description = request.Description,
                    UpdatedBy = request.UpdatedBy ?? "API"
                };

                var result = await _databaseUserService.UpdateUserAsync(userId, updatedUser);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating database user {UserId}", userId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult<OperationResult>> DeleteUser(Guid userId)
        {
            try
            {
                var result = await _databaseUserService.DeleteUserAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting database user {UserId}", userId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<DatabaseUser>> GetUser(Guid userId)
        {
            try
            {
                var user = await _databaseUserService.GetUserByIdAsync(userId);
                if (user == null)
                    return NotFound($"Database user with ID {userId} not found");

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving database user {UserId}", userId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("by-name")]
        public async Task<ActionResult<DatabaseUser>> GetUserByName([FromQuery] string serverName, 
            [FromQuery] string? instanceName, [FromQuery] string databaseName, [FromQuery] string userName)
        {
            try
            {
                var user = await _databaseUserService.GetUserByNameAsync(serverName, instanceName, databaseName, userName);
                if (user == null)
                    return NotFound($"Database user {userName} not found in database {databaseName}");

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving database user {UserName} in {DatabaseName}", userName, databaseName);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DatabaseUser>>> GetAllUsers([FromQuery] string serverName, 
            [FromQuery] string? instanceName = null, [FromQuery] string? databaseName = null)
        {
            try
            {
                var users = await _databaseUserService.GetAllUsersAsync(serverName, instanceName, databaseName);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving database users for server {ServerName}", serverName);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("by-login/{loginId}")]
        public async Task<ActionResult<IEnumerable<DatabaseUser>>> GetUsersByLogin(Guid loginId)
        {
            try
            {
                var users = await _databaseUserService.GetUsersByLoginAsync(loginId);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving database users for login {LoginId}", loginId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

    public class CreateDatabaseUserRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string ServerName { get; set; } = string.Empty;
        public string? InstanceName { get; set; }
        public string DatabaseName { get; set; } = string.Empty;
        public UserType UserType { get; set; }
        public Guid? SqlLoginId { get; set; }
        public string? LoginName { get; set; }
        public string? DefaultSchema { get; set; } = "dbo";
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class UpdateDatabaseUserRequest
    {
        public string? DefaultSchema { get; set; }
        public string? Description { get; set; }
        public string? UpdatedBy { get; set; }
    }
}