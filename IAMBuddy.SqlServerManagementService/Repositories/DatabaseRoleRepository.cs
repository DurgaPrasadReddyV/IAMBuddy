using Dapper;
using IAMBuddy.SqlServerManagementService.Models;
using IAMBuddy.SqlServerManagementService.Services;

namespace IAMBuddy.SqlServerManagementService.Repositories;

public class DatabaseRoleRepository : IDatabaseRoleRepository
{
    private readonly ISqlServerConnectionService _connectionService;
    private readonly ILogger<DatabaseRoleRepository> _logger;

    public DatabaseRoleRepository(ISqlServerConnectionService connectionService, ILogger<DatabaseRoleRepository> logger)
    {
        _connectionService = connectionService;
        _logger = logger;
    }

    public async Task<bool> CreateRoleAsync(string roleName, string databaseName, string serverInstance, string? ownerName = null)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance, databaseName);
            
            var sql = $"CREATE ROLE [{roleName}]";
            
            if (!string.IsNullOrEmpty(ownerName))
            {
                sql += $" AUTHORIZATION [{ownerName}]";
            }

            await connection.ExecuteAsync(sql);
            
            _logger.LogInformation("Successfully created role {RoleName} in database {DatabaseName} on {ServerInstance}", 
                roleName, databaseName, serverInstance);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create role {RoleName} in database {DatabaseName} on {ServerInstance}", 
                roleName, databaseName, serverInstance);
            return false;
        }
    }

    public async Task<SqlServerRole?> GetRoleAsync(string roleName, string databaseName, string serverInstance)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance, databaseName);
            
            const string sql = @"
                SELECT 
                    dp.name as RoleName,
                    dp.type_desc as RoleType,
                    dp.is_fixed_role,
                    dp.create_date as CreatedDate,
                    dp.modify_date as ModifiedDate,
                    owner.name as OwnerName
                FROM sys.database_principals dp
                    LEFT JOIN sys.database_principals owner ON dp.owning_principal_id = owner.principal_id
                WHERE dp.name = @RoleName 
                    AND dp.type = 'R'"; // Database Role

            var roleData = await connection.QuerySingleOrDefaultAsync(sql, new { RoleName = roleName });
            
            if (roleData == null)
                return null;

            return new SqlServerRole
            {
                RoleName = roleData.RoleName,
                RoleType = roleData.RoleType,
                ServerInstance = serverInstance,
                DatabaseName = databaseName,
                CreatedDate = roleData.CreatedDate,
                ModifiedDate = roleData.ModifiedDate
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get role {RoleName} in database {DatabaseName} on {ServerInstance}", 
                roleName, databaseName, serverInstance);
            return null;
        }
    }

    public async Task<IEnumerable<SqlServerRole>> GetRolesAsync(string databaseName, string serverInstance)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance, databaseName);
            
            const string sql = @"
                SELECT 
                    dp.name as RoleName,
                    dp.type_desc as RoleType,
                    dp.is_fixed_role,
                    dp.create_date as CreatedDate,
                    dp.modify_date as ModifiedDate,
                    owner.name as OwnerName
                FROM sys.database_principals dp
                    LEFT JOIN sys.database_principals owner ON dp.owning_principal_id = owner.principal_id
                WHERE dp.type = 'R'
                    AND dp.name NOT IN ('public')
                ORDER BY dp.name";

            var roleData = await connection.QueryAsync(sql);
            
            return roleData.Select(data => new SqlServerRole
            {
                RoleName = data.RoleName,
                RoleType = data.RoleType,
                ServerInstance = serverInstance,
                DatabaseName = databaseName,
                CreatedDate = data.CreatedDate,
                ModifiedDate = data.ModifiedDate
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get roles in database {DatabaseName} on {ServerInstance}", databaseName, serverInstance);
            return Enumerable.Empty<SqlServerRole>();
        }
    }

    public async Task<bool> DropRoleAsync(string roleName, string databaseName, string serverInstance)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance, databaseName);
            
            var sql = $"DROP ROLE [{roleName}]";
            await connection.ExecuteAsync(sql);
            
            _logger.LogInformation("Successfully dropped role {RoleName} from database {DatabaseName} on {ServerInstance}", 
                roleName, databaseName, serverInstance);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to drop role {RoleName} from database {DatabaseName} on {ServerInstance}", 
                roleName, databaseName, serverInstance);
            return false;
        }
    }

    public async Task<bool> AddMemberToRoleAsync(string roleName, string memberName, string databaseName, string serverInstance)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance, databaseName);
            
            var sql = $"ALTER ROLE [{roleName}] ADD MEMBER [{memberName}]";
            await connection.ExecuteAsync(sql);
            
            _logger.LogInformation("Successfully added member {MemberName} to role {RoleName} in database {DatabaseName} on {ServerInstance}", 
                memberName, roleName, databaseName, serverInstance);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add member {MemberName} to role {RoleName} in database {DatabaseName} on {ServerInstance}", 
                memberName, roleName, databaseName, serverInstance);
            return false;
        }
    }

    public async Task<bool> RemoveMemberFromRoleAsync(string roleName, string memberName, string databaseName, string serverInstance)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance, databaseName);
            
            var sql = $"ALTER ROLE [{roleName}] DROP MEMBER [{memberName}]";
            await connection.ExecuteAsync(sql);
            
            _logger.LogInformation("Successfully removed member {MemberName} from role {RoleName} in database {DatabaseName} on {ServerInstance}", 
                memberName, roleName, databaseName, serverInstance);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove member {MemberName} from role {RoleName} in database {DatabaseName} on {ServerInstance}", 
                memberName, roleName, databaseName, serverInstance);
            return false;
        }
    }

    public async Task<IEnumerable<string>> GetRoleMembersAsync(string roleName, string databaseName, string serverInstance)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance, databaseName);
            
            const string sql = @"
                SELECT m.name as MemberName
                FROM sys.database_role_members rm
                    INNER JOIN sys.database_principals r ON rm.role_principal_id = r.principal_id
                    INNER JOIN sys.database_principals m ON rm.member_principal_id = m.principal_id
                WHERE r.name = @RoleName";

            var members = await connection.QueryAsync<string>(sql, new { RoleName = roleName });
            return members;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get members for role {RoleName} in database {DatabaseName} on {ServerInstance}", 
                roleName, databaseName, serverInstance);
            return Enumerable.Empty<string>();
        }
    }

    public async Task<bool> RoleExistsAsync(string roleName, string databaseName, string serverInstance)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance, databaseName);
            
            const string sql = @"
                SELECT COUNT(*)
                FROM sys.database_principals 
                WHERE name = @RoleName 
                    AND type = 'R'";

            var count = await connection.QuerySingleAsync<int>(sql, new { RoleName = roleName });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check if role {RoleName} exists in database {DatabaseName} on {ServerInstance}", 
                roleName, databaseName, serverInstance);
            return false;
        }
    }

    public async Task<bool> GrantPermissionToRoleAsync(string roleName, string permission, string databaseName, string serverInstance, string? objectName = null)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance, databaseName);
            
            string sql;
            if (string.IsNullOrEmpty(objectName))
            {
                sql = $"GRANT {permission} TO [{roleName}]";
            }
            else
            {
                sql = $"GRANT {permission} ON {objectName} TO [{roleName}]";
            }
            
            await connection.ExecuteAsync(sql);
            
            _logger.LogInformation("Successfully granted {Permission} permission to role {RoleName} in database {DatabaseName} on {ServerInstance}", 
                permission, roleName, databaseName, serverInstance);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to grant {Permission} permission to role {RoleName} in database {DatabaseName} on {ServerInstance}", 
                permission, roleName, databaseName, serverInstance);
            return false;
        }
    }

    public async Task<bool> RevokePermissionFromRoleAsync(string roleName, string permission, string databaseName, string serverInstance, string? objectName = null)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance, databaseName);
            
            string sql;
            if (string.IsNullOrEmpty(objectName))
            {
                sql = $"REVOKE {permission} FROM [{roleName}]";
            }
            else
            {
                sql = $"REVOKE {permission} ON {objectName} FROM [{roleName}]";
            }
            
            await connection.ExecuteAsync(sql);
            
            _logger.LogInformation("Successfully revoked {Permission} permission from role {RoleName} in database {DatabaseName} on {ServerInstance}", 
                permission, roleName, databaseName, serverInstance);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to revoke {Permission} permission from role {RoleName} in database {DatabaseName} on {ServerInstance}", 
                permission, roleName, databaseName, serverInstance);
            return false;
        }
    }

    public async Task<IEnumerable<string>> GetRolePermissionsAsync(string roleName, string databaseName, string serverInstance)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance, databaseName);
            
            const string sql = @"
                SELECT 
                    p.permission_name + CASE 
                        WHEN p.class_desc = 'DATABASE' THEN ''
                        WHEN p.class_desc = 'OBJECT_OR_COLUMN' THEN ' ON ' + OBJECT_NAME(p.major_id)
                        WHEN p.class_desc = 'SCHEMA' THEN ' ON SCHEMA::' + SCHEMA_NAME(p.major_id)
                        WHEN p.class_desc = 'PRINCIPAL' THEN ' ON ' + pr2.name
                        ELSE ' ON ' + p.class_desc + '::' + CAST(p.major_id AS VARCHAR)
                    END as Permission
                FROM sys.database_permissions p
                    INNER JOIN sys.database_principals pr ON p.grantee_principal_id = pr.principal_id
                    LEFT JOIN sys.database_principals pr2 ON p.major_id = pr2.principal_id AND p.class_desc = 'PRINCIPAL'
                WHERE pr.name = @RoleName
                    AND p.state IN ('G', 'W')"; // Grant and Grant With Grant Option

            var permissions = await connection.QueryAsync<string>(sql, new { RoleName = roleName });
            return permissions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get permissions for role {RoleName} in database {DatabaseName} on {ServerInstance}", 
                roleName, databaseName, serverInstance);
            return Enumerable.Empty<string>();
        }
    }

    public async Task<bool> AlterRoleOwnerAsync(string roleName, string newOwner, string databaseName, string serverInstance)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance, databaseName);
            
            var sql = $"ALTER AUTHORIZATION ON ROLE::[{roleName}] TO [{newOwner}]";
            await connection.ExecuteAsync(sql);
            
            _logger.LogInformation("Successfully changed owner of role {RoleName} to {NewOwner} in database {DatabaseName} on {ServerInstance}", 
                roleName, newOwner, databaseName, serverInstance);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to change owner of role {RoleName} to {NewOwner} in database {DatabaseName} on {ServerInstance}", 
                roleName, newOwner, databaseName, serverInstance);
            return false;
        }
    }
}