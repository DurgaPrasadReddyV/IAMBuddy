using System.Data.Common;
using IAMBuddy.SqlServerManagementService.Exceptions;
using IAMBuddy.SqlServerManagementService.Models;
using IAMBuddy.SqlServerManagementService.Repositories;
using Microsoft.Extensions.Logging;

namespace IAMBuddy.SqlServerManagementService.Services;

public class DatabaseRoleService : IDatabaseRoleService
{
    private readonly IDatabaseRoleRepository _roleRepository;
    private readonly IDatabaseUserRepository _userRepository;
    private readonly ILogger<DatabaseRoleService> _logger;

    public DatabaseRoleService(
        IDatabaseRoleRepository roleRepository,
        IDatabaseUserRepository userRepository,
        ILogger<DatabaseRoleService> logger)
    {
        _roleRepository = roleRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<SqlServerRole> CreateRoleAsync(string roleName, string databaseName, string serverInstance, string? ownerName = null, string? createdBy = null)
    {
        try
        {
            _logger.LogInformation("Creating database role {RoleName} in database {DatabaseName} on server {ServerInstance}", 
                roleName, databaseName, serverInstance);
            
            await ValidateRoleCreationAsync(roleName, databaseName, serverInstance, ownerName);

            var success = await _roleRepository.CreateRoleAsync(roleName, databaseName, serverInstance, ownerName);
            if (!success)
            {
                throw new SqlServerRoleException($"Failed to create database role '{roleName}' in database '{databaseName}' on server '{serverInstance}'");
            }

            var role = await _roleRepository.GetRoleAsync(roleName, databaseName, serverInstance);
            if (role == null)
            {
                throw new SqlServerRoleException($"Database role '{roleName}' was created but could not be retrieved");
            }

            _logger.LogInformation("Successfully created database role {RoleName} in database {DatabaseName} on server {ServerInstance}", 
                roleName, databaseName, serverInstance);
            return role;
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Unexpected error creating database role {RoleName} in database {DatabaseName} on server {ServerInstance}", 
                roleName, databaseName, serverInstance);
            throw new SqlServerRoleException($"Unexpected error creating database role '{roleName}': {ex.Message}", ex);
        }
    }

    public async Task<SqlServerRole?> GetRoleAsync(string roleName, string databaseName, string serverInstance)
    {
        try
        {
            ValidateRoleName(roleName);
            ValidateDatabaseName(databaseName);
            ValidateServerInstance(serverInstance);

            return await _roleRepository.GetRoleAsync(roleName, databaseName, serverInstance);
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error retrieving database role {RoleName} from database {DatabaseName} on server {ServerInstance}", 
                roleName, databaseName, serverInstance);
            throw new SqlServerRoleException($"Error retrieving database role '{roleName}': {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<SqlServerRole>> GetRolesAsync(string databaseName, string serverInstance)
    {
        try
        {
            ValidateDatabaseName(databaseName);
            ValidateServerInstance(serverInstance);

            return await _roleRepository.GetRolesAsync(databaseName, serverInstance);
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error retrieving database roles from database {DatabaseName} on server {ServerInstance}", 
                databaseName, serverInstance);
            throw new SqlServerRoleException($"Error retrieving database roles from database '{databaseName}': {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteRoleAsync(string roleName, string databaseName, string serverInstance, string? deletedBy = null)
    {
        try
        {
            _logger.LogInformation("Deleting database role {RoleName} from database {DatabaseName} on server {ServerInstance}", 
                roleName, databaseName, serverInstance);
            
            ValidateRoleName(roleName);
            ValidateDatabaseName(databaseName);
            ValidateServerInstance(serverInstance);

            var roleExists = await _roleRepository.RoleExistsAsync(roleName, databaseName, serverInstance);
            if (!roleExists)
            {
                _logger.LogWarning("Database role {RoleName} does not exist in database {DatabaseName} on server {ServerInstance}", 
                    roleName, databaseName, serverInstance);
                return false;
            }

            var role = await _roleRepository.GetRoleAsync(roleName, databaseName, serverInstance);
            if (role?.IsBuiltIn == true)
            {
                throw new SqlServerValidationException($"Cannot delete built-in database role '{roleName}'");
            }

            var members = await _roleRepository.GetRoleMembersAsync(roleName, databaseName, serverInstance);
            if (members.Any())
            {
                _logger.LogWarning("Cannot delete database role {RoleName} as it has {MemberCount} members", roleName, members.Count());
                throw new SqlServerValidationException($"Cannot delete database role '{roleName}' because it has members. Remove all members first.");
            }

            var success = await _roleRepository.DropRoleAsync(roleName, databaseName, serverInstance);
            if (success)
            {
                _logger.LogInformation("Successfully deleted database role {RoleName} from database {DatabaseName} on server {ServerInstance}", 
                    roleName, databaseName, serverInstance);
            }
            else
            {
                _logger.LogWarning("Failed to delete database role {RoleName} from database {DatabaseName} on server {ServerInstance}", 
                    roleName, databaseName, serverInstance);
            }

            return success;
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Unexpected error deleting database role {RoleName} from database {DatabaseName} on server {ServerInstance}", 
                roleName, databaseName, serverInstance);
            throw new SqlServerRoleException($"Unexpected error deleting database role '{roleName}': {ex.Message}", ex);
        }
    }

    public async Task<bool> AddMemberToRoleAsync(string roleName, string memberName, string databaseName, string serverInstance, string? modifiedBy = null)
    {
        try
        {
            _logger.LogInformation("Adding member {MemberName} to database role {RoleName} in database {DatabaseName} on server {ServerInstance}", 
                memberName, roleName, databaseName, serverInstance);
            
            ValidateRoleName(roleName);
            ValidateMemberName(memberName);
            ValidateDatabaseName(databaseName);
            ValidateServerInstance(serverInstance);

            var roleExists = await _roleRepository.RoleExistsAsync(roleName, databaseName, serverInstance);
            if (!roleExists)
            {
                throw new SqlServerValidationException($"Database role '{roleName}' does not exist in database '{databaseName}' on server '{serverInstance}'");
            }

            var userExists = await _userRepository.UserExistsAsync(memberName, databaseName, serverInstance);
            if (!userExists)
            {
                throw new SqlServerValidationException($"Database user '{memberName}' does not exist in database '{databaseName}' on server '{serverInstance}'");
            }

            var success = await _roleRepository.AddMemberToRoleAsync(roleName, memberName, databaseName, serverInstance);
            if (success)
            {
                _logger.LogInformation("Successfully added member {MemberName} to database role {RoleName} in database {DatabaseName} on server {ServerInstance}", 
                    memberName, roleName, databaseName, serverInstance);
            }

            return success;
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error adding member {MemberName} to database role {RoleName} in database {DatabaseName} on server {ServerInstance}", 
                memberName, roleName, databaseName, serverInstance);
            throw new SqlServerRoleException($"Error adding member '{memberName}' to database role '{roleName}': {ex.Message}", ex);
        }
    }

    public async Task<bool> RemoveMemberFromRoleAsync(string roleName, string memberName, string databaseName, string serverInstance, string? modifiedBy = null)
    {
        try
        {
            _logger.LogInformation("Removing member {MemberName} from database role {RoleName} in database {DatabaseName} on server {ServerInstance}", 
                memberName, roleName, databaseName, serverInstance);
            
            ValidateRoleName(roleName);
            ValidateMemberName(memberName);
            ValidateDatabaseName(databaseName);
            ValidateServerInstance(serverInstance);

            var success = await _roleRepository.RemoveMemberFromRoleAsync(roleName, memberName, databaseName, serverInstance);
            if (success)
            {
                _logger.LogInformation("Successfully removed member {MemberName} from database role {RoleName} in database {DatabaseName} on server {ServerInstance}", 
                    memberName, roleName, databaseName, serverInstance);
            }

            return success;
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error removing member {MemberName} from database role {RoleName} in database {DatabaseName} on server {ServerInstance}", 
                memberName, roleName, databaseName, serverInstance);
            throw new SqlServerRoleException($"Error removing member '{memberName}' from database role '{roleName}': {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<string>> GetRoleMembersAsync(string roleName, string databaseName, string serverInstance)
    {
        try
        {
            ValidateRoleName(roleName);
            ValidateDatabaseName(databaseName);
            ValidateServerInstance(serverInstance);

            return await _roleRepository.GetRoleMembersAsync(roleName, databaseName, serverInstance);
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error retrieving members for database role {RoleName} in database {DatabaseName} on server {ServerInstance}", 
                roleName, databaseName, serverInstance);
            throw new SqlServerRoleException($"Error retrieving members for database role '{roleName}': {ex.Message}", ex);
        }
    }

    public async Task<bool> RoleExistsAsync(string roleName, string databaseName, string serverInstance)
    {
        try
        {
            ValidateRoleName(roleName);
            ValidateDatabaseName(databaseName);
            ValidateServerInstance(serverInstance);

            return await _roleRepository.RoleExistsAsync(roleName, databaseName, serverInstance);
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error checking if database role {RoleName} exists in database {DatabaseName} on server {ServerInstance}", 
                roleName, databaseName, serverInstance);
            throw new SqlServerRoleException($"Error checking if database role '{roleName}' exists: {ex.Message}", ex);
        }
    }

    public async Task<bool> GrantPermissionToRoleAsync(string roleName, string permission, string databaseName, string serverInstance, string? objectName = null, string? grantedBy = null)
    {
        try
        {
            _logger.LogInformation("Granting permission {Permission} to database role {RoleName} in database {DatabaseName} on server {ServerInstance}", 
                permission, roleName, databaseName, serverInstance);
            
            ValidateRoleName(roleName);
            ValidatePermission(permission);
            ValidateDatabaseName(databaseName);
            ValidateServerInstance(serverInstance);

            var roleExists = await _roleRepository.RoleExistsAsync(roleName, databaseName, serverInstance);
            if (!roleExists)
            {
                throw new SqlServerValidationException($"Database role '{roleName}' does not exist in database '{databaseName}' on server '{serverInstance}'");
            }

            var success = await _roleRepository.GrantPermissionToRoleAsync(roleName, permission, databaseName, serverInstance, objectName);
            if (success)
            {
                _logger.LogInformation("Successfully granted permission {Permission} to database role {RoleName} in database {DatabaseName} on server {ServerInstance}", 
                    permission, roleName, databaseName, serverInstance);
            }

            return success;
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error granting permission {Permission} to database role {RoleName} in database {DatabaseName} on server {ServerInstance}", 
                permission, roleName, databaseName, serverInstance);
            throw new SqlServerPermissionException($"Error granting permission '{permission}' to database role '{roleName}': {ex.Message}", ex);
        }
    }

    public async Task<bool> RevokePermissionFromRoleAsync(string roleName, string permission, string databaseName, string serverInstance, string? objectName = null, string? revokedBy = null)
    {
        try
        {
            _logger.LogInformation("Revoking permission {Permission} from database role {RoleName} in database {DatabaseName} on server {ServerInstance}", 
                permission, roleName, databaseName, serverInstance);
            
            ValidateRoleName(roleName);
            ValidatePermission(permission);
            ValidateDatabaseName(databaseName);
            ValidateServerInstance(serverInstance);

            var success = await _roleRepository.RevokePermissionFromRoleAsync(roleName, permission, databaseName, serverInstance, objectName);
            if (success)
            {
                _logger.LogInformation("Successfully revoked permission {Permission} from database role {RoleName} in database {DatabaseName} on server {ServerInstance}", 
                    permission, roleName, databaseName, serverInstance);
            }

            return success;
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error revoking permission {Permission} from database role {RoleName} in database {DatabaseName} on server {ServerInstance}", 
                permission, roleName, databaseName, serverInstance);
            throw new SqlServerPermissionException($"Error revoking permission '{permission}' from database role '{roleName}': {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<string>> GetRolePermissionsAsync(string roleName, string databaseName, string serverInstance)
    {
        try
        {
            ValidateRoleName(roleName);
            ValidateDatabaseName(databaseName);
            ValidateServerInstance(serverInstance);

            return await _roleRepository.GetRolePermissionsAsync(roleName, databaseName, serverInstance);
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error retrieving permissions for database role {RoleName} in database {DatabaseName} on server {ServerInstance}", 
                roleName, databaseName, serverInstance);
            throw new SqlServerRoleException($"Error retrieving permissions for database role '{roleName}': {ex.Message}", ex);
        }
    }

    public async Task<bool> AlterRoleOwnerAsync(string roleName, string newOwner, string databaseName, string serverInstance, string? modifiedBy = null)
    {
        try
        {
            _logger.LogInformation("Altering owner of database role {RoleName} to {NewOwner} in database {DatabaseName} on server {ServerInstance}", 
                roleName, newOwner, databaseName, serverInstance);
            
            ValidateRoleName(roleName);
            ValidateOwnerName(newOwner);
            ValidateDatabaseName(databaseName);
            ValidateServerInstance(serverInstance);

            var roleExists = await _roleRepository.RoleExistsAsync(roleName, databaseName, serverInstance);
            if (!roleExists)
            {
                throw new SqlServerValidationException($"Database role '{roleName}' does not exist in database '{databaseName}' on server '{serverInstance}'");
            }

            var ownerExists = await _userRepository.UserExistsAsync(newOwner, databaseName, serverInstance);
            if (!ownerExists)
            {
                throw new SqlServerValidationException($"Database user '{newOwner}' does not exist in database '{databaseName}' on server '{serverInstance}'");
            }

            var success = await _roleRepository.AlterRoleOwnerAsync(roleName, newOwner, databaseName, serverInstance);
            if (success)
            {
                _logger.LogInformation("Successfully altered owner of database role {RoleName} to {NewOwner} in database {DatabaseName} on server {ServerInstance}", 
                    roleName, newOwner, databaseName, serverInstance);
            }

            return success;
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Error altering owner of database role {RoleName} in database {DatabaseName} on server {ServerInstance}", 
                roleName, databaseName, serverInstance);
            throw new SqlServerRoleException($"Error altering owner of database role '{roleName}': {ex.Message}", ex);
        }
    }

    public Task<bool> ValidateRoleNameAsync(string roleName)
    {
        ValidateRoleName(roleName);
        return Task.FromResult(true);
    }

    public Task<bool> ValidateOwnerNameAsync(string ownerName)
    {
        ValidateOwnerName(ownerName);
        return Task.FromResult(true);
    }

    public async Task<SqlServerRole> CreateRoleWithTransactionAsync(string roleName, string databaseName, string serverInstance, IEnumerable<string>? initialMembers = null, string? ownerName = null, string? createdBy = null)
    {
        try
        {
            _logger.LogInformation("Creating database role {RoleName} with transaction in database {DatabaseName} on server {ServerInstance}", 
                roleName, databaseName, serverInstance);

            await ValidateRoleCreationAsync(roleName, databaseName, serverInstance, ownerName);

            if (initialMembers != null)
            {
                foreach (var member in initialMembers)
                {
                    var memberExists = await _userRepository.UserExistsAsync(member, databaseName, serverInstance);
                    if (!memberExists)
                    {
                        throw new SqlServerValidationException($"Initial member '{member}' does not exist in database '{databaseName}' on server '{serverInstance}'");
                    }
                }
            }

            var success = await _roleRepository.CreateRoleAsync(roleName, databaseName, serverInstance, ownerName);
            if (!success)
            {
                throw new SqlServerRoleException($"Failed to create database role '{roleName}' in database '{databaseName}' on server '{serverInstance}'");
            }

            if (initialMembers != null)
            {
                var membersToAdd = initialMembers.ToList();
                var addedMembers = new List<string>();

                try
                {
                    foreach (var member in membersToAdd)
                    {
                        var memberAdded = await _roleRepository.AddMemberToRoleAsync(roleName, member, databaseName, serverInstance);
                        if (memberAdded)
                        {
                            addedMembers.Add(member);
                            _logger.LogDebug("Added member {MemberName} to role {RoleName}", member, roleName);
                        }
                        else
                        {
                            _logger.LogWarning("Failed to add member {MemberName} to role {RoleName}", member, roleName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error adding members to role {RoleName}. Rolling back role creation", roleName);
                    
                    foreach (var addedMember in addedMembers)
                    {
                        try
                        {
                            await _roleRepository.RemoveMemberFromRoleAsync(roleName, addedMember, databaseName, serverInstance);
                        }
                        catch (Exception rollbackEx)
                        {
                            _logger.LogError(rollbackEx, "Failed to rollback member {MemberName} from role {RoleName}", addedMember, roleName);
                        }
                    }

                    try
                    {
                        await _roleRepository.DropRoleAsync(roleName, databaseName, serverInstance);
                    }
                    catch (Exception rollbackEx)
                    {
                        _logger.LogError(rollbackEx, "Failed to rollback role creation for {RoleName}", roleName);
                    }

                    throw new SqlServerRoleException($"Failed to add initial members to role '{roleName}'. Role creation rolled back.", ex);
                }
            }

            var role = await _roleRepository.GetRoleAsync(roleName, databaseName, serverInstance);
            if (role == null)
            {
                throw new SqlServerRoleException($"Database role '{roleName}' was created but could not be retrieved");
            }

            _logger.LogInformation("Successfully created database role {RoleName} with transaction in database {DatabaseName} on server {ServerInstance}", 
                roleName, databaseName, serverInstance);
            return role;
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Unexpected error creating database role {RoleName} with transaction in database {DatabaseName} on server {ServerInstance}", 
                roleName, databaseName, serverInstance);
            throw new SqlServerRoleException($"Unexpected error creating database role '{roleName}' with transaction: {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteRoleWithTransactionAsync(string roleName, string databaseName, string serverInstance, bool forceDelete = false, string? deletedBy = null)
    {
        try
        {
            _logger.LogInformation("Deleting database role {RoleName} with transaction from database {DatabaseName} on server {ServerInstance} (Force: {ForceDelete})", 
                roleName, databaseName, serverInstance, forceDelete);
            
            ValidateRoleName(roleName);
            ValidateDatabaseName(databaseName);
            ValidateServerInstance(serverInstance);

            var roleExists = await _roleRepository.RoleExistsAsync(roleName, databaseName, serverInstance);
            if (!roleExists)
            {
                _logger.LogWarning("Database role {RoleName} does not exist in database {DatabaseName} on server {ServerInstance}", 
                    roleName, databaseName, serverInstance);
                return false;
            }

            var role = await _roleRepository.GetRoleAsync(roleName, databaseName, serverInstance);
            if (role?.IsBuiltIn == true)
            {
                throw new SqlServerValidationException($"Cannot delete built-in database role '{roleName}'");
            }

            var members = await _roleRepository.GetRoleMembersAsync(roleName, databaseName, serverInstance);
            var membersList = members.ToList();

            if (membersList.Any())
            {
                if (!forceDelete)
                {
                    throw new SqlServerValidationException($"Cannot delete database role '{roleName}' because it has {membersList.Count} members. Use forceDelete=true to remove members automatically.");
                }

                _logger.LogInformation("Force deleting role {RoleName} with {MemberCount} members", roleName, membersList.Count);
                
                var removedMembers = new List<string>();
                try
                {
                    foreach (var member in membersList)
                    {
                        var memberRemoved = await _roleRepository.RemoveMemberFromRoleAsync(roleName, member, databaseName, serverInstance);
                        if (memberRemoved)
                        {
                            removedMembers.Add(member);
                            _logger.LogDebug("Removed member {MemberName} from role {RoleName}", member, roleName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error removing members from role {RoleName}. Rolling back member removals", roleName);
                    
                    foreach (var removedMember in removedMembers)
                    {
                        try
                        {
                            await _roleRepository.AddMemberToRoleAsync(roleName, removedMember, databaseName, serverInstance);
                        }
                        catch (Exception rollbackEx)
                        {
                            _logger.LogError(rollbackEx, "Failed to rollback member {MemberName} to role {RoleName}", removedMember, roleName);
                        }
                    }

                    throw new SqlServerRoleException($"Failed to remove members from role '{roleName}'. Member removals rolled back.", ex);
                }
            }

            var success = await _roleRepository.DropRoleAsync(roleName, databaseName, serverInstance);
            if (success)
            {
                _logger.LogInformation("Successfully deleted database role {RoleName} with transaction from database {DatabaseName} on server {ServerInstance}", 
                    roleName, databaseName, serverInstance);
            }
            else
            {
                _logger.LogWarning("Failed to delete database role {RoleName} from database {DatabaseName} on server {ServerInstance}", 
                    roleName, databaseName, serverInstance);

                if (forceDelete && membersList.Any())
                {
                    _logger.LogWarning("Attempting to restore members to role {RoleName} after failed deletion", roleName);
                    foreach (var member in membersList)
                    {
                        try
                        {
                            await _roleRepository.AddMemberToRoleAsync(roleName, member, databaseName, serverInstance);
                        }
                        catch (Exception restoreEx)
                        {
                            _logger.LogError(restoreEx, "Failed to restore member {MemberName} to role {RoleName}", member, roleName);
                        }
                    }
                }
            }

            return success;
        }
        catch (Exception ex) when (!(ex is SqlServerManagementException))
        {
            _logger.LogError(ex, "Unexpected error deleting database role {RoleName} with transaction from database {DatabaseName} on server {ServerInstance}", 
                roleName, databaseName, serverInstance);
            throw new SqlServerRoleException($"Unexpected error deleting database role '{roleName}' with transaction: {ex.Message}", ex);
        }
    }

    private async Task ValidateRoleCreationAsync(string roleName, string databaseName, string serverInstance, string? ownerName)
    {
        ValidateRoleName(roleName);
        ValidateDatabaseName(databaseName);
        ValidateServerInstance(serverInstance);

        if (!string.IsNullOrEmpty(ownerName))
        {
            ValidateOwnerName(ownerName);
            var ownerExists = await _userRepository.UserExistsAsync(ownerName, databaseName, serverInstance);
            if (!ownerExists)
            {
                throw new SqlServerValidationException($"Owner '{ownerName}' does not exist in database '{databaseName}' on server '{serverInstance}'");
            }
        }

        var roleExists = await _roleRepository.RoleExistsAsync(roleName, databaseName, serverInstance);
        if (roleExists)
        {
            throw new SqlServerValidationException($"Database role '{roleName}' already exists in database '{databaseName}' on server '{serverInstance}'");
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

        var builtInDatabaseRoles = new[] { "db_owner", "db_accessadmin", "db_securityadmin", "db_ddladmin", "db_backupoperator", "db_datareader", "db_datawriter", "db_denydatareader", "db_denydatawriter" };
        if (builtInDatabaseRoles.Contains(roleName, StringComparer.OrdinalIgnoreCase))
        {
            throw new SqlServerValidationException($"Role name '{roleName}' is a built-in database role and cannot be created");
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

    private static void ValidateOwnerName(string ownerName)
    {
        if (string.IsNullOrWhiteSpace(ownerName))
        {
            throw new SqlServerValidationException("Owner name cannot be null or empty");
        }

        if (ownerName.Length > 128)
        {
            throw new SqlServerValidationException("Owner name cannot exceed 128 characters");
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
}