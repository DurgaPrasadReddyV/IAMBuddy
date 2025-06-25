using IAMBuddy.MSSQLAccountManager.Data;
using IAMBuddy.MSSQLAccountManager.Models;
using IAMBuddy.MSSQLAccountManager.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Dapper;

namespace IAMBuddy.MSSQLAccountManager.Services
{
    public class DatabaseRoleService : IDatabaseRoleService
    {
        private readonly MSSQLAccountManagerContext _context;
        private readonly IAuditService _auditService;
        private readonly ILogger<DatabaseRoleService> _logger;

        public DatabaseRoleService(
            MSSQLAccountManagerContext context,
            IAuditService auditService,
            ILogger<DatabaseRoleService> logger)
        {
            _context = context;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<OperationResult> CreateRoleAsync(DatabaseRole role)
        {
            var operation = await _auditService.LogOperationAsync(
                OperationType.Create, "DatabaseRole", role.RoleName, role.ServerName, 
                role.DatabaseName, null, role.CreatedBy);

            try
            {
                var existingRole = await GetRoleByNameAsync(role.ServerName, role.InstanceName, 
                    role.DatabaseName, role.RoleName);
                if (existingRole != null)
                {
                    await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Failed, 
                        $"Database role '{role.RoleName}' already exists in database '{role.DatabaseName}'");
                    return operation;
                }

                var connectionString = await GetConnectionStringAsync(role.ServerName, role.InstanceName, role.DatabaseName);
                using var connection = new SqlConnection(connectionString);
                
                await connection.ExecuteAsync($"CREATE ROLE [{role.RoleName}]");

                _context.DatabaseRoles.Add(role);
                await _context.SaveChangesAsync();

                await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Success, 
                    null, $"Database role '{role.RoleName}' created successfully in database '{role.DatabaseName}'");

                return operation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create database role {RoleName} in database {DatabaseName}", 
                    role.RoleName, role.DatabaseName);
                
                await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Failed, 
                    ex.Message, ex.ToString());
                
                return operation;
            }
        }

        public async Task<OperationResult> UpdateRoleAsync(Guid roleId, DatabaseRole updatedRole)
        {
            var existingRole = await _context.DatabaseRoles.FindAsync(roleId);
            if (existingRole == null)
            {
                var operation = await _auditService.LogOperationAsync(
                    OperationType.Update, "DatabaseRole", "Unknown", "Unknown");
                await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Failed, 
                    $"Database role with ID '{roleId}' not found");
                return operation;
            }

            var updateOperation = await _auditService.LogOperationAsync(
                OperationType.Update, "DatabaseRole", existingRole.RoleName, existingRole.ServerName, 
                existingRole.DatabaseName, null, updatedRole.UpdatedBy);

            try
            {
                existingRole.Description = updatedRole.Description;
                existingRole.UpdatedBy = updatedRole.UpdatedBy;
                existingRole.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                await _auditService.UpdateOperationResultAsync(updateOperation.Id, OperationStatus.Success, 
                    null, $"Database role '{existingRole.RoleName}' updated successfully");

                return updateOperation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update database role {RoleId}", roleId);
                
                await _auditService.UpdateOperationResultAsync(updateOperation.Id, OperationStatus.Failed, 
                    ex.Message, ex.ToString());
                
                return updateOperation;
            }
        }

        public async Task<OperationResult> DeleteRoleAsync(Guid roleId)
        {
            var role = await _context.DatabaseRoles
                .Include(r => r.DatabaseRoleAssignments)
                .FirstOrDefaultAsync(r => r.Id == roleId);

            if (role == null)
            {
                var operation = await _auditService.LogOperationAsync(
                    OperationType.Delete, "DatabaseRole", "Unknown", "Unknown");
                await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Failed, 
                    $"Database role with ID '{roleId}' not found");
                return operation;
            }

            var deleteOperation = await _auditService.LogOperationAsync(
                OperationType.Delete, "DatabaseRole", role.RoleName, role.ServerName, role.DatabaseName);

            try
            {
                if (role.IsFixedRole)
                {
                    await _auditService.UpdateOperationResultAsync(deleteOperation.Id, OperationStatus.Failed, 
                        $"Cannot delete fixed database role '{role.RoleName}'");
                    return deleteOperation;
                }

                var connectionString = await GetConnectionStringAsync(role.ServerName, role.InstanceName, role.DatabaseName);
                using var connection = new SqlConnection(connectionString);
                
                await connection.ExecuteAsync($"DROP ROLE [{role.RoleName}]");

                _context.DatabaseRoles.Remove(role);
                await _context.SaveChangesAsync();

                await _auditService.UpdateOperationResultAsync(deleteOperation.Id, OperationStatus.Success, 
                    null, $"Database role '{role.RoleName}' deleted successfully from database '{role.DatabaseName}'");

                return deleteOperation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete database role {RoleName} from database {DatabaseName}", 
                    role.RoleName, role.DatabaseName);
                
                await _auditService.UpdateOperationResultAsync(deleteOperation.Id, OperationStatus.Failed, 
                    ex.Message, ex.ToString());
                
                return deleteOperation;
            }
        }

        public async Task<DatabaseRole?> GetRoleByIdAsync(Guid roleId)
        {
            return await _context.DatabaseRoles
                .Include(r => r.DatabaseRoleAssignments)
                    .ThenInclude(dra => dra.DatabaseUser)
                .FirstOrDefaultAsync(r => r.Id == roleId);
        }

        public async Task<DatabaseRole?> GetRoleByNameAsync(string serverName, string? instanceName, 
            string databaseName, string roleName)
        {
            return await _context.DatabaseRoles
                .Include(r => r.DatabaseRoleAssignments)
                    .ThenInclude(dra => dra.DatabaseUser)
                .FirstOrDefaultAsync(r => r.ServerName == serverName && 
                                         r.InstanceName == instanceName && 
                                         r.DatabaseName == databaseName && 
                                         r.RoleName == roleName);
        }

        public async Task<IEnumerable<DatabaseRole>> GetAllRolesAsync(string serverName, 
            string? instanceName = null, string? databaseName = null)
        {
            var query = _context.DatabaseRoles
                .Include(r => r.DatabaseRoleAssignments)
                    .ThenInclude(dra => dra.DatabaseUser)
                .Where(r => r.ServerName == serverName);

            if (instanceName != null)
                query = query.Where(r => r.InstanceName == instanceName);

            if (databaseName != null)
                query = query.Where(r => r.DatabaseName == databaseName);

            return await query.ToListAsync();
        }

        public async Task<OperationResult> AssignRoleToUserAsync(Guid userId, Guid roleId)
        {
            var user = await _context.DatabaseUsers.FindAsync(userId);
            var role = await _context.DatabaseRoles.FindAsync(roleId);

            if (user == null || role == null)
            {
                var operation = await _auditService.LogOperationAsync(
                    OperationType.Assign, "DatabaseRoleAssignment", "Unknown", "Unknown");
                await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Failed, 
                    "Database user or role not found");
                return operation;
            }

            var assignOperation = await _auditService.LogOperationAsync(
                OperationType.Assign, "DatabaseRoleAssignment", $"{user.UserName} -> {role.RoleName}", 
                role.ServerName, role.DatabaseName, null, user.UpdatedBy);

            try
            {
                var existingAssignment = await _context.DatabaseRoleAssignments
                    .FirstOrDefaultAsync(dra => dra.DatabaseUserId == userId && dra.DatabaseRoleId == roleId);

                if (existingAssignment != null)
                {
                    if (existingAssignment.IsActive)
                    {
                        await _auditService.UpdateOperationResultAsync(assignOperation.Id, OperationStatus.Failed, 
                            $"Role '{role.RoleName}' is already assigned to user '{user.UserName}'");
                        return assignOperation;
                    }
                    else
                    {
                        existingAssignment.IsActive = true;
                        existingAssignment.RevokedAt = null;
                        existingAssignment.RevokedBy = null;
                        existingAssignment.RevokeReason = null;
                    }
                }
                else
                {
                    var assignment = new DatabaseRoleAssignment
                    {
                        DatabaseUserId = userId,
                        DatabaseRoleId = roleId,
                        ServerName = role.ServerName,
                        InstanceName = role.InstanceName,
                        DatabaseName = role.DatabaseName,
                        CreatedBy = user.UpdatedBy ?? "System"
                    };
                    _context.DatabaseRoleAssignments.Add(assignment);
                }

                var connectionString = await GetConnectionStringAsync(role.ServerName, role.InstanceName, role.DatabaseName);
                using var connection = new SqlConnection(connectionString);
                
                await connection.ExecuteAsync($"ALTER ROLE [{role.RoleName}] ADD MEMBER [{user.UserName}]");

                await _context.SaveChangesAsync();

                await _auditService.UpdateOperationResultAsync(assignOperation.Id, OperationStatus.Success, 
                    null, $"Role '{role.RoleName}' assigned to user '{user.UserName}' successfully");

                return assignOperation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to assign role {RoleName} to user {UserName} in database {DatabaseName}", 
                    role.RoleName, user.UserName, role.DatabaseName);
                
                await _auditService.UpdateOperationResultAsync(assignOperation.Id, OperationStatus.Failed, 
                    ex.Message, ex.ToString());
                
                return assignOperation;
            }
        }

        public async Task<OperationResult> RemoveRoleFromUserAsync(Guid userId, Guid roleId)
        {
            var assignment = await _context.DatabaseRoleAssignments
                .Include(dra => dra.DatabaseUser)
                .Include(dra => dra.DatabaseRole)
                .FirstOrDefaultAsync(dra => dra.DatabaseUserId == userId && dra.DatabaseRoleId == roleId && dra.IsActive);

            if (assignment == null)
            {
                var operation = await _auditService.LogOperationAsync(
                    OperationType.Remove, "DatabaseRoleAssignment", "Unknown", "Unknown");
                await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Failed, 
                    "Active database role assignment not found");
                return operation;
            }

            var removeOperation = await _auditService.LogOperationAsync(
                OperationType.Remove, "DatabaseRoleAssignment", 
                $"{assignment.DatabaseUser.UserName} -> {assignment.DatabaseRole.RoleName}", 
                assignment.ServerName, assignment.DatabaseName, null, assignment.DatabaseUser.UpdatedBy);

            try
            {
                var connectionString = await GetConnectionStringAsync(assignment.ServerName, 
                    assignment.InstanceName, assignment.DatabaseName);
                using var connection = new SqlConnection(connectionString);
                
                await connection.ExecuteAsync($"ALTER ROLE [{assignment.DatabaseRole.RoleName}] DROP MEMBER [{assignment.DatabaseUser.UserName}]");

                assignment.IsActive = false;
                assignment.RevokedAt = DateTime.UtcNow;
                assignment.RevokedBy = assignment.DatabaseUser.UpdatedBy ?? "System";

                await _context.SaveChangesAsync();

                await _auditService.UpdateOperationResultAsync(removeOperation.Id, OperationStatus.Success, 
                    null, $"Role '{assignment.DatabaseRole.RoleName}' removed from user '{assignment.DatabaseUser.UserName}' successfully");

                return removeOperation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove role {RoleName} from user {UserName} in database {DatabaseName}", 
                    assignment.DatabaseRole.RoleName, assignment.DatabaseUser.UserName, assignment.DatabaseName);
                
                await _auditService.UpdateOperationResultAsync(removeOperation.Id, OperationStatus.Failed, 
                    ex.Message, ex.ToString());
                
                return removeOperation;
            }
        }

        public async Task<IEnumerable<DatabaseRoleAssignment>> GetUserRoleAssignmentsAsync(Guid userId)
        {
            return await _context.DatabaseRoleAssignments
                .Include(dra => dra.DatabaseRole)
                .Where(dra => dra.DatabaseUserId == userId)
                .OrderByDescending(dra => dra.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<DatabaseRoleAssignment>> GetRoleAssignmentsAsync(Guid roleId)
        {
            return await _context.DatabaseRoleAssignments
                .Include(dra => dra.DatabaseUser)
                .Where(dra => dra.DatabaseRoleId == roleId)
                .OrderByDescending(dra => dra.CreatedAt)
                .ToListAsync();
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