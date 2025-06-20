using Dapper;
using IAMBuddy.SqlServerManagementService.Models;
using IAMBuddy.SqlServerManagementService.Services;

namespace IAMBuddy.SqlServerManagementService.Repositories;

public class DatabaseUserRepository : IDatabaseUserRepository
{
    private readonly ISqlServerConnectionService _connectionService;
    private readonly ILogger<DatabaseUserRepository> _logger;

    public DatabaseUserRepository(ISqlServerConnectionService connectionService, ILogger<DatabaseUserRepository> logger)
    {
        _connectionService = connectionService;
        _logger = logger;
    }

    public async Task<bool> CreateUserAsync(string userName, string loginName, string databaseName, string serverInstance, string? defaultSchema = null)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance, databaseName);
            
            var sql = $"CREATE USER [{userName}] FOR LOGIN [{loginName}]";
            
            if (!string.IsNullOrEmpty(defaultSchema))
            {
                sql += $" WITH DEFAULT_SCHEMA = [{defaultSchema}]";
            }

            await connection.ExecuteAsync(sql);
            
            _logger.LogInformation("Successfully created user {UserName} for login {LoginName} in database {DatabaseName} on {ServerInstance}", 
                userName, loginName, databaseName, serverInstance);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create user {UserName} for login {LoginName} in database {DatabaseName} on {ServerInstance}", 
                userName, loginName, databaseName, serverInstance);
            return false;
        }
    }

    public async Task<bool> CreateUserWithoutLoginAsync(string userName, string databaseName, string serverInstance, string? defaultSchema = null)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance, databaseName);
            
            var sql = $"CREATE USER [{userName}] WITHOUT LOGIN";
            
            if (!string.IsNullOrEmpty(defaultSchema))
            {
                sql += $" WITH DEFAULT_SCHEMA = [{defaultSchema}]";
            }

            await connection.ExecuteAsync(sql);
            
            _logger.LogInformation("Successfully created user {UserName} without login in database {DatabaseName} on {ServerInstance}", 
                userName, databaseName, serverInstance);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create user {UserName} without login in database {DatabaseName} on {ServerInstance}", 
                userName, databaseName, serverInstance);
            return false;
        }
    }

    public async Task<SqlServerUser?> GetUserAsync(string userName, string databaseName, string serverInstance)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance, databaseName);
            
            const string sql = @"
                SELECT 
                    dp.name as UserName,
                    dp.type_desc as UserType,
                    dp.default_schema_name as DefaultSchemaName,
                    dp.create_date as CreatedDate,
                    dp.modify_date as ModifiedDate,
                    sp.name as LoginName
                FROM sys.database_principals dp
                    LEFT JOIN sys.server_principals sp ON dp.sid = sp.sid
                WHERE dp.name = @UserName 
                    AND dp.type IN ('S', 'U', 'G')"; // SQL User, Windows User, Windows Group

            var userData = await connection.QuerySingleOrDefaultAsync(sql, new { UserName = userName });
            
            if (userData == null)
                return null;

            return new SqlServerUser
            {
                UserName = userData.UserName,
                DatabaseName = databaseName,
                ServerInstance = serverInstance,
                DefaultSchema = userData.DefaultSchemaName,
                CreatedDate = userData.CreatedDate,
                ModifiedDate = userData.ModifiedDate
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user {UserName} in database {DatabaseName} on {ServerInstance}", 
                userName, databaseName, serverInstance);
            return null;
        }
    }

    public async Task<IEnumerable<SqlServerUser>> GetUsersAsync(string databaseName, string serverInstance)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance, databaseName);
            
            const string sql = @"
                SELECT 
                    dp.name as UserName,
                    dp.type_desc as UserType,
                    dp.default_schema_name as DefaultSchemaName,
                    dp.create_date as CreatedDate,
                    dp.modify_date as ModifiedDate,
                    sp.name as LoginName
                FROM sys.database_principals dp
                    LEFT JOIN sys.server_principals sp ON dp.sid = sp.sid
                WHERE dp.type IN ('S', 'U', 'G')
                    AND dp.name NOT IN ('dbo', 'guest', 'INFORMATION_SCHEMA', 'sys')
                    AND dp.name NOT LIKE '##%'
                ORDER BY dp.name";

            var userData = await connection.QueryAsync(sql);
            
            return userData.Select(data => new SqlServerUser
            {
                UserName = data.UserName,
                DatabaseName = databaseName,
                ServerInstance = serverInstance,
                DefaultSchema = data.DefaultSchemaName,
                CreatedDate = data.CreatedDate,
                ModifiedDate = data.ModifiedDate
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get users in database {DatabaseName} on {ServerInstance}", databaseName, serverInstance);
            return Enumerable.Empty<SqlServerUser>();
        }
    }

    public async Task<bool> DropUserAsync(string userName, string databaseName, string serverInstance)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance, databaseName);
            
            var sql = $"DROP USER [{userName}]";
            await connection.ExecuteAsync(sql);
            
            _logger.LogInformation("Successfully dropped user {UserName} from database {DatabaseName} on {ServerInstance}", 
                userName, databaseName, serverInstance);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to drop user {UserName} from database {DatabaseName} on {ServerInstance}", 
                userName, databaseName, serverInstance);
            return false;
        }
    }

    public async Task<bool> AlterUserDefaultSchemaAsync(string userName, string defaultSchema, string databaseName, string serverInstance)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance, databaseName);
            
            var sql = $"ALTER USER [{userName}] WITH DEFAULT_SCHEMA = [{defaultSchema}]";
            await connection.ExecuteAsync(sql);
            
            _logger.LogInformation("Successfully changed default schema for user {UserName} to {DefaultSchema} in database {DatabaseName} on {ServerInstance}", 
                userName, defaultSchema, databaseName, serverInstance);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to change default schema for user {UserName} to {DefaultSchema} in database {DatabaseName} on {ServerInstance}", 
                userName, defaultSchema, databaseName, serverInstance);
            return false;
        }
    }

    public async Task<bool> AddToRoleAsync(string userName, string roleName, string databaseName, string serverInstance)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance, databaseName);
            
            var sql = $"ALTER ROLE [{roleName}] ADD MEMBER [{userName}]";
            await connection.ExecuteAsync(sql);
            
            _logger.LogInformation("Successfully added user {UserName} to role {RoleName} in database {DatabaseName} on {ServerInstance}", 
                userName, roleName, databaseName, serverInstance);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add user {UserName} to role {RoleName} in database {DatabaseName} on {ServerInstance}", 
                userName, roleName, databaseName, serverInstance);
            return false;
        }
    }

    public async Task<bool> RemoveFromRoleAsync(string userName, string roleName, string databaseName, string serverInstance)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance, databaseName);
            
            var sql = $"ALTER ROLE [{roleName}] DROP MEMBER [{userName}]";
            await connection.ExecuteAsync(sql);
            
            _logger.LogInformation("Successfully removed user {UserName} from role {RoleName} in database {DatabaseName} on {ServerInstance}", 
                userName, roleName, databaseName, serverInstance);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove user {UserName} from role {RoleName} in database {DatabaseName} on {ServerInstance}", 
                userName, roleName, databaseName, serverInstance);
            return false;
        }
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(string userName, string databaseName, string serverInstance)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance, databaseName);
            
            const string sql = @"
                SELECT r.name as RoleName
                FROM sys.database_role_members rm
                    INNER JOIN sys.database_principals r ON rm.role_principal_id = r.principal_id
                    INNER JOIN sys.database_principals m ON rm.member_principal_id = m.principal_id
                WHERE m.name = @UserName";

            var roles = await connection.QueryAsync<string>(sql, new { UserName = userName });
            return roles;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get roles for user {UserName} in database {DatabaseName} on {ServerInstance}", 
                userName, databaseName, serverInstance);
            return Enumerable.Empty<string>();
        }
    }

    public async Task<bool> UserExistsAsync(string userName, string databaseName, string serverInstance)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance, databaseName);
            
            const string sql = @"
                SELECT COUNT(*)
                FROM sys.database_principals 
                WHERE name = @UserName 
                    AND type IN ('S', 'U', 'G')";

            var count = await connection.QuerySingleAsync<int>(sql, new { UserName = userName });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check if user {UserName} exists in database {DatabaseName} on {ServerInstance}", 
                userName, databaseName, serverInstance);
            return false;
        }
    }

    public async Task<bool> GrantPermissionAsync(string userName, string permission, string databaseName, string serverInstance, string? objectName = null)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance, databaseName);
            
            string sql;
            if (string.IsNullOrEmpty(objectName))
            {
                sql = $"GRANT {permission} TO [{userName}]";
            }
            else
            {
                sql = $"GRANT {permission} ON {objectName} TO [{userName}]";
            }
            
            await connection.ExecuteAsync(sql);
            
            _logger.LogInformation("Successfully granted {Permission} permission to user {UserName} in database {DatabaseName} on {ServerInstance}", 
                permission, userName, databaseName, serverInstance);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to grant {Permission} permission to user {UserName} in database {DatabaseName} on {ServerInstance}", 
                permission, userName, databaseName, serverInstance);
            return false;
        }
    }

    public async Task<bool> RevokePermissionAsync(string userName, string permission, string databaseName, string serverInstance, string? objectName = null)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance, databaseName);
            
            string sql;
            if (string.IsNullOrEmpty(objectName))
            {
                sql = $"REVOKE {permission} FROM [{userName}]";
            }
            else
            {
                sql = $"REVOKE {permission} ON {objectName} FROM [{userName}]";
            }
            
            await connection.ExecuteAsync(sql);
            
            _logger.LogInformation("Successfully revoked {Permission} permission from user {UserName} in database {DatabaseName} on {ServerInstance}", 
                permission, userName, databaseName, serverInstance);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to revoke {Permission} permission from user {UserName} in database {DatabaseName} on {ServerInstance}", 
                permission, userName, databaseName, serverInstance);
            return false;
        }
    }

    public async Task<IEnumerable<string>> GetUserPermissionsAsync(string userName, string databaseName, string serverInstance)
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
                WHERE pr.name = @UserName
                    AND p.state IN ('G', 'W')"; // Grant and Grant With Grant Option

            var permissions = await connection.QueryAsync<string>(sql, new { UserName = userName });
            return permissions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get permissions for user {UserName} in database {DatabaseName} on {ServerInstance}", 
                userName, databaseName, serverInstance);
            return Enumerable.Empty<string>();
        }
    }
}