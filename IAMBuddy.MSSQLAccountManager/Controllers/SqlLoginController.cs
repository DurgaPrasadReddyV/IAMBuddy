using IAMBuddy.MSSQLAccountManager.Models;
using IAMBuddy.MSSQLAccountManager.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IAMBuddy.MSSQLAccountManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SqlLoginController : ControllerBase
    {
        private readonly ISqlLoginService _sqlLoginService;
        private readonly ILogger<SqlLoginController> _logger;

        public SqlLoginController(ISqlLoginService sqlLoginService, ILogger<SqlLoginController> logger)
        {
            _sqlLoginService = sqlLoginService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<OperationResult>> CreateLogin([FromBody] CreateSqlLoginRequest request)
        {
            try
            {
                var login = new SqlLogin
                {
                    LoginName = request.LoginName,
                    ServerName = request.ServerName,
                    InstanceName = request.InstanceName,
                    LoginType = request.LoginType,
                    DefaultDatabase = request.DefaultDatabase,
                    DefaultLanguage = request.DefaultLanguage,
                    IsPasswordPolicyEnforced = request.IsPasswordPolicyEnforced,
                    Description = request.Description,
                    CreatedBy = request.CreatedBy ?? "API"
                };

                var result = await _sqlLoginService.CreateLoginAsync(login, request.Password);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating SQL login {LoginName}", request.LoginName);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{loginId}")]
        public async Task<ActionResult<OperationResult>> UpdateLogin(Guid loginId, [FromBody] UpdateSqlLoginRequest request)
        {
            try
            {
                var updatedLogin = new SqlLogin
                {
                    DefaultDatabase = request.DefaultDatabase,
                    DefaultLanguage = request.DefaultLanguage,
                    Description = request.Description,
                    UpdatedBy = request.UpdatedBy ?? "API"
                };

                var result = await _sqlLoginService.UpdateLoginAsync(loginId, updatedLogin);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating SQL login {LoginId}", loginId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{loginId}")]
        public async Task<ActionResult<OperationResult>> DeleteLogin(Guid loginId)
        {
            try
            {
                var result = await _sqlLoginService.DeleteLoginAsync(loginId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting SQL login {LoginId}", loginId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{loginId}")]
        public async Task<ActionResult<SqlLogin>> GetLogin(Guid loginId)
        {
            try
            {
                var login = await _sqlLoginService.GetLoginByIdAsync(loginId);
                if (login == null)
                    return NotFound($"SQL login with ID {loginId} not found");

                return Ok(login);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving SQL login {LoginId}", loginId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("by-name")]
        public async Task<ActionResult<SqlLogin>> GetLoginByName([FromQuery] string serverName, 
            [FromQuery] string? instanceName, [FromQuery] string loginName)
        {
            try
            {
                var login = await _sqlLoginService.GetLoginByNameAsync(serverName, instanceName, loginName);
                if (login == null)
                    return NotFound($"SQL login {loginName} not found on server {serverName}");

                return Ok(login);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving SQL login {LoginName} on {ServerName}", loginName, serverName);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SqlLogin>>> GetAllLogins([FromQuery] string serverName, 
            [FromQuery] string? instanceName = null)
        {
            try
            {
                var logins = await _sqlLoginService.GetAllLoginsAsync(serverName, instanceName);
                return Ok(logins);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving SQL logins for server {ServerName}", serverName);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPatch("{loginId}/enable")]
        public async Task<ActionResult<OperationResult>> EnableLogin(Guid loginId)
        {
            try
            {
                var result = await _sqlLoginService.EnableLoginAsync(loginId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enabling SQL login {LoginId}", loginId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPatch("{loginId}/disable")]
        public async Task<ActionResult<OperationResult>> DisableLogin(Guid loginId)
        {
            try
            {
                var result = await _sqlLoginService.DisableLoginAsync(loginId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disabling SQL login {LoginId}", loginId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPatch("{loginId}/change-password")]
        public async Task<ActionResult<OperationResult>> ChangePassword(Guid loginId, [FromBody] ChangePasswordRequest request)
        {
            try
            {
                var result = await _sqlLoginService.ChangePasswordAsync(loginId, request.NewPassword);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for SQL login {LoginId}", loginId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

    public class CreateSqlLoginRequest
    {
        public string LoginName { get; set; } = string.Empty;
        public string ServerName { get; set; } = string.Empty;
        public string? InstanceName { get; set; }
        public LoginType LoginType { get; set; }
        public string? Password { get; set; }
        public string? DefaultDatabase { get; set; }
        public string? DefaultLanguage { get; set; }
        public bool IsPasswordPolicyEnforced { get; set; } = true;
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class UpdateSqlLoginRequest
    {
        public string? DefaultDatabase { get; set; }
        public string? DefaultLanguage { get; set; }
        public string? Description { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string NewPassword { get; set; } = string.Empty;
    }
}