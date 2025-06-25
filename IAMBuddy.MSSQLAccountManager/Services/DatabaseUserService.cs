using IAMBuddy.MSSQLAccountManager.Data;
using IAMBuddy.MSSQLAccountManager.Models;
using IAMBuddy.MSSQLAccountManager.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Dapper;

namespace IAMBuddy.MSSQLAccountManager.Services
{
    public class DatabaseUserService : IDatabaseUserService
    {
        private readonly MSSQLAccountManagerContext _context;
        private readonly IAuditService _auditService;
        private readonly ILogger<DatabaseUserService> _logger;

        public DatabaseUserService(
            MSSQLAccountManagerContext context,
            IAuditService auditService,
            ILogger<DatabaseUserService> logger)
        {
            _context = context;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<OperationResult> CreateUserAsync(DatabaseUser user)
        {
            var operation = await _auditService.LogOperationAsync(
                OperationType.Create, "DatabaseUser", user.UserName, user.ServerName, 
                user.DatabaseName, null, user.CreatedBy);

            try
            {
                var existingUser = await GetUserByNameAsync(user.ServerName, user.InstanceName, 
                    user.DatabaseName, user.UserName);
                if (existingUser != null)
                {
                    await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Failed, 
                        $"Database user '{user.UserName}' already exists in database '{user.DatabaseName}'");
                    return operation;
                }

                var connectionString = await GetConnectionStringAsync(user.ServerName, user.InstanceName, user.DatabaseName);
                using var connection = new SqlConnection(connectionString);
                
                var createUserSql = GenerateCreateUserSql(user);
                await connection.ExecuteAsync(createUserSql);

                _context.DatabaseUsers.Add(user);
                await _context.SaveChangesAsync();

                await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Success, 
                    null, $"Database user '{user.UserName}' created successfully in database '{user.DatabaseName}'");

                return operation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create database user {UserName} in database {DatabaseName}", 
                    user.UserName, user.DatabaseName);
                
                await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Failed, 
                    ex.Message, ex.ToString());
                
                return operation;
            }
        }

        public async Task<OperationResult> UpdateUserAsync(Guid userId, DatabaseUser updatedUser)
        {
            var existingUser = await _context.DatabaseUsers.FindAsync(userId);
            if (existingUser == null)
            {
                var operation = await _auditService.LogOperationAsync(
                    OperationType.Update, "DatabaseUser", "Unknown", "Unknown");
                await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Failed, 
                    $"Database user with ID '{userId}' not found");
                return operation;
            }

            var updateOperation = await _auditService.LogOperationAsync(
                OperationType.Update, "DatabaseUser", existingUser.UserName, existingUser.ServerName, 
                existingUser.DatabaseName, null, updatedUser.UpdatedBy);

            try
            {
                var connectionString = await GetConnectionStringAsync(existingUser.ServerName, 
                    existingUser.InstanceName, existingUser.DatabaseName);
                using var connection = new SqlConnection(connectionString);
                
                // Update default schema if changed
                if (!string.IsNullOrEmpty(updatedUser.DefaultSchema) && 
                    updatedUser.DefaultSchema != existingUser.DefaultSchema)
                {
                    var alterSchemaSql = $"ALTER USER [{existingUser.UserName}] WITH DEFAULT_SCHEMA = [{updatedUser.DefaultSchema}]";
                    await connection.ExecuteAsync(alterSchemaSql);
                }

                existingUser.DefaultSchema = updatedUser.DefaultSchema;
                existingUser.Description = updatedUser.Description;
                existingUser.UpdatedBy = updatedUser.UpdatedBy;
                existingUser.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                await _auditService.UpdateOperationResultAsync(updateOperation.Id, OperationStatus.Success, 
                    null, $"Database user '{existingUser.UserName}' updated successfully");

                return updateOperation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update database user {UserId}", userId);
                
                await _auditService.UpdateOperationResultAsync(updateOperation.Id, OperationStatus.Failed, 
                    ex.Message, ex.ToString());
                
                return updateOperation;
            }
        }

        public async Task<OperationResult> DeleteUserAsync(Guid userId)
        {
            var user = await _context.DatabaseUsers
                .Include(u => u.DatabaseRoleAssignments)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                var operation = await _auditService.LogOperationAsync(
                    OperationType.Delete, "DatabaseUser", "Unknown", "Unknown");
                await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Failed, 
                    $"Database user with ID '{userId}' not found");
                return operation;
            }

            var deleteOperation = await _auditService.LogOperationAsync(
                OperationType.Delete, "DatabaseUser", user.UserName, user.ServerName, user.DatabaseName);

            try
            {
                var connectionString = await GetConnectionStringAsync(user.ServerName, user.InstanceName, user.DatabaseName);
                using var connection = new SqlConnection(connectionString);
                
                await connection.ExecuteAsync($"DROP USER [{user.UserName}]");

                _context.DatabaseUsers.Remove(user);
                await _context.SaveChangesAsync();

                await _auditService.UpdateOperationResultAsync(deleteOperation.Id, OperationStatus.Success, 
                    null, $"Database user '{user.UserName}' deleted successfully from database '{user.DatabaseName}'");

                return deleteOperation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete database user {UserName} from database {DatabaseName}", 
                    user.UserName, user.DatabaseName);
                
                await _auditService.UpdateOperationResultAsync(deleteOperation.Id, OperationStatus.Failed, 
                    ex.Message, ex.ToString());
                
                return deleteOperation;
            }
        }

        public async Task<DatabaseUser?> GetUserByIdAsync(Guid userId)
        {
            return await _context.DatabaseUsers
                .Include(u => u.SqlLogin)
                .Include(u => u.DatabaseRoleAssignments)
                    .ThenInclude(dra => dra.DatabaseRole)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<DatabaseUser?> GetUserByNameAsync(string serverName, string? instanceName, 
            string databaseName, string userName)
        {
            return await _context.DatabaseUsers
                .Include(u => u.SqlLogin)
                .Include(u => u.DatabaseRoleAssignments)
                    .ThenInclude(dra => dra.DatabaseRole)
                .FirstOrDefaultAsync(u => u.ServerName == serverName && 
                                         u.InstanceName == instanceName && 
                                         u.DatabaseName == databaseName && 
                                         u.UserName == userName);
        }

        public async Task<IEnumerable<DatabaseUser>> GetAllUsersAsync(string serverName, 
            string? instanceName = null, string? databaseName = null)
        {
            var query = _context.DatabaseUsers
                .Include(u => u.SqlLogin)
                .Include(u => u.DatabaseRoleAssignments)
                    .ThenInclude(dra => dra.DatabaseRole)
                .Where(u => u.ServerName == serverName);

            if (instanceName != null)
                query = query.Where(u => u.InstanceName == instanceName);

            if (databaseName != null)
                query = query.Where(u => u.DatabaseName == databaseName);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<DatabaseUser>> GetUsersByLoginAsync(Guid loginId)
        {
            return await _context.DatabaseUsers
                .Include(u => u.SqlLogin)
                .Include(u => u.DatabaseRoleAssignments)
                    .ThenInclude(dra => dra.DatabaseRole)
                .Where(u => u.SqlLoginId == loginId)
                .ToListAsync();
        }

        private string GenerateCreateUserSql(DatabaseUser user)
        {
            var sql = $"CREATE USER [{user.UserName}]";

            switch (user.UserType)
            {
                case UserType.SqlUser when user.SqlLoginId.HasValue:
                    sql += $" FOR LOGIN [{user.LoginName}]";
                    break;
                case UserType.WindowsUser:
                    sql += $" FOR LOGIN [{user.LoginName}]";
                    break;
                case UserType.ContainedUser:
                    sql += " WITHOUT LOGIN";
                    break;
                case UserType.ExternalUser:
                    sql += " FROM EXTERNAL PROVIDER";
                    break;
            }

            if (!string.IsNullOrEmpty(user.DefaultSchema))
                sql += $" WITH DEFAULT_SCHEMA = [{user.DefaultSchema}]";

            return sql;
        }

        private async Task<string> GetConnectionStringAsync(string serverName, string? instanceName, string databaseName)
        {
            var serverInstance = await _context.ServerInstances
                .FirstOrDefaultAsync(si => si.ServerName == serverName && si.InstanceName == instanceName);
            
            if (serverInstance != null)
            {
                var builder = new SqlConnectionStringBuilder(serverInstance.ConnectionString)
                {
                    InitialCatalog = databaseName
                };
                return builder.ConnectionString;
            }

            var server = instanceName != null ? $"{serverName}\\{instanceName}" : serverName;
            return $"Server={server};Database={databaseName};Integrated Security=true;TrustServerCertificate=true;";
        }
    }
}