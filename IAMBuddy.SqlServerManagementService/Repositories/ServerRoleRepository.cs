using Dapper;
using IAMBuddy.SqlServerManagementService.Models;
using IAMBuddy.SqlServerManagementService.Services;

namespace IAMBuddy.SqlServerManagementService.Repositories;

public class ServerRoleRepository : IServerRoleRepository
{
    private readonly ISqlServerConnectionService _connectionService;
    private readonly ILogger<ServerRoleRepository> _logger;

    public ServerRoleRepository(ISqlServerConnectionService connectionService, ILogger<ServerRoleRepository> logger)
    {
        _connectionService = connectionService;
        _logger = logger;
    }

    public async Task<bool> CreateServerRoleAsync(string roleName, string serverInstance)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance);
            
            var sql = $"CREATE SERVER ROLE [{roleName}]";
            await connection.ExecuteAsync(sql);
            
            _logger.LogInformation("Successfully created server role {RoleName} on {ServerInstance}", roleName, serverInstance);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create server role {RoleName} on {ServerInstance}", roleName, serverInstance);
            return false;
        }
    }

    public async Task<SqlServerRole?> GetServerRoleAsync(string roleName, string serverInstance)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance);
            
            const string sql = @"
                SELECT 
                    name as RoleName,
                    type_desc as RoleType,
                    is_fixed_role,
                    create_date as CreatedDate,
                    modify_date as ModifiedDate
                FROM sys.server_principals 
                WHERE name = @RoleName 
                    AND type = 'R'"; // Server Role

            var roleData = await connection.QuerySingleOrDefaultAsync(sql, new { RoleName = roleName });
            
            if (roleData == null)
                return null;

            return new SqlServerRole
            {
                RoleName = roleData.RoleName,
                RoleType = roleData.RoleType,
                ServerInstance = serverInstance,
                DatabaseName = null, // Server roles don't have database context
                CreatedDate = roleData.CreatedDate,
                ModifiedDate = roleData.ModifiedDate
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get server role {RoleName} on {ServerInstance}", roleName, serverInstance);
            return null;
        }
    }

    public async Task<IEnumerable<SqlServerRole>> GetServerRolesAsync(string serverInstance)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance);
            
            const string sql = @"
                SELECT 
                    name as RoleName,
                    type_desc as RoleType,
                    is_fixed_role,
                    create_date as CreatedDate,
                    modify_date as ModifiedDate
                FROM sys.server_principals 
                WHERE type = 'R'
                ORDER BY name";

            var roleData = await connection.QueryAsync(sql);
            
            return roleData.Select(data => new SqlServerRole
            {
                RoleName = data.RoleName,
                RoleType = data.RoleType,
                ServerInstance = serverInstance,
                DatabaseName = null,
                CreatedDate = data.CreatedDate,
                ModifiedDate = data.ModifiedDate
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get server roles on {ServerInstance}", serverInstance);
            return Enumerable.Empty<SqlServerRole>();
        }
    }

    public async Task<bool> DropServerRoleAsync(string roleName, string serverInstance)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance);
            
            var sql = $"DROP SERVER ROLE [{roleName}]";
            await connection.ExecuteAsync(sql);
            
            _logger.LogInformation("Successfully dropped server role {RoleName} on {ServerInstance}", roleName, serverInstance);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to drop server role {RoleName} on {ServerInstance}", roleName, serverInstance);
            return false;
        }
    }

    public async Task<bool> AddMemberToRoleAsync(string roleName, string memberName, string serverInstance)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance);
            
            var sql = $"ALTER SERVER ROLE [{roleName}] ADD MEMBER [{memberName}]";
            await connection.ExecuteAsync(sql);
            
            _logger.LogInformation("Successfully added member {MemberName} to server role {RoleName} on {ServerInstance}", 
                memberName, roleName, serverInstance);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add member {MemberName} to server role {RoleName} on {ServerInstance}", 
                memberName, roleName, serverInstance);
            return false;
        }
    }

    public async Task<bool> RemoveMemberFromRoleAsync(string roleName, string memberName, string serverInstance)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance);
            
            var sql = $"ALTER SERVER ROLE [{roleName}] DROP MEMBER [{memberName}]";
            await connection.ExecuteAsync(sql);
            
            _logger.LogInformation("Successfully removed member {MemberName} from server role {RoleName} on {ServerInstance}", 
                memberName, roleName, serverInstance);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove member {MemberName} from server role {RoleName} on {ServerInstance}", 
                memberName, roleName, serverInstance);
            return false;
        }
    }

    public async Task<IEnumerable<string>> GetRoleMembersAsync(string roleName, string serverInstance)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance);
            
            const string sql = @"
                SELECT m.name as MemberName
                FROM sys.server_role_members rm
                    INNER JOIN sys.server_principals r ON rm.role_principal_id = r.principal_id
                    INNER JOIN sys.server_principals m ON rm.member_principal_id = m.principal_id
                WHERE r.name = @RoleName";

            var members = await connection.QueryAsync<string>(sql, new { RoleName = roleName });
            return members;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get members for server role {RoleName} on {ServerInstance}", roleName, serverInstance);
            return Enumerable.Empty<string>();
        }
    }

    public async Task<bool> ServerRoleExistsAsync(string roleName, string serverInstance)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance);
            
            const string sql = @"
                SELECT COUNT(*)
                FROM sys.server_principals 
                WHERE name = @RoleName 
                    AND type = 'R'";

            var count = await connection.QuerySingleAsync<int>(sql, new { RoleName = roleName });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check if server role {RoleName} exists on {ServerInstance}", roleName, serverInstance);
            return false;
        }
    }

    public async Task<bool> GrantPermissionToRoleAsync(string roleName, string permission, string serverInstance, string? objectName = null)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance);
            
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
            
            _logger.LogInformation("Successfully granted {Permission} permission to server role {RoleName} on {ServerInstance}", 
                permission, roleName, serverInstance);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to grant {Permission} permission to server role {RoleName} on {ServerInstance}", 
                permission, roleName, serverInstance);
            return false;
        }
    }

    public async Task<bool> RevokePermissionFromRoleAsync(string roleName, string permission, string serverInstance, string? objectName = null)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance);
            
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
            
            _logger.LogInformation("Successfully revoked {Permission} permission from server role {RoleName} on {ServerInstance}", 
                permission, roleName, serverInstance);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to revoke {Permission} permission from server role {RoleName} on {ServerInstance}", 
                permission, roleName, serverInstance);
            return false;
        }
    }

    public async Task<IEnumerable<string>> GetRolePermissionsAsync(string roleName, string serverInstance)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance);
            
            const string sql = @"
                SELECT 
                    p.permission_name + CASE 
                        WHEN p.class_desc = 'SERVER' THEN ''
                        WHEN p.class_desc = 'OBJECT_OR_COLUMN' THEN ' ON ' + OBJECT_NAME(p.major_id)
                        WHEN p.class_desc = 'DATABASE' THEN ' ON DATABASE::' + DB_NAME(p.major_id)
                        WHEN p.class_desc = 'SCHEMA' THEN ' ON SCHEMA::' + SCHEMA_NAME(p.major_id)
                        ELSE ' ON ' + p.class_desc + '::' + CAST(p.major_id AS VARCHAR)
                    END as Permission
                FROM sys.server_permissions p
                    INNER JOIN sys.server_principals pr ON p.grantee_principal_id = pr.principal_id
                WHERE pr.name = @RoleName
                    AND p.state IN ('G', 'W')"; // Grant and Grant With Grant Option

            var permissions = await connection.QueryAsync<string>(sql, new { RoleName = roleName });
            return permissions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get permissions for server role {RoleName} on {ServerInstance}", roleName, serverInstance);
            return Enumerable.Empty<string>();
        }
    }
}