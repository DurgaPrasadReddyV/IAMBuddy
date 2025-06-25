using IAMBuddy.MSSQLAccountManager.Data;
using IAMBuddy.MSSQLAccountManager.Models;
using IAMBuddy.MSSQLAccountManager.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Dapper;

namespace IAMBuddy.MSSQLAccountManager.Services
{
    public class SqlLoginService : ISqlLoginService
    {
        private readonly MSSQLAccountManagerContext _context;
        private readonly IAuditService _auditService;
        private readonly ILogger<SqlLoginService> _logger;

        public SqlLoginService(
            MSSQLAccountManagerContext context,
            IAuditService auditService,
            ILogger<SqlLoginService> logger)
        {
            _context = context;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<OperationResult> CreateLoginAsync(SqlLogin login, string? password = null)
        {
            var operation = await _auditService.LogOperationAsync(
                OperationType.Create, "SqlLogin", login.LoginName, login.ServerName, null, null, login.CreatedBy);

            try
            {
                // Check if login already exists
                var existingLogin = await GetLoginByNameAsync(login.ServerName, login.InstanceName, login.LoginName);
                if (existingLogin != null)
                {
                    var errorMsg = $"Login '{login.LoginName}' already exists on server '{login.ServerName}'";
                    await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Failed, errorMsg);
                    return operation;
                }

                // Create login in SQL Server
                var connectionString = await GetConnectionStringAsync(login.ServerName, login.InstanceName);
                using var connection = new SqlConnection(connectionString);
                
                var createLoginSql = GenerateCreateLoginSql(login, password);
                await connection.ExecuteAsync(createLoginSql);

                // Save to our tracking database
                _context.SqlLogins.Add(login);
                await _context.SaveChangesAsync();

                await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Success, 
                    null, $"Login '{login.LoginName}' created successfully");

                _logger.LogInformation("Successfully created SQL login {LoginName} on {ServerName}", 
                    login.LoginName, login.ServerName);

                return operation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create SQL login {LoginName} on {ServerName}", 
                    login.LoginName, login.ServerName);
                
                await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Failed, 
                    ex.Message, ex.ToString());
                
                return operation;
            }
        }

        public async Task<OperationResult> UpdateLoginAsync(Guid loginId, SqlLogin updatedLogin)
        {
            var operation = await _auditService.LogOperationAsync(
                OperationType.Update, "SqlLogin", updatedLogin.LoginName, updatedLogin.ServerName, 
                null, null, updatedLogin.UpdatedBy);

            try
            {
                var existingLogin = await _context.SqlLogins.FindAsync(loginId);
                if (existingLogin == null)
                {
                    var errorMsg = $"Login with ID '{loginId}' not found";
                    await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Failed, errorMsg);
                    return operation;
                }

                // Update SQL Server login properties
                var connectionString = await GetConnectionStringAsync(existingLogin.ServerName, existingLogin.InstanceName);
                using var connection = new SqlConnection(connectionString);
                
                var updateLoginSql = GenerateUpdateLoginSql(existingLogin.LoginName, updatedLogin);
                await connection.ExecuteAsync(updateLoginSql);

                // Update tracking database
                existingLogin.DefaultDatabase = updatedLogin.DefaultDatabase;
                existingLogin.DefaultLanguage = updatedLogin.DefaultLanguage;
                existingLogin.Description = updatedLogin.Description;
                existingLogin.UpdatedBy = updatedLogin.UpdatedBy;
                existingLogin.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Success, 
                    null, $"Login '{existingLogin.LoginName}' updated successfully");

                return operation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update SQL login {LoginId}", loginId);
                
                await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Failed, 
                    ex.Message, ex.ToString());
                
                return operation;
            }
        }

        public async Task<OperationResult> DeleteLoginAsync(Guid loginId)
        {
            var login = await _context.SqlLogins
                .Include(l => l.DatabaseUsers)
                .Include(l => l.ServerRoleAssignments)
                .FirstOrDefaultAsync(l => l.Id == loginId);

            if (login == null)
            {
                var operation = await _auditService.LogOperationAsync(
                    OperationType.Delete, "SqlLogin", "Unknown", "Unknown");
                await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Failed, 
                    $"Login with ID '{loginId}' not found");
                return operation;
            }

            var deleteOperation = await _auditService.LogOperationAsync(
                OperationType.Delete, "SqlLogin", login.LoginName, login.ServerName);

            try
            {
                // Delete from SQL Server
                var connectionString = await GetConnectionStringAsync(login.ServerName, login.InstanceName);
                using var connection = new SqlConnection(connectionString);
                
                await connection.ExecuteAsync($"DROP LOGIN [{login.LoginName}]");

                // Remove from tracking database (cascading deletes will handle related records)
                _context.SqlLogins.Remove(login);
                await _context.SaveChangesAsync();

                await _auditService.UpdateOperationResultAsync(deleteOperation.Id, OperationStatus.Success, 
                    null, $"Login '{login.LoginName}' and all associated users deleted successfully");

                return deleteOperation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete SQL login {LoginName}", login.LoginName);
                
                await _auditService.UpdateOperationResultAsync(deleteOperation.Id, OperationStatus.Failed, 
                    ex.Message, ex.ToString());
                
                return deleteOperation;
            }
        }

        public async Task<SqlLogin?> GetLoginByIdAsync(Guid loginId)
        {
            return await _context.SqlLogins
                .Include(l => l.DatabaseUsers)
                .Include(l => l.ServerRoleAssignments)
                    .ThenInclude(sra => sra.ServerRole)
                .FirstOrDefaultAsync(l => l.Id == loginId);
        }

        public async Task<SqlLogin?> GetLoginByNameAsync(string serverName, string? instanceName, string loginName)
        {
            return await _context.SqlLogins
                .Include(l => l.DatabaseUsers)
                .Include(l => l.ServerRoleAssignments)
                    .ThenInclude(sra => sra.ServerRole)
                .FirstOrDefaultAsync(l => l.ServerName == serverName && 
                                         l.InstanceName == instanceName && 
                                         l.LoginName == loginName);
        }

        public async Task<IEnumerable<SqlLogin>> GetAllLoginsAsync(string serverName, string? instanceName = null)
        {
            var query = _context.SqlLogins
                .Include(l => l.DatabaseUsers)
                .Include(l => l.ServerRoleAssignments)
                    .ThenInclude(sra => sra.ServerRole)
                .Where(l => l.ServerName == serverName);

            if (instanceName != null)
            {
                query = query.Where(l => l.InstanceName == instanceName);
            }

            return await query.ToListAsync();
        }

        public async Task<OperationResult> EnableLoginAsync(Guid loginId)
        {
            return await SetLoginStatusAsync(loginId, true);
        }

        public async Task<OperationResult> DisableLoginAsync(Guid loginId)
        {
            return await SetLoginStatusAsync(loginId, false);
        }

        public async Task<OperationResult> ChangePasswordAsync(Guid loginId, string newPassword)
        {
            var login = await _context.SqlLogins.FindAsync(loginId);
            if (login == null)
            {
                var operation = await _auditService.LogOperationAsync(
                    OperationType.Update, "SqlLogin", "Unknown", "Unknown");
                await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Failed, 
                    $"Login with ID '{loginId}' not found");
                return operation;
            }

            var passwordOperation = await _auditService.LogOperationAsync(
                OperationType.Update, "SqlLogin", login.LoginName, login.ServerName, 
                null, "Password change", login.UpdatedBy);

            try
            {
                var connectionString = await GetConnectionStringAsync(login.ServerName, login.InstanceName);
                using var connection = new SqlConnection(connectionString);
                
                var sql = $"ALTER LOGIN [{login.LoginName}] WITH PASSWORD = '{newPassword}'";
                await connection.ExecuteAsync(sql);

                await _auditService.UpdateOperationResultAsync(passwordOperation.Id, OperationStatus.Success, 
                    null, $"Password changed for login '{login.LoginName}'");

                return passwordOperation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to change password for SQL login {LoginName}", login.LoginName);
                
                await _auditService.UpdateOperationResultAsync(passwordOperation.Id, OperationStatus.Failed, 
                    ex.Message, ex.ToString());
                
                return passwordOperation;
            }
        }

        private async Task<OperationResult> SetLoginStatusAsync(Guid loginId, bool enabled)
        {
            var login = await _context.SqlLogins.FindAsync(loginId);
            if (login == null)
            {
                var operation = await _auditService.LogOperationAsync(
                    OperationType.Update, "SqlLogin", "Unknown", "Unknown");
                await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Failed, 
                    $"Login with ID '{loginId}' not found");
                return operation;
            }

            var statusOperation = await _auditService.LogOperationAsync(
                OperationType.Update, "SqlLogin", login.LoginName, login.ServerName, 
                null, enabled ? "Enable login" : "Disable login", login.UpdatedBy);

            try
            {
                var connectionString = await GetConnectionStringAsync(login.ServerName, login.InstanceName);
                using var connection = new SqlConnection(connectionString);
                
                var sql = $"ALTER LOGIN [{login.LoginName}] {(enabled ? "ENABLE" : "DISABLE")}";
                await connection.ExecuteAsync(sql);

                login.IsEnabled = enabled;
                await _context.SaveChangesAsync();

                await _auditService.UpdateOperationResultAsync(statusOperation.Id, OperationStatus.Success, 
                    null, $"Login '{login.LoginName}' {(enabled ? "enabled" : "disabled")} successfully");

                return statusOperation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to {Action} SQL login {LoginName}", 
                    enabled ? "enable" : "disable", login.LoginName);
                
                await _auditService.UpdateOperationResultAsync(statusOperation.Id, OperationStatus.Failed, 
                    ex.Message, ex.ToString());
                
                return statusOperation;
            }
        }

        private string GenerateCreateLoginSql(SqlLogin login, string? password)
        {
            var sql = $"CREATE LOGIN [{login.LoginName}]";
            
            switch (login.LoginType)
            {
                case LoginType.SqlLogin:
                    sql += $" WITH PASSWORD = '{password ?? "TempPassword123!"}', CHECK_POLICY = {(login.IsPasswordPolicyEnforced ? "ON" : "OFF")}";
                    break;
                case LoginType.WindowsLogin:
                case LoginType.ActiveDirectoryLogin:
                    sql += " FROM WINDOWS";
                    break;
            }

            if (!string.IsNullOrEmpty(login.DefaultDatabase))
                sql += $", DEFAULT_DATABASE = [{login.DefaultDatabase}]";
            
            if (!string.IsNullOrEmpty(login.DefaultLanguage))
                sql += $", DEFAULT_LANGUAGE = [{login.DefaultLanguage}]";

            return sql;
        }

        private string GenerateUpdateLoginSql(string loginName, SqlLogin updatedLogin)
        {
            var sql = $"ALTER LOGIN [{loginName}] WITH";
            var clauses = new List<string>();

            if (!string.IsNullOrEmpty(updatedLogin.DefaultDatabase))
                clauses.Add($"DEFAULT_DATABASE = [{updatedLogin.DefaultDatabase}]");
            
            if (!string.IsNullOrEmpty(updatedLogin.DefaultLanguage))
                clauses.Add($"DEFAULT_LANGUAGE = [{updatedLogin.DefaultLanguage}]");

            return sql + " " + string.Join(", ", clauses);
        }

        private async Task<string> GetConnectionStringAsync(string serverName, string? instanceName)
        {
            var serverInstance = await _context.ServerInstances
                .FirstOrDefaultAsync(si => si.ServerName == serverName && si.InstanceName == instanceName);
            
            if (serverInstance != null)
                return serverInstance.ConnectionString;

            // Fallback to construct connection string
            var server = instanceName != null ? $"{serverName}\\{instanceName}" : serverName;
            return $"Server={server};Integrated Security=true;TrustServerCertificate=true;";
        }
    }
}