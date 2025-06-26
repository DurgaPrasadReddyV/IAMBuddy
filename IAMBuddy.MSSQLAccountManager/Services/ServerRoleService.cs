using IAMBuddy.MSSQLAccountManager.Data;
using IAMBuddy.MSSQLAccountManager.Models;
using IAMBuddy.MSSQLAccountManager.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Dapper;

namespace IAMBuddy.MSSQLAccountManager.Services
{
    public class ServerRoleService : IServerRoleService
    {
        private readonly MSSQLAccountManagerContext _context;
        private readonly IAuditService _auditService;
        private readonly ILogger<ServerRoleService> _logger;

        public ServerRoleService(
            MSSQLAccountManagerContext context,
            IAuditService auditService,
            ILogger<ServerRoleService> logger)
        {
            _context = context;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<OperationResult> CreateRoleAsync(ServerRole role)
        {
            var operation = await _auditService.LogOperationAsync(
                OperationType.Create, "ServerRole", role.RoleName, role.ServerName, 
                null, null, role.CreatedBy);

            try
            {
                var existingRole = await GetRoleByNameAsync(role.ServerName, role.InstanceName, role.RoleName);
                if (existingRole != null)
                {
                    await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Failed, 
                        $"Server role '{role.RoleName}' already exists on server '{role.ServerName}'");
                    return operation;
                }

                var connectionString = await GetConnectionStringAsync(role.ServerName, role.InstanceName);
                using var connection = new SqlConnection(connectionString);
                
                await connection.ExecuteAsync($"CREATE SERVER ROLE [{role.RoleName}]");

                _context.ServerRoles.Add(role);
                await _context.SaveChangesAsync();

                await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Success, 
                    null, $"Server role '{role.RoleName}' created successfully");

                return operation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create server role {RoleName} on {ServerName}", 
                    role.RoleName, role.ServerName);
                
                await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Failed, 
                    ex.Message, ex.ToString());
                
                return operation;
            }
        }

        public async Task<OperationResult> UpdateRoleAsync(Guid roleId, ServerRole updatedRole)
        {
            var existingRole = await _context.ServerRoles.FindAsync(roleId);
            if (existingRole == null)
            {
                var operation = await _auditService.LogOperationAsync(
                    OperationType.Update, "ServerRole", "Unknown", "Unknown");
                await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Failed, 
                    $"Server role with ID '{roleId}' not found");
                return operation;
            }

            var updateOperation = await _auditService.LogOperationAsync(
                OperationType.Update, "ServerRole", existingRole.RoleName, existingRole.ServerName, 
                null, null, updatedRole.UpdatedBy);

            try
            {
                existingRole.Description = updatedRole.Description;
                existingRole.UpdatedBy = updatedRole.UpdatedBy;
                existingRole.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                await _auditService.UpdateOperationResultAsync(updateOperation.Id, OperationStatus.Success, 
                    null, $"Server role '{existingRole.RoleName}' updated successfully");

                return updateOperation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update server role {RoleId}", roleId);
                
                await _auditService.UpdateOperationResultAsync(updateOperation.Id, OperationStatus.Failed, 
                    ex.Message, ex.ToString());
                
                return updateOperation;
            }
        }

        public async Task<OperationResult> DeleteRoleAsync(Guid roleId)
        {
            var role = await _context.ServerRoles
                .Include(r => r.ServerRoleAssignments)
                .FirstOrDefaultAsync(r => r.Id == roleId);

            if (role == null)
            {
                var operation = await _auditService.LogOperationAsync(
                    OperationType.Delete, "ServerRole", "Unknown", "Unknown");
                await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Failed, 
                    $"Server role with ID '{roleId}' not found");
                return operation;
            }

            var deleteOperation = await _auditService.LogOperationAsync(
                OperationType.Delete, "ServerRole", role.RoleName, role.ServerName);

            try
            {
                if (role.IsFixedRole)
                {
                    await _auditService.UpdateOperationResultAsync(deleteOperation.Id, OperationStatus.Failed, 
                        $"Cannot delete fixed server role '{role.RoleName}'");
                    return deleteOperation;
                }

                var connectionString = await GetConnectionStringAsync(role.ServerName, role.InstanceName);
                using var connection = new SqlConnection(connectionString);
                
                await connection.ExecuteAsync($"DROP SERVER ROLE [{role.RoleName}]");

                _context.ServerRoles.Remove(role);
                await _context.SaveChangesAsync();

                await _auditService.UpdateOperationResultAsync(deleteOperation.Id, OperationStatus.Success, 
                    null, $"Server role '{role.RoleName}' deleted successfully");

                return deleteOperation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete server role {RoleName}", role.RoleName);
                
                await _auditService.UpdateOperationResultAsync(deleteOperation.Id, OperationStatus.Failed, 
                    ex.Message, ex.ToString());
                
                return deleteOperation;
            }
        }

        public async Task<ServerRole?> GetRoleByIdAsync(Guid roleId)
        {
            return await _context.ServerRoles
                .Include(r => r.ServerRoleAssignments)
                    .ThenInclude(sra => sra.SqlLogin)
                .FirstOrDefaultAsync(r => r.Id == roleId);
        }

        public async Task<ServerRole?> GetRoleByNameAsync(string serverName, string? instanceName, string roleName)
        {
            return await _context.ServerRoles
                .Include(r => r.ServerRoleAssignments)
                    .ThenInclude(sra => sra.SqlLogin)
                .FirstOrDefaultAsync(r => r.ServerName == serverName && 
                                         r.InstanceName == instanceName && 
                                         r.RoleName == roleName);
        }

        public async Task<IEnumerable<ServerRole>> GetAllRolesAsync(string serverName, string? instanceName = null)
        {
            var query = _context.ServerRoles
                .Include(r => r.ServerRoleAssignments)
                    .ThenInclude(sra => sra.SqlLogin)
                .Where(r => r.ServerName == serverName);

            if (instanceName != null)
            {
                query = query.Where(r => r.InstanceName == instanceName);
            }

            return await query.ToListAsync();
        }

        public async Task<OperationResult> AssignRoleToLoginAsync(Guid loginId, Guid roleId)
        {
            var login = await _context.SqlLogins.FindAsync(loginId);
            var role = await _context.ServerRoles.FindAsync(roleId);

            if (login == null || role == null)
            {
                var operation = await _auditService.LogOperationAsync(
                    OperationType.Assign, "ServerRoleAssignment", "Unknown", "Unknown");
                await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Failed, 
                    "Login or role not found");
                return operation;
            }

            var assignOperation = await _auditService.LogOperationAsync(
                OperationType.Assign, "ServerRoleAssignment", $"{login.LoginName} -> {role.RoleName}", 
                role.ServerName, null, null, login.UpdatedBy);

            try
            {
                var existingAssignment = await _context.ServerRoleAssignments
                    .FirstOrDefaultAsync(sra => sra.SqlLoginId == loginId && sra.ServerRoleId == roleId);

                if (existingAssignment != null)
                {
                    if (existingAssignment.IsActive)
                    {
                        await _auditService.UpdateOperationResultAsync(assignOperation.Id, OperationStatus.Failed, 
                            $"Role '{role.RoleName}' is already assigned to login '{login.LoginName}'");
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
                    var assignment = new ServerRoleAssignment
                    {
                        SqlLoginId = loginId,
                        ServerRoleId = roleId,
                        ServerName = role.ServerName,
                        InstanceName = role.InstanceName,
                        CreatedBy = login.UpdatedBy ?? "System"
                    };
                    _context.ServerRoleAssignments.Add(assignment);
                }

                var connectionString = await GetConnectionStringAsync(role.ServerName, role.InstanceName);
                using var connection = new SqlConnection(connectionString);
                
                await connection.ExecuteAsync($"ALTER SERVER ROLE [{role.RoleName}] ADD MEMBER [{login.LoginName}]");

                await _context.SaveChangesAsync();

                await _auditService.UpdateOperationResultAsync(assignOperation.Id, OperationStatus.Success, 
                    null, $"Role '{role.RoleName}' assigned to login '{login.LoginName}' successfully");

                return assignOperation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to assign role {RoleName} to login {LoginName}", 
                    role.RoleName, login.LoginName);
                
                await _auditService.UpdateOperationResultAsync(assignOperation.Id, OperationStatus.Failed, 
                    ex.Message, ex.ToString());
                
                return assignOperation;
            }
        }

        public async Task<OperationResult> RemoveRoleFromLoginAsync(Guid loginId, Guid roleId)
        {
            var assignment = await _context.ServerRoleAssignments
                .Include(sra => sra.SqlLogin)
                .Include(sra => sra.ServerRole)
                .FirstOrDefaultAsync(sra => sra.SqlLoginId == loginId && sra.ServerRoleId == roleId && sra.IsActive);

            if (assignment == null)
            {
                var operation = await _auditService.LogOperationAsync(
                    OperationType.Remove, "ServerRoleAssignment", "Unknown", "Unknown");
                await _auditService.UpdateOperationResultAsync(operation.Id, OperationStatus.Failed, 
                    "Active role assignment not found");
                return operation;
            }

            var removeOperation = await _auditService.LogOperationAsync(
                OperationType.Remove, "ServerRoleAssignment", 
                $"{assignment.SqlLogin.LoginName} -> {assignment.ServerRole.RoleName}", 
                assignment.ServerName, null, null, assignment.SqlLogin.UpdatedBy);

            try
            {
                var connectionString = await GetConnectionStringAsync(assignment.ServerName, assignment.InstanceName);
                using var connection = new SqlConnection(connectionString);
                
                await connection.ExecuteAsync($"ALTER SERVER ROLE [{assignment.ServerRole.RoleName}] DROP MEMBER [{assignment.SqlLogin.LoginName}]");

                assignment.IsActive = false;
                assignment.RevokedAt = DateTime.UtcNow;
                assignment.RevokedBy = assignment.SqlLogin.UpdatedBy ?? "System";

                await _context.SaveChangesAsync();

                await _auditService.UpdateOperationResultAsync(removeOperation.Id, OperationStatus.Success, 
                    null, $"Role '{assignment.ServerRole.RoleName}' removed from login '{assignment.SqlLogin.LoginName}' successfully");

                return removeOperation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove role {RoleName} from login {LoginName}", 
                    assignment.ServerRole.RoleName, assignment.SqlLogin.LoginName);
                
                await _auditService.UpdateOperationResultAsync(removeOperation.Id, OperationStatus.Failed, 
                    ex.Message, ex.ToString());
                
                return removeOperation;
            }
        }

        public async Task<IEnumerable<ServerRoleAssignment>> GetLoginRoleAssignmentsAsync(Guid loginId)
        {
            return await _context.ServerRoleAssignments
                .Include(sra => sra.ServerRole)
                .Where(sra => sra.SqlLoginId == loginId)
                .OrderByDescending(sra => sra.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ServerRoleAssignment>> GetRoleAssignmentsAsync(Guid roleId)
        {
            return await _context.ServerRoleAssignments
                .Include(sra => sra.SqlLogin)
                .Where(sra => sra.ServerRoleId == roleId)
                .OrderByDescending(sra => sra.CreatedAt)
                .ToListAsync();
        }

        private async Task<string> GetConnectionStringAsync(string serverName, string? instanceName)
        {
            var serverInstance = await _context.ServerInstances
                .FirstOrDefaultAsync(si => si.ServerName == serverName && si.InstanceName == instanceName);
            
            if (serverInstance != null)
                return serverInstance.ConnectionString;

            var server = instanceName != null ? $"{serverName}\\{instanceName}" : serverName;
            return $"Server={server};Integrated Security=true;TrustServerCertificate=true;";
        }
    }
}