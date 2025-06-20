using System.Text.RegularExpressions;
using IAMBuddy.SqlServerManagementService.Exceptions;
using IAMBuddy.SqlServerManagementService.Models;
using IAMBuddy.SqlServerManagementService.Repositories;
using Microsoft.Extensions.Logging;

namespace IAMBuddy.SqlServerManagementService.Services;

public class SqlLoginService : ISqlLoginService
{
    private readonly ISqlLoginRepository _loginRepository;
    private readonly IDatabaseUserRepository _userRepository;
    private readonly ILogger<SqlLoginService> _logger;

    public SqlLoginService(
        ISqlLoginRepository loginRepository,
        IDatabaseUserRepository userRepository,
        ILogger<SqlLoginService> logger)
    {
        _loginRepository = loginRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<SqlServerLogin> CreateLoginAsync(string loginName, string password, string serverInstance, string? defaultDatabase = null, string? createdBy = null)
    {
        try
        {
            _logger.LogInformation("Creating SQL login {LoginName} on server {ServerInstance}", loginName, serverInstance);
            
            await ValidateLoginCreationAsync(loginName, password, serverInstance);

            var success = await _loginRepository.CreateLoginAsync(loginName, password, serverInstance, defaultDatabase);
            if (!success)
            {
                throw new SqlServerLoginException($"Failed to create login '{loginName}' on server '{serverInstance}'");
            }

            var login = await _loginRepository.GetLoginAsync(loginName, serverInstance);
            if (login == null)
            {
                throw new SqlServerLoginException($"Login '{loginName}' was created but could not be retrieved");
            }

            _logger.LogInformation("Successfully created SQL login {LoginName} on server {ServerInstance}", loginName, serverInstance);
            return login;
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Unexpected error creating login {LoginName} on server {ServerInstance}", loginName, serverInstance);
            throw new SqlServerLoginException($"Unexpected error creating login '{loginName}': {ex.Message}", ex);
        }
    }

    public async Task<SqlServerLogin> CreateWindowsLoginAsync(string loginName, string serverInstance, string? defaultDatabase = null, string? createdBy = null)
    {
        try
        {
            _logger.LogInformation("Creating Windows login {LoginName} on server {ServerInstance}", loginName, serverInstance);
            
            await ValidateWindowsLoginCreationAsync(loginName, serverInstance);

            var success = await _loginRepository.CreateWindowsLoginAsync(loginName, serverInstance, defaultDatabase);
            if (!success)
            {
                throw new SqlServerLoginException($"Failed to create Windows login '{loginName}' on server '{serverInstance}'");
            }

            var login = await _loginRepository.GetLoginAsync(loginName, serverInstance);
            if (login == null)
            {
                throw new SqlServerLoginException($"Windows login '{loginName}' was created but could not be retrieved");
            }

            _logger.LogInformation("Successfully created Windows login {LoginName} on server {ServerInstance}", loginName, serverInstance);
            return login;
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Unexpected error creating Windows login {LoginName} on server {ServerInstance}", loginName, serverInstance);
            throw new SqlServerLoginException($"Unexpected error creating Windows login '{loginName}': {ex.Message}", ex);
        }
    }

    public async Task<SqlServerLogin?> GetLoginAsync(string loginName, string serverInstance)
    {
        try
        {
            ValidateLoginName(loginName);
            ValidateServerInstance(serverInstance);

            return await _loginRepository.GetLoginAsync(loginName, serverInstance);
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error retrieving login {LoginName} from server {ServerInstance}", loginName, serverInstance);
            throw new SqlServerLoginException($"Error retrieving login '{loginName}': {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<SqlServerLogin>> GetLoginsAsync(string serverInstance)
    {
        try
        {
            ValidateServerInstance(serverInstance);
            return await _loginRepository.GetLoginsAsync(serverInstance);
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error retrieving logins from server {ServerInstance}", serverInstance);
            throw new SqlServerLoginException($"Error retrieving logins from server '{serverInstance}': {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteLoginAsync(string loginName, string serverInstance, string? deletedBy = null)
    {
        try
        {
            _logger.LogInformation("Deleting login {LoginName} from server {ServerInstance}", loginName, serverInstance);
            
            ValidateLoginName(loginName);
            ValidateServerInstance(serverInstance);

            var loginExists = await _loginRepository.LoginExistsAsync(loginName, serverInstance);
            if (!loginExists)
            {
                _logger.LogWarning("Login {LoginName} does not exist on server {ServerInstance}", loginName, serverInstance);
                return false;
            }

            var users = await _userRepository.GetUsersAsync("", serverInstance);
            var associatedUsers = users.Where(u => u.Login?.LoginName == loginName).ToList();
            
            if (associatedUsers.Any())
            {
                _logger.LogInformation("Cascading delete: removing {UserCount} database users associated with login {LoginName}", 
                    associatedUsers.Count, loginName);
                
                foreach (var user in associatedUsers)
                {
                    await _userRepository.DropUserAsync(user.UserName, user.DatabaseName, serverInstance);
                    _logger.LogDebug("Removed user {UserName} from database {DatabaseName}", user.UserName, user.DatabaseName);
                }
            }

            var success = await _loginRepository.DropLoginAsync(loginName, serverInstance);
            if (success)
            {
                _logger.LogInformation("Successfully deleted login {LoginName} from server {ServerInstance}", loginName, serverInstance);
            }
            else
            {
                _logger.LogWarning("Failed to delete login {LoginName} from server {ServerInstance}", loginName, serverInstance);
            }

            return success;
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Unexpected error deleting login {LoginName} from server {ServerInstance}", loginName, serverInstance);
            throw new SqlServerLoginException($"Unexpected error deleting login '{loginName}': {ex.Message}", ex);
        }
    }

    public async Task<bool> EnableLoginAsync(string loginName, string serverInstance, string? modifiedBy = null)
    {
        try
        {
            _logger.LogInformation("Enabling login {LoginName} on server {ServerInstance}", loginName, serverInstance);
            
            ValidateLoginName(loginName);
            ValidateServerInstance(serverInstance);

            var success = await _loginRepository.EnableLoginAsync(loginName, serverInstance);
            if (success)
            {
                _logger.LogInformation("Successfully enabled login {LoginName} on server {ServerInstance}", loginName, serverInstance);
            }

            return success;
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error enabling login {LoginName} on server {ServerInstance}", loginName, serverInstance);
            throw new SqlServerLoginException($"Error enabling login '{loginName}': {ex.Message}", ex);
        }
    }

    public async Task<bool> DisableLoginAsync(string loginName, string serverInstance, string? modifiedBy = null)
    {
        try
        {
            _logger.LogInformation("Disabling login {LoginName} on server {ServerInstance}", loginName, serverInstance);
            
            ValidateLoginName(loginName);
            ValidateServerInstance(serverInstance);

            var success = await _loginRepository.DisableLoginAsync(loginName, serverInstance);
            if (success)
            {
                _logger.LogInformation("Successfully disabled login {LoginName} on server {ServerInstance}", loginName, serverInstance);
            }

            return success;
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error disabling login {LoginName} on server {ServerInstance}", loginName, serverInstance);
            throw new SqlServerLoginException($"Error disabling login '{loginName}': {ex.Message}", ex);
        }
    }

    public async Task<bool> ChangePasswordAsync(string loginName, string newPassword, string serverInstance, string? modifiedBy = null)
    {
        try
        {
            _logger.LogInformation("Changing password for login {LoginName} on server {ServerInstance}", loginName, serverInstance);
            
            ValidateLoginName(loginName);
            await ValidatePasswordAsync(newPassword);
            ValidateServerInstance(serverInstance);

            var success = await _loginRepository.ChangePasswordAsync(loginName, newPassword, serverInstance);
            if (success)
            {
                _logger.LogInformation("Successfully changed password for login {LoginName} on server {ServerInstance}", loginName, serverInstance);
            }

            return success;
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error changing password for login {LoginName} on server {ServerInstance}", loginName, serverInstance);
            throw new SqlServerLoginException($"Error changing password for login '{loginName}': {ex.Message}", ex);
        }
    }

    public async Task<bool> AddToServerRoleAsync(string loginName, string roleName, string serverInstance, string? modifiedBy = null)
    {
        try
        {
            _logger.LogInformation("Adding login {LoginName} to server role {RoleName} on server {ServerInstance}", 
                loginName, roleName, serverInstance);
            
            ValidateLoginName(loginName);
            ValidateRoleName(roleName);
            ValidateServerInstance(serverInstance);

            var success = await _loginRepository.AddToServerRoleAsync(loginName, roleName, serverInstance);
            if (success)
            {
                _logger.LogInformation("Successfully added login {LoginName} to server role {RoleName} on server {ServerInstance}", 
                    loginName, roleName, serverInstance);
            }

            return success;
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error adding login {LoginName} to server role {RoleName} on server {ServerInstance}", 
                loginName, roleName, serverInstance);
            throw new SqlServerLoginException($"Error adding login '{loginName}' to server role '{roleName}': {ex.Message}", ex);
        }
    }

    public async Task<bool> RemoveFromServerRoleAsync(string loginName, string roleName, string serverInstance, string? modifiedBy = null)
    {
        try
        {
            _logger.LogInformation("Removing login {LoginName} from server role {RoleName} on server {ServerInstance}", 
                loginName, roleName, serverInstance);
            
            ValidateLoginName(loginName);
            ValidateRoleName(roleName);
            ValidateServerInstance(serverInstance);

            var success = await _loginRepository.RemoveFromServerRoleAsync(loginName, roleName, serverInstance);
            if (success)
            {
                _logger.LogInformation("Successfully removed login {LoginName} from server role {RoleName} on server {ServerInstance}", 
                    loginName, roleName, serverInstance);
            }

            return success;
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error removing login {LoginName} from server role {RoleName} on server {ServerInstance}", 
                loginName, roleName, serverInstance);
            throw new SqlServerLoginException($"Error removing login '{loginName}' from server role '{roleName}': {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<string>> GetServerRolesForLoginAsync(string loginName, string serverInstance)
    {
        try
        {
            ValidateLoginName(loginName);
            ValidateServerInstance(serverInstance);

            return await _loginRepository.GetServerRolesForLoginAsync(loginName, serverInstance);
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error retrieving server roles for login {LoginName} on server {ServerInstance}", 
                loginName, serverInstance);
            throw new SqlServerLoginException($"Error retrieving server roles for login '{loginName}': {ex.Message}", ex);
        }
    }

    public async Task<bool> LoginExistsAsync(string loginName, string serverInstance)
    {
        try
        {
            ValidateLoginName(loginName);
            ValidateServerInstance(serverInstance);

            return await _loginRepository.LoginExistsAsync(loginName, serverInstance);
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error checking if login {LoginName} exists on server {ServerInstance}", 
                loginName, serverInstance);
            throw new SqlServerLoginException($"Error checking if login '{loginName}' exists: {ex.Message}", ex);
        }
    }

    public Task<bool> ValidateLoginNameAsync(string loginName)
    {
        ValidateLoginName(loginName);
        return Task.FromResult(true);
    }

    public Task<bool> ValidatePasswordAsync(string password)
    {
        ValidatePassword(password);
        return Task.FromResult(true);
    }

    private async Task ValidateLoginCreationAsync(string loginName, string password, string serverInstance)
    {
        ValidateLoginName(loginName);
        ValidatePassword(password);
        ValidateServerInstance(serverInstance);

        var loginExists = await _loginRepository.LoginExistsAsync(loginName, serverInstance);
        if (loginExists)
        {
            throw new SqlServerValidationException($"Login '{loginName}' already exists on server '{serverInstance}'");
        }
    }

    private async Task ValidateWindowsLoginCreationAsync(string loginName, string serverInstance)
    {
        ValidateWindowsLoginName(loginName);
        ValidateServerInstance(serverInstance);

        var loginExists = await _loginRepository.LoginExistsAsync(loginName, serverInstance);
        if (loginExists)
        {
            throw new SqlServerValidationException($"Windows login '{loginName}' already exists on server '{serverInstance}'");
        }
    }

    private static void ValidateLoginName(string loginName)
    {
        if (string.IsNullOrWhiteSpace(loginName))
        {
            throw new SqlServerValidationException("Login name cannot be null or empty");
        }

        if (loginName.Length > 128)
        {
            throw new SqlServerValidationException("Login name cannot exceed 128 characters");
        }

        if (loginName.Contains("'") || loginName.Contains("\"") || loginName.Contains(";"))
        {
            throw new SqlServerValidationException("Login name cannot contain quotes or semicolons");
        }

        var reservedNames = new[] { "sa", "public", "guest", "dbo", "sys", "INFORMATION_SCHEMA" };
        if (reservedNames.Contains(loginName, StringComparer.OrdinalIgnoreCase))
        {
            throw new SqlServerValidationException($"Login name '{loginName}' is reserved and cannot be used");
        }
    }

    private static void ValidateWindowsLoginName(string loginName)
    {
        if (string.IsNullOrWhiteSpace(loginName))
        {
            throw new SqlServerValidationException("Windows login name cannot be null or empty");
        }

        if (!loginName.Contains('\\') && !loginName.Contains('@'))
        {
            throw new SqlServerValidationException("Windows login name must be in format 'DOMAIN\\User' or 'user@domain.com'");
        }

        if (loginName.Length > 128)
        {
            throw new SqlServerValidationException("Windows login name cannot exceed 128 characters");
        }
    }

    private static void ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new SqlServerValidationException("Password cannot be null or empty");
        }

        if (password.Length < 8)
        {
            throw new SqlServerValidationException("Password must be at least 8 characters long");
        }

        if (password.Length > 128)
        {
            throw new SqlServerValidationException("Password cannot exceed 128 characters");
        }

        var hasUpper = Regex.IsMatch(password, @"[A-Z]");
        var hasLower = Regex.IsMatch(password, @"[a-z]");
        var hasDigit = Regex.IsMatch(password, @"[0-9]");
        var hasSpecial = Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]");

        var strengthCount = new[] { hasUpper, hasLower, hasDigit, hasSpecial }.Count(x => x);
        if (strengthCount < 3)
        {
            throw new SqlServerValidationException("Password must contain at least 3 of the following: uppercase letters, lowercase letters, digits, special characters");
        }
    }

    private static void ValidateRoleName(string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            throw new SqlServerValidationException("Role name cannot be null or empty");
        }

        if (roleName.Length > 128)
        {
            throw new SqlServerValidationException("Role name cannot exceed 128 characters");
        }
    }

    private static void ValidateServerInstance(string serverInstance)
    {
        if (string.IsNullOrWhiteSpace(serverInstance))
        {
            throw new SqlServerValidationException("Server instance cannot be null or empty");
        }

        if (serverInstance.Length > 128)
        {
            throw new SqlServerValidationException("Server instance cannot exceed 128 characters");
        }
    }
}