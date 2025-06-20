using IAMBuddy.SqlServerManagementService.Exceptions;
using IAMBuddy.SqlServerManagementService.Models;
using IAMBuddy.SqlServerManagementService.Repositories;
using Microsoft.Extensions.Logging;

namespace IAMBuddy.SqlServerManagementService.Services;

public class DatabaseUserService : IDatabaseUserService
{
    private readonly IDatabaseUserRepository _userRepository;
    private readonly ISqlLoginRepository _loginRepository;
    private readonly IDatabaseRoleRepository _roleRepository;
    private readonly ILogger<DatabaseUserService> _logger;

    public DatabaseUserService(
        IDatabaseUserRepository userRepository,
        ISqlLoginRepository loginRepository,
        IDatabaseRoleRepository roleRepository,
        ILogger<DatabaseUserService> logger)
    {
        _userRepository = userRepository;
        _loginRepository = loginRepository;
        _roleRepository = roleRepository;
        _logger = logger;
    }

    public async Task<SqlServerUser> CreateUserAsync(string userName, string loginName, string databaseName, string serverInstance, string? defaultSchema = null, string? createdBy = null)
    {
        try
        {
            _logger.LogInformation("Creating database user {UserName} for login {LoginName} in database {DatabaseName} on server {ServerInstance}", 
                userName, loginName, databaseName, serverInstance);
            
            await ValidateUserCreationAsync(userName, loginName, databaseName, serverInstance, defaultSchema);

            var success = await _userRepository.CreateUserAsync(userName, loginName, databaseName, serverInstance, defaultSchema);
            if (!success)
            {
                throw new SqlServerUserException($"Failed to create user '{userName}' for login '{loginName}' in database '{databaseName}' on server '{serverInstance}'");
            }

            var user = await _userRepository.GetUserAsync(userName, databaseName, serverInstance);
            if (user == null)
            {
                throw new SqlServerUserException($"User '{userName}' was created but could not be retrieved");
            }

            _logger.LogInformation("Successfully created database user {UserName} for login {LoginName} in database {DatabaseName} on server {ServerInstance}", 
                userName, loginName, databaseName, serverInstance);
            return user;
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Unexpected error creating database user {UserName} for login {LoginName} in database {DatabaseName} on server {ServerInstance}", 
                userName, loginName, databaseName, serverInstance);
            throw new SqlServerUserException($"Unexpected error creating database user '{userName}': {ex.Message}", ex);
        }
    }

    public async Task<SqlServerUser> CreateUserWithoutLoginAsync(string userName, string databaseName, string serverInstance, string? defaultSchema = null, string? createdBy = null)
    {
        try
        {
            _logger.LogInformation("Creating database user {UserName} without login in database {DatabaseName} on server {ServerInstance}", 
                userName, databaseName, serverInstance);
            
            await ValidateUserWithoutLoginCreationAsync(userName, databaseName, serverInstance, defaultSchema);

            var success = await _userRepository.CreateUserWithoutLoginAsync(userName, databaseName, serverInstance, defaultSchema);
            if (!success)
            {
                throw new SqlServerUserException($"Failed to create user '{userName}' without login in database '{databaseName}' on server '{serverInstance}'");
            }

            var user = await _userRepository.GetUserAsync(userName, databaseName, serverInstance);
            if (user == null)
            {
                throw new SqlServerUserException($"User '{userName}' was created but could not be retrieved");
            }

            _logger.LogInformation("Successfully created database user {UserName} without login in database {DatabaseName} on server {ServerInstance}", 
                userName, databaseName, serverInstance);
            return user;
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Unexpected error creating database user {UserName} without login in database {DatabaseName} on server {ServerInstance}", 
                userName, databaseName, serverInstance);
            throw new SqlServerUserException($"Unexpected error creating database user '{userName}' without login: {ex.Message}", ex);
        }
    }

    public async Task<SqlServerUser?> GetUserAsync(string userName, string databaseName, string serverInstance)
    {
        try
        {
            ValidateUserName(userName);
            ValidateDatabaseName(databaseName);
            ValidateServerInstance(serverInstance);

            return await _userRepository.GetUserAsync(userName, databaseName, serverInstance);
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error retrieving database user {UserName} from database {DatabaseName} on server {ServerInstance}", 
                userName, databaseName, serverInstance);
            throw new SqlServerUserException($"Error retrieving database user '{userName}': {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<SqlServerUser>> GetUsersAsync(string databaseName, string serverInstance)
    {
        try
        {
            ValidateDatabaseName(databaseName);
            ValidateServerInstance(serverInstance);

            return await _userRepository.GetUsersAsync(databaseName, serverInstance);
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error retrieving database users from database {DatabaseName} on server {ServerInstance}", 
                databaseName, serverInstance);
            throw new SqlServerUserException($"Error retrieving database users from database '{databaseName}': {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<SqlServerUser>> GetUsersByLoginAsync(string loginName, string serverInstance)
    {
        try
        {
            ValidateLoginName(loginName);
            ValidateServerInstance(serverInstance);

            var allUsers = await _userRepository.GetUsersAsync("", serverInstance);
            return allUsers.Where(u => u.Login?.LoginName == loginName).ToList();
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error retrieving database users for login {LoginName} on server {ServerInstance}", 
                loginName, serverInstance);
            throw new SqlServerUserException($"Error retrieving database users for login '{loginName}': {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteUserAsync(string userName, string databaseName, string serverInstance, string? deletedBy = null)
    {
        try
        {
            _logger.LogInformation("Deleting database user {UserName} from database {DatabaseName} on server {ServerInstance}", 
                userName, databaseName, serverInstance);
            
            ValidateUserName(userName);
            ValidateDatabaseName(databaseName);
            ValidateServerInstance(serverInstance);

            var userExists = await _userRepository.UserExistsAsync(userName, databaseName, serverInstance);
            if (!userExists)
            {
                _logger.LogWarning("Database user {UserName} does not exist in database {DatabaseName} on server {ServerInstance}", 
                    userName, databaseName, serverInstance);
                return false;
            }

            var userRoles = await _userRepository.GetUserRolesAsync(userName, databaseName, serverInstance);
            foreach (var role in userRoles)
            {
                _logger.LogDebug("Removing user {UserName} from role {RoleName} in database {DatabaseName}", userName, role, databaseName);
                await _userRepository.RemoveFromRoleAsync(userName, role, databaseName, serverInstance);
            }

            var success = await _userRepository.DropUserAsync(userName, databaseName, serverInstance);
            if (success)
            {
                _logger.LogInformation("Successfully deleted database user {UserName} from database {DatabaseName} on server {ServerInstance}", 
                    userName, databaseName, serverInstance);
            }
            else
            {
                _logger.LogWarning("Failed to delete database user {UserName} from database {DatabaseName} on server {ServerInstance}", 
                    userName, databaseName, serverInstance);
            }

            return success;
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Unexpected error deleting database user {UserName} from database {DatabaseName} on server {ServerInstance}", 
                userName, databaseName, serverInstance);
            throw new SqlServerUserException($"Unexpected error deleting database user '{userName}': {ex.Message}", ex);
        }
    }

    public async Task<bool> AlterUserDefaultSchemaAsync(string userName, string defaultSchema, string databaseName, string serverInstance, string? modifiedBy = null)
    {
        try
        {
            _logger.LogInformation("Altering default schema for user {UserName} to {DefaultSchema} in database {DatabaseName} on server {ServerInstance}", 
                userName, defaultSchema, databaseName, serverInstance);
            
            ValidateUserName(userName);
            ValidateSchemaName(defaultSchema);
            ValidateDatabaseName(databaseName);
            ValidateServerInstance(serverInstance);

            var userExists = await _userRepository.UserExistsAsync(userName, databaseName, serverInstance);
            if (!userExists)
            {
                throw new SqlServerValidationException($"Database user '{userName}' does not exist in database '{databaseName}' on server '{serverInstance}'");
            }

            var success = await _userRepository.AlterUserDefaultSchemaAsync(userName, defaultSchema, databaseName, serverInstance);
            if (success)
            {
                _logger.LogInformation("Successfully altered default schema for user {UserName} to {DefaultSchema} in database {DatabaseName} on server {ServerInstance}", 
                    userName, defaultSchema, databaseName, serverInstance);
            }

            return success;
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error altering default schema for user {UserName} in database {DatabaseName} on server {ServerInstance}", 
                userName, databaseName, serverInstance);
            throw new SqlServerUserException($"Error altering default schema for user '{userName}': {ex.Message}", ex);
        }
    }

    public async Task<bool> AddToRoleAsync(string userName, string roleName, string databaseName, string serverInstance, string? modifiedBy = null)
    {
        try
        {
            _logger.LogInformation("Adding user {UserName} to role {RoleName} in database {DatabaseName} on server {ServerInstance}", 
                userName, roleName, databaseName, serverInstance);
            
            ValidateUserName(userName);
            ValidateRoleName(roleName);
            ValidateDatabaseName(databaseName);
            ValidateServerInstance(serverInstance);

            var userExists = await _userRepository.UserExistsAsync(userName, databaseName, serverInstance);
            if (!userExists)
            {
                throw new SqlServerValidationException($"Database user '{userName}' does not exist in database '{databaseName}' on server '{serverInstance}'");
            }

            var roleExists = await _roleRepository.RoleExistsAsync(roleName, databaseName, serverInstance);
            if (!roleExists)
            {
                throw new SqlServerValidationException($"Database role '{roleName}' does not exist in database '{databaseName}' on server '{serverInstance}'");
            }

            var success = await _userRepository.AddToRoleAsync(userName, roleName, databaseName, serverInstance);
            if (success)
            {
                _logger.LogInformation("Successfully added user {UserName} to role {RoleName} in database {DatabaseName} on server {ServerInstance}", 
                    userName, roleName, databaseName, serverInstance);
            }

            return success;
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error adding user {UserName} to role {RoleName} in database {DatabaseName} on server {ServerInstance}", 
                userName, roleName, databaseName, serverInstance);
            throw new SqlServerUserException($"Error adding user '{userName}' to role '{roleName}': {ex.Message}", ex);
        }
    }

    public async Task<bool> RemoveFromRoleAsync(string userName, string roleName, string databaseName, string serverInstance, string? modifiedBy = null)
    {
        try
        {
            _logger.LogInformation("Removing user {UserName} from role {RoleName} in database {DatabaseName} on server {ServerInstance}", 
                userName, roleName, databaseName, serverInstance);
            
            ValidateUserName(userName);
            ValidateRoleName(roleName);
            ValidateDatabaseName(databaseName);
            ValidateServerInstance(serverInstance);

            var success = await _userRepository.RemoveFromRoleAsync(userName, roleName, databaseName, serverInstance);
            if (success)
            {
                _logger.LogInformation("Successfully removed user {UserName} from role {RoleName} in database {DatabaseName} on server {ServerInstance}", 
                    userName, roleName, databaseName, serverInstance);
            }

            return success;
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error removing user {UserName} from role {RoleName} in database {DatabaseName} on server {ServerInstance}", 
                userName, roleName, databaseName, serverInstance);
            throw new SqlServerUserException($"Error removing user '{userName}' from role '{roleName}': {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(string userName, string databaseName, string serverInstance)
    {
        try
        {
            ValidateUserName(userName);
            ValidateDatabaseName(databaseName);
            ValidateServerInstance(serverInstance);

            return await _userRepository.GetUserRolesAsync(userName, databaseName, serverInstance);
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error retrieving roles for user {UserName} in database {DatabaseName} on server {ServerInstance}", 
                userName, databaseName, serverInstance);
            throw new SqlServerUserException($"Error retrieving roles for user '{userName}': {ex.Message}", ex);
        }
    }

    public async Task<bool> UserExistsAsync(string userName, string databaseName, string serverInstance)
    {
        try
        {
            ValidateUserName(userName);
            ValidateDatabaseName(databaseName);
            ValidateServerInstance(serverInstance);

            return await _userRepository.UserExistsAsync(userName, databaseName, serverInstance);
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error checking if user {UserName} exists in database {DatabaseName} on server {ServerInstance}", 
                userName, databaseName, serverInstance);
            throw new SqlServerUserException($"Error checking if user '{userName}' exists: {ex.Message}", ex);
        }
    }

    public async Task<bool> GrantPermissionAsync(string userName, string permission, string databaseName, string serverInstance, string? objectName = null, string? grantedBy = null)
    {
        try
        {
            _logger.LogInformation("Granting permission {Permission} to user {UserName} in database {DatabaseName} on server {ServerInstance}", 
                permission, userName, databaseName, serverInstance);
            
            ValidateUserName(userName);
            ValidatePermission(permission);
            ValidateDatabaseName(databaseName);
            ValidateServerInstance(serverInstance);

            var userExists = await _userRepository.UserExistsAsync(userName, databaseName, serverInstance);
            if (!userExists)
            {
                throw new SqlServerValidationException($"Database user '{userName}' does not exist in database '{databaseName}' on server '{serverInstance}'");
            }

            var success = await _userRepository.GrantPermissionAsync(userName, permission, databaseName, serverInstance, objectName);
            if (success)
            {
                _logger.LogInformation("Successfully granted permission {Permission} to user {UserName} in database {DatabaseName} on server {ServerInstance}", 
                    permission, userName, databaseName, serverInstance);
            }

            return success;
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error granting permission {Permission} to user {UserName} in database {DatabaseName} on server {ServerInstance}", 
                permission, userName, databaseName, serverInstance);
            throw new SqlServerPermissionException($"Error granting permission '{permission}' to user '{userName}': {ex.Message}", ex);
        }
    }

    public async Task<bool> RevokePermissionAsync(string userName, string permission, string databaseName, string serverInstance, string? objectName = null, string? revokedBy = null)
    {
        try
        {
            _logger.LogInformation("Revoking permission {Permission} from user {UserName} in database {DatabaseName} on server {ServerInstance}", 
                permission, userName, databaseName, serverInstance);
            
            ValidateUserName(userName);
            ValidatePermission(permission);
            ValidateDatabaseName(databaseName);
            ValidateServerInstance(serverInstance);

            var success = await _userRepository.RevokePermissionAsync(userName, permission, databaseName, serverInstance, objectName);
            if (success)
            {
                _logger.LogInformation("Successfully revoked permission {Permission} from user {UserName} in database {DatabaseName} on server {ServerInstance}", 
                    permission, userName, databaseName, serverInstance);
            }

            return success;
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error revoking permission {Permission} from user {UserName} in database {DatabaseName} on server {ServerInstance}", 
                permission, userName, databaseName, serverInstance);
            throw new SqlServerPermissionException($"Error revoking permission '{permission}' from user '{userName}': {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<string>> GetUserPermissionsAsync(string userName, string databaseName, string serverInstance)
    {
        try
        {
            ValidateUserName(userName);
            ValidateDatabaseName(databaseName);
            ValidateServerInstance(serverInstance);

            return await _userRepository.GetUserPermissionsAsync(userName, databaseName, serverInstance);
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error retrieving permissions for user {UserName} in database {DatabaseName} on server {ServerInstance}", 
                userName, databaseName, serverInstance);
            throw new SqlServerUserException($"Error retrieving permissions for user '{userName}': {ex.Message}", ex);
        }
    }

    public Task<bool> ValidateUserNameAsync(string userName)
    {
        ValidateUserName(userName);
        return Task.FromResult(true);
    }

    public Task<bool> ValidateSchemaNameAsync(string schemaName)
    {
        ValidateSchemaName(schemaName);
        return Task.FromResult(true);
    }

    private async Task ValidateUserCreationAsync(string userName, string loginName, string databaseName, string serverInstance, string? defaultSchema)
    {
        ValidateUserName(userName);
        ValidateLoginName(loginName);
        ValidateDatabaseName(databaseName);
        ValidateServerInstance(serverInstance);

        if (!string.IsNullOrEmpty(defaultSchema))
        {
            ValidateSchemaName(defaultSchema);
        }

        var userExists = await _userRepository.UserExistsAsync(userName, databaseName, serverInstance);
        if (userExists)
        {
            throw new SqlServerValidationException($"Database user '{userName}' already exists in database '{databaseName}' on server '{serverInstance}'");
        }

        var loginExists = await _loginRepository.LoginExistsAsync(loginName, serverInstance);
        if (!loginExists)
        {
            throw new SqlServerValidationException($"Login '{loginName}' does not exist on server '{serverInstance}'");
        }
    }

    private async Task ValidateUserWithoutLoginCreationAsync(string userName, string databaseName, string serverInstance, string? defaultSchema)
    {
        ValidateUserName(userName);
        ValidateDatabaseName(databaseName);
        ValidateServerInstance(serverInstance);

        if (!string.IsNullOrEmpty(defaultSchema))
        {
            ValidateSchemaName(defaultSchema);
        }

        var userExists = await _userRepository.UserExistsAsync(userName, databaseName, serverInstance);
        if (userExists)
        {
            throw new SqlServerValidationException($"Database user '{userName}' already exists in database '{databaseName}' on server '{serverInstance}'");
        }
    }

    private static void ValidateUserName(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            throw new SqlServerValidationException("User name cannot be null or empty");
        }

        if (userName.Length > 128)
        {
            throw new SqlServerValidationException("User name cannot exceed 128 characters");
        }

        if (userName.Contains("'") || userName.Contains("\"") || userName.Contains(";"))
        {
            throw new SqlServerValidationException("User name cannot contain quotes or semicolons");
        }

        var reservedNames = new[] { "public", "guest", "dbo", "sys", "INFORMATION_SCHEMA" };
        if (reservedNames.Contains(userName, StringComparer.OrdinalIgnoreCase))
        {
            throw new SqlServerValidationException($"User name '{userName}' is reserved and cannot be used");
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
    }

    private static void ValidateDatabaseName(string databaseName)
    {
        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new SqlServerValidationException("Database name cannot be null or empty");
        }

        if (databaseName.Length > 128)
        {
            throw new SqlServerValidationException("Database name cannot exceed 128 characters");
        }
    }

    private static void ValidateSchemaName(string schemaName)
    {
        if (string.IsNullOrWhiteSpace(schemaName))
        {
            throw new SqlServerValidationException("Schema name cannot be null or empty");
        }

        if (schemaName.Length > 128)
        {
            throw new SqlServerValidationException("Schema name cannot exceed 128 characters");
        }

        if (schemaName.Contains("'") || schemaName.Contains("\"") || schemaName.Contains(";"))
        {
            throw new SqlServerValidationException("Schema name cannot contain quotes or semicolons");
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

    private static void ValidatePermission(string permission)
    {
        if (string.IsNullOrWhiteSpace(permission))
        {
            throw new SqlServerValidationException("Permission cannot be null or empty");
        }

        if (permission.Length > 128)
        {
            throw new SqlServerValidationException("Permission cannot exceed 128 characters");
        }

        var validDatabasePermissions = new[]
        {
            "ALTER", "CONTROL", "CREATE AGGREGATE", "CREATE ASSEMBLY", "CREATE ASYMMETRIC KEY", "CREATE CERTIFICATE",
            "CREATE CONTRACT", "CREATE DATABASE", "CREATE DDL EVENT NOTIFICATION", "CREATE DEFAULT", "CREATE FULLTEXT CATALOG",
            "CREATE FUNCTION", "CREATE MESSAGE TYPE", "CREATE PROCEDURE", "CREATE QUEUE", "CREATE REMOTE SERVICE BINDING",
            "CREATE ROLE", "CREATE ROUTE", "CREATE RULE", "CREATE SCHEMA", "CREATE SERVICE", "CREATE SYMMETRIC KEY",
            "CREATE SYNONYM", "CREATE TABLE", "CREATE TYPE", "CREATE VIEW", "CREATE XML SCHEMA COLLECTION",
            "DELETE", "EXECUTE", "INSERT", "REFERENCES", "SELECT", "TAKE OWNERSHIP", "UPDATE", "VIEW DEFINITION"
        };

        if (!validDatabasePermissions.Contains(permission, StringComparer.OrdinalIgnoreCase))
        {
            throw new SqlServerValidationException($"Invalid database permission: '{permission}'");
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