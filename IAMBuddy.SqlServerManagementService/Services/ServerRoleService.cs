using IAMBuddy.SqlServerManagementService.Exceptions;
using IAMBuddy.SqlServerManagementService.Models;
using IAMBuddy.SqlServerManagementService.Repositories;
using Microsoft.Extensions.Logging;

namespace IAMBuddy.SqlServerManagementService.Services;

public class ServerRoleService : IServerRoleService
{
    private readonly IServerRoleRepository _roleRepository;
    private readonly ILogger<ServerRoleService> _logger;

    public ServerRoleService(
        IServerRoleRepository roleRepository,
        ILogger<ServerRoleService> logger)
    {
        _roleRepository = roleRepository;
        _logger = logger;
    }

    public async Task<SqlServerRole> CreateServerRoleAsync(string roleName, string serverInstance, string? createdBy = null)
    {
        try
        {
            _logger.LogInformation("Creating server role {RoleName} on server {ServerInstance}", roleName, serverInstance);
            
            await ValidateRoleCreationAsync(roleName, serverInstance);

            var success = await _roleRepository.CreateServerRoleAsync(roleName, serverInstance);
            if (!success)
            {
                throw new SqlServerRoleException($"Failed to create server role '{roleName}' on server '{serverInstance}'");
            }

            var role = await _roleRepository.GetServerRoleAsync(roleName, serverInstance);
            if (role == null)
            {
                throw new SqlServerRoleException($"Server role '{roleName}' was created but could not be retrieved");
            }

            _logger.LogInformation("Successfully created server role {RoleName} on server {ServerInstance}", roleName, serverInstance);
            return role;
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Unexpected error creating server role {RoleName} on server {ServerInstance}", roleName, serverInstance);
            throw new SqlServerRoleException($"Unexpected error creating server role '{roleName}': {ex.Message}", ex);
        }
    }

    public async Task<SqlServerRole?> GetServerRoleAsync(string roleName, string serverInstance)
    {
        try
        {
            ValidateRoleName(roleName);
            ValidateServerInstance(serverInstance);

            return await _roleRepository.GetServerRoleAsync(roleName, serverInstance);
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error retrieving server role {RoleName} from server {ServerInstance}", roleName, serverInstance);
            throw new SqlServerRoleException($"Error retrieving server role '{roleName}': {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<SqlServerRole>> GetServerRolesAsync(string serverInstance)
    {
        try
        {
            ValidateServerInstance(serverInstance);
            return await _roleRepository.GetServerRolesAsync(serverInstance);
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error retrieving server roles from server {ServerInstance}", serverInstance);
            throw new SqlServerRoleException($"Error retrieving server roles from server '{serverInstance}': {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteServerRoleAsync(string roleName, string serverInstance, string? deletedBy = null)
    {
        try
        {
            _logger.LogInformation("Deleting server role {RoleName} from server {ServerInstance}", roleName, serverInstance);
            
            ValidateRoleName(roleName);
            ValidateServerInstance(serverInstance);

            var roleExists = await _roleRepository.ServerRoleExistsAsync(roleName, serverInstance);
            if (!roleExists)
            {
                _logger.LogWarning("Server role {RoleName} does not exist on server {ServerInstance}", roleName, serverInstance);
                return false;
            }

            var role = await _roleRepository.GetServerRoleAsync(roleName, serverInstance);
            if (role?.IsBuiltIn == true)
            {
                throw new SqlServerValidationException($"Cannot delete built-in server role '{roleName}'");
            }

            var members = await _roleRepository.GetRoleMembersAsync(roleName, serverInstance);
            if (members.Any())
            {
                _logger.LogWarning("Cannot delete server role {RoleName} as it has {MemberCount} members", roleName, members.Count());
                throw new SqlServerValidationException($"Cannot delete server role '{roleName}' because it has members. Remove all members first.");
            }

            var success = await _roleRepository.DropServerRoleAsync(roleName, serverInstance);
            if (success)
            {
                _logger.LogInformation("Successfully deleted server role {RoleName} from server {ServerInstance}", roleName, serverInstance);
            }
            else
            {
                _logger.LogWarning("Failed to delete server role {RoleName} from server {ServerInstance}", roleName, serverInstance);
            }

            return success;
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Unexpected error deleting server role {RoleName} from server {ServerInstance}", roleName, serverInstance);
            throw new SqlServerRoleException($"Unexpected error deleting server role '{roleName}': {ex.Message}", ex);
        }
    }

    public async Task<bool> AddMemberToRoleAsync(string roleName, string memberName, string serverInstance, string? modifiedBy = null)
    {
        try
        {
            _logger.LogInformation("Adding member {MemberName} to server role {RoleName} on server {ServerInstance}", 
                memberName, roleName, serverInstance);
            
            ValidateRoleName(roleName);
            ValidateMemberName(memberName);
            ValidateServerInstance(serverInstance);

            var roleExists = await _roleRepository.ServerRoleExistsAsync(roleName, serverInstance);
            if (!roleExists)
            {
                throw new SqlServerValidationException($"Server role '{roleName}' does not exist on server '{serverInstance}'");
            }

            var success = await _roleRepository.AddMemberToRoleAsync(roleName, memberName, serverInstance);
            if (success)
            {
                _logger.LogInformation("Successfully added member {MemberName} to server role {RoleName} on server {ServerInstance}", 
                    memberName, roleName, serverInstance);
            }

            return success;
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error adding member {MemberName} to server role {RoleName} on server {ServerInstance}", 
                memberName, roleName, serverInstance);
            throw new SqlServerRoleException($"Error adding member '{memberName}' to server role '{roleName}': {ex.Message}", ex);
        }
    }

    public async Task<bool> RemoveMemberFromRoleAsync(string roleName, string memberName, string serverInstance, string? modifiedBy = null)
    {
        try
        {
            _logger.LogInformation("Removing member {MemberName} from server role {RoleName} on server {ServerInstance}", 
                memberName, roleName, serverInstance);
            
            ValidateRoleName(roleName);
            ValidateMemberName(memberName);
            ValidateServerInstance(serverInstance);

            var success = await _roleRepository.RemoveMemberFromRoleAsync(roleName, memberName, serverInstance);
            if (success)
            {
                _logger.LogInformation("Successfully removed member {MemberName} from server role {RoleName} on server {ServerInstance}", 
                    memberName, roleName, serverInstance);
            }

            return success;
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error removing member {MemberName} from server role {RoleName} on server {ServerInstance}", 
                memberName, roleName, serverInstance);
            throw new SqlServerRoleException($"Error removing member '{memberName}' from server role '{roleName}': {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<string>> GetRoleMembersAsync(string roleName, string serverInstance)
    {
        try
        {
            ValidateRoleName(roleName);
            ValidateServerInstance(serverInstance);

            return await _roleRepository.GetRoleMembersAsync(roleName, serverInstance);
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error retrieving members for server role {RoleName} on server {ServerInstance}", 
                roleName, serverInstance);
            throw new SqlServerRoleException($"Error retrieving members for server role '{roleName}': {ex.Message}", ex);
        }
    }

    public async Task<bool> ServerRoleExistsAsync(string roleName, string serverInstance)
    {
        try
        {
            ValidateRoleName(roleName);
            ValidateServerInstance(serverInstance);

            return await _roleRepository.ServerRoleExistsAsync(roleName, serverInstance);
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error checking if server role {RoleName} exists on server {ServerInstance}", 
                roleName, serverInstance);
            throw new SqlServerRoleException($"Error checking if server role '{roleName}' exists: {ex.Message}", ex);
        }
    }

    public async Task<bool> GrantPermissionToRoleAsync(string roleName, string permission, string serverInstance, string? objectName = null, string? grantedBy = null)
    {
        try
        {
            _logger.LogInformation("Granting permission {Permission} to server role {RoleName} on server {ServerInstance}", 
                permission, roleName, serverInstance);
            
            ValidateRoleName(roleName);
            ValidatePermission(permission);
            ValidateServerInstance(serverInstance);

            var roleExists = await _roleRepository.ServerRoleExistsAsync(roleName, serverInstance);
            if (!roleExists)
            {
                throw new SqlServerValidationException($"Server role '{roleName}' does not exist on server '{serverInstance}'");
            }

            var success = await _roleRepository.GrantPermissionToRoleAsync(roleName, permission, serverInstance, objectName);
            if (success)
            {
                _logger.LogInformation("Successfully granted permission {Permission} to server role {RoleName} on server {ServerInstance}", 
                    permission, roleName, serverInstance);
            }

            return success;
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error granting permission {Permission} to server role {RoleName} on server {ServerInstance}", 
                permission, roleName, serverInstance);
            throw new SqlServerPermissionException($"Error granting permission '{permission}' to server role '{roleName}': {ex.Message}", ex);
        }
    }

    public async Task<bool> RevokePermissionFromRoleAsync(string roleName, string permission, string serverInstance, string? objectName = null, string? revokedBy = null)
    {
        try
        {
            _logger.LogInformation("Revoking permission {Permission} from server role {RoleName} on server {ServerInstance}", 
                permission, roleName, serverInstance);
            
            ValidateRoleName(roleName);
            ValidatePermission(permission);
            ValidateServerInstance(serverInstance);

            var success = await _roleRepository.RevokePermissionFromRoleAsync(roleName, permission, serverInstance, objectName);
            if (success)
            {
                _logger.LogInformation("Successfully revoked permission {Permission} from server role {RoleName} on server {ServerInstance}", 
                    permission, roleName, serverInstance);
            }

            return success;
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error revoking permission {Permission} from server role {RoleName} on server {ServerInstance}", 
                permission, roleName, serverInstance);
            throw new SqlServerPermissionException($"Error revoking permission '{permission}' from server role '{roleName}': {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<string>> GetRolePermissionsAsync(string roleName, string serverInstance)
    {
        try
        {
            ValidateRoleName(roleName);
            ValidateServerInstance(serverInstance);

            return await _roleRepository.GetRolePermissionsAsync(roleName, serverInstance);
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error retrieving permissions for server role {RoleName} on server {ServerInstance}", 
                roleName, serverInstance);
            throw new SqlServerRoleException($"Error retrieving permissions for server role '{roleName}': {ex.Message}", ex);
        }
    }

    public Task<bool> ValidateRoleNameAsync(string roleName)
    {
        ValidateRoleName(roleName);
        return Task.FromResult(true);
    }

    public Task<bool> ValidatePermissionAsync(string permission)
    {
        ValidatePermission(permission);
        return Task.FromResult(true);
    }

    private async Task ValidateRoleCreationAsync(string roleName, string serverInstance)
    {
        ValidateRoleName(roleName);
        ValidateServerInstance(serverInstance);

        var roleExists = await _roleRepository.ServerRoleExistsAsync(roleName, serverInstance);
        if (roleExists)
        {
            throw new SqlServerValidationException($"Server role '{roleName}' already exists on server '{serverInstance}'");
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

        if (roleName.Contains("'") || roleName.Contains("\"") || roleName.Contains(";"))
        {
            throw new SqlServerValidationException("Role name cannot contain quotes or semicolons");
        }

        var reservedNames = new[] { "public", "guest", "dbo", "sys", "INFORMATION_SCHEMA" };
        if (reservedNames.Contains(roleName, StringComparer.OrdinalIgnoreCase))
        {
            throw new SqlServerValidationException($"Role name '{roleName}' is reserved and cannot be used");
        }

        var builtInServerRoles = new[] { "sysadmin", "serveradmin", "securityadmin", "processadmin", "setupadmin", "bulkadmin", "diskadmin", "dbcreator" };
        if (builtInServerRoles.Contains(roleName, StringComparer.OrdinalIgnoreCase))
        {
            throw new SqlServerValidationException($"Role name '{roleName}' is a built-in server role and cannot be created");
        }
    }

    private static void ValidateMemberName(string memberName)
    {
        if (string.IsNullOrWhiteSpace(memberName))
        {
            throw new SqlServerValidationException("Member name cannot be null or empty");
        }

        if (memberName.Length > 128)
        {
            throw new SqlServerValidationException("Member name cannot exceed 128 characters");
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

        var validServerPermissions = new[]
        {
            "ADMINISTER BULK OPERATIONS", "ALTER ANY CONNECTION", "ALTER ANY CREDENTIAL", "ALTER ANY DATABASE",
            "ALTER ANY ENDPOINT", "ALTER ANY EVENT NOTIFICATION", "ALTER ANY EVENT SESSION", "ALTER ANY LINKED SERVER",
            "ALTER ANY LOGIN", "ALTER ANY SERVER AUDIT", "ALTER ANY SERVER ROLE", "ALTER RESOURCES",
            "ALTER SERVER STATE", "ALTER SETTINGS", "ALTER TRACE", "AUTHENTICATE SERVER", "CONNECT ANY DATABASE",
            "CONNECT SQL", "CONTROL SERVER", "CREATE ANY DATABASE", "CREATE AVAILABILITY GROUP", "CREATE DDL EVENT NOTIFICATION",
            "CREATE ENDPOINT", "CREATE SERVER ROLE", "CREATE TRACE EVENT NOTIFICATION", "EXTERNAL ACCESS ASSEMBLY",
            "SHUTDOWN", "UNSAFE ASSEMBLY", "VIEW ANY DATABASE", "VIEW ANY DEFINITION", "VIEW SERVER STATE"
        };

        if (!validServerPermissions.Contains(permission, StringComparer.OrdinalIgnoreCase))
        {
            throw new SqlServerValidationException($"Invalid server permission: '{permission}'");
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