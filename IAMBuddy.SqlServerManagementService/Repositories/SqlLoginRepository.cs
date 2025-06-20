using Dapper;
using IAMBuddy.SqlServerManagementService.Models;
using IAMBuddy.SqlServerManagementService.Services;

namespace IAMBuddy.SqlServerManagementService.Repositories;

public class SqlLoginRepository : ISqlLoginRepository
{
    private readonly ISqlServerConnectionService _connectionService;
    private readonly ILogger<SqlLoginRepository> _logger;

    public SqlLoginRepository(ISqlServerConnectionService connectionService, ILogger<SqlLoginRepository> logger)
    {
        _connectionService = connectionService;
        _logger = logger;
    }

    public async Task<bool> CreateLoginAsync(string loginName, string password, string serverInstance, string? defaultDatabase = null)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance);
            
            var sql = $@"
                CREATE LOGIN [{loginName}] 
                WITH PASSWORD = @Password";
            
            if (!string.IsNullOrEmpty(defaultDatabase))
            {
                sql += $", DEFAULT_DATABASE = [{defaultDatabase}]";
            }

            var parameters = new { Password = password };
            await connection.ExecuteAsync(sql, parameters);
            
            _logger.LogInformation("Successfully created SQL login {LoginName} on {ServerInstance}", loginName, serverInstance);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create SQL login {LoginName} on {ServerInstance}", loginName, serverInstance);
            return false;
        }
    }

    public async Task<bool> CreateWindowsLoginAsync(string loginName, string serverInstance, string? defaultDatabase = null)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance);
            
            var sql = $@"
                CREATE LOGIN [{loginName}] 
                FROM WINDOWS";
            
            if (!string.IsNullOrEmpty(defaultDatabase))
            {
                sql += $" WITH DEFAULT_DATABASE = [{defaultDatabase}]";
            }

            await connection.ExecuteAsync(sql);
            
            _logger.LogInformation("Successfully created Windows login {LoginName} on {ServerInstance}", loginName, serverInstance);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create Windows login {LoginName} on {ServerInstance}", loginName, serverInstance);
            return false;
        }
    }

    public async Task<SqlServerLogin?> GetLoginAsync(string loginName, string serverInstance)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance);
            
            const string sql = @"
                SELECT 
                    name as LoginName,
                    type_desc as LoginType,
                    sid as Sid,
                    is_disabled,
                    is_policy_checked,
                    is_expiration_checked,
                    create_date as CreatedDate,
                    modify_date as ModifiedDate
                FROM sys.server_principals 
                WHERE name = @LoginName 
                    AND type IN ('S', 'U', 'G')"; // SQL Login, Windows User, Windows Group

            var loginData = await connection.QuerySingleOrDefaultAsync(sql, new { LoginName = loginName });
            
            if (loginData == null)
                return null;

            return new SqlServerLogin
            {
                LoginName = loginData.LoginName,
                LoginType = loginData.LoginType,
                Sid = Convert.ToBase64String(loginData.Sid),
                IsEnabled = !loginData.is_disabled,
                ServerInstance = serverInstance,
                CreatedDate = loginData.CreatedDate,
                ModifiedDate = loginData.ModifiedDate
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get login {LoginName} on {ServerInstance}", loginName, serverInstance);
            return null;
        }
    }

    public async Task<IEnumerable<SqlServerLogin>> GetLoginsAsync(string serverInstance)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance);
            
            const string sql = @"
                SELECT 
                    name as LoginName,
                    type_desc as LoginType,
                    sid as Sid,
                    is_disabled,
                    is_policy_checked,
                    is_expiration_checked,
                    create_date as CreatedDate,
                    modify_date as ModifiedDate
                FROM sys.server_principals 
                WHERE type IN ('S', 'U', 'G')
                    AND name NOT IN ('sa', 'guest', 'INFORMATION_SCHEMA', 'sys')
                    AND name NOT LIKE '##%'
                ORDER BY name";

            var loginData = await connection.QueryAsync(sql);
            
            return loginData.Select(data => new SqlServerLogin
            {
                LoginName = data.LoginName,
                LoginType = data.LoginType,
                Sid = Convert.ToBase64String(data.Sid),
                IsEnabled = !data.is_disabled,
                ServerInstance = serverInstance,
                CreatedDate = data.CreatedDate,
                ModifiedDate = data.ModifiedDate
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get logins on {ServerInstance}", serverInstance);
            return Enumerable.Empty<SqlServerLogin>();
        }
    }

    public async Task<bool> DropLoginAsync(string loginName, string serverInstance)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance);
            
            var sql = $"DROP LOGIN [{loginName}]";
            await connection.ExecuteAsync(sql);
            
            _logger.LogInformation("Successfully dropped login {LoginName} on {ServerInstance}", loginName, serverInstance);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to drop login {LoginName} on {ServerInstance}", loginName, serverInstance);
            return false;
        }
    }

    public async Task<bool> EnableLoginAsync(string loginName, string serverInstance)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance);
            
            var sql = $"ALTER LOGIN [{loginName}] ENABLE";
            await connection.ExecuteAsync(sql);
            
            _logger.LogInformation("Successfully enabled login {LoginName} on {ServerInstance}", loginName, serverInstance);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enable login {LoginName} on {ServerInstance}", loginName, serverInstance);
            return false;
        }
    }

    public async Task<bool> DisableLoginAsync(string loginName, string serverInstance)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance);
            
            var sql = $"ALTER LOGIN [{loginName}] DISABLE";
            await connection.ExecuteAsync(sql);
            
            _logger.LogInformation("Successfully disabled login {LoginName} on {ServerInstance}", loginName, serverInstance);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to disable login {LoginName} on {ServerInstance}", loginName, serverInstance);
            return false;
        }
    }

    public async Task<bool> ChangePasswordAsync(string loginName, string newPassword, string serverInstance)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance);
            
            var sql = $"ALTER LOGIN [{loginName}] WITH PASSWORD = @NewPassword";
            await connection.ExecuteAsync(sql, new { NewPassword = newPassword });
            
            _logger.LogInformation("Successfully changed password for login {LoginName} on {ServerInstance}", loginName, serverInstance);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to change password for login {LoginName} on {ServerInstance}", loginName, serverInstance);
            return false;
        }
    }

    public async Task<bool> AddToServerRoleAsync(string loginName, string roleName, string serverInstance)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance);
            
            var sql = $"ALTER SERVER ROLE [{roleName}] ADD MEMBER [{loginName}]";
            await connection.ExecuteAsync(sql);
            
            _logger.LogInformation("Successfully added login {LoginName} to server role {RoleName} on {ServerInstance}", 
                loginName, roleName, serverInstance);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add login {LoginName} to server role {RoleName} on {ServerInstance}", 
                loginName, roleName, serverInstance);
            return false;
        }
    }

    public async Task<bool> RemoveFromServerRoleAsync(string loginName, string roleName, string serverInstance)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance);
            
            var sql = $"ALTER SERVER ROLE [{roleName}] DROP MEMBER [{loginName}]";
            await connection.ExecuteAsync(sql);
            
            _logger.LogInformation("Successfully removed login {LoginName} from server role {RoleName} on {ServerInstance}", 
                loginName, roleName, serverInstance);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove login {LoginName} from server role {RoleName} on {ServerInstance}", 
                loginName, roleName, serverInstance);
            return false;
        }
    }

    public async Task<IEnumerable<string>> GetServerRolesForLoginAsync(string loginName, string serverInstance)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance);
            
            const string sql = @"
                SELECT r.name as RoleName
                FROM sys.server_role_members rm
                    INNER JOIN sys.server_principals r ON rm.role_principal_id = r.principal_id
                    INNER JOIN sys.server_principals m ON rm.member_principal_id = m.principal_id
                WHERE m.name = @LoginName";

            var roles = await connection.QueryAsync<string>(sql, new { LoginName = loginName });
            return roles;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get server roles for login {LoginName} on {ServerInstance}", loginName, serverInstance);
            return Enumerable.Empty<string>();
        }
    }

    public async Task<bool> LoginExistsAsync(string loginName, string serverInstance)
    {
        try
        {
            using var connection = await _connectionService.GetConnectionAsync(serverInstance);
            
            const string sql = @"
                SELECT COUNT(*)
                FROM sys.server_principals 
                WHERE name = @LoginName 
                    AND type IN ('S', 'U', 'G')";

            var count = await connection.QuerySingleAsync<int>(sql, new { LoginName = loginName });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check if login {LoginName} exists on {ServerInstance}", loginName, serverInstance);
            return false;
        }
    }
}