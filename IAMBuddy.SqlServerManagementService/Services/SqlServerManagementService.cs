using IAMBuddy.SqlServerManagementService.Models;
using Microsoft.EntityFrameworkCore;

namespace IAMBuddy.SqlServerManagementService.Services;

public class SqlServerManagementService : ISqlServerManagementService
{
    private readonly SqlServerManagementDbContext _context;

    public SqlServerManagementService(SqlServerManagementDbContext context)
    {
        _context = context;
    }

    // Login management
    public async Task<SqlServerLogin> CreateLoginAsync(SqlServerLogin login)
    {
        _context.SqlServerLogins.Add(login);
        await _context.SaveChangesAsync();
        return login;
    }

    public async Task<SqlServerLogin?> GetLoginAsync(int id)
    {
        return await _context.SqlServerLogins
            .Include(l => l.Users)
            .Include(l => l.RoleAssignments)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<SqlServerLogin?> GetLoginByNameAsync(string loginName, string serverInstance)
    {
        return await _context.SqlServerLogins
            .Include(l => l.Users)
            .Include(l => l.RoleAssignments)
            .FirstOrDefaultAsync(l => l.LoginName == loginName && l.ServerInstance == serverInstance);
    }

    public async Task<IEnumerable<SqlServerLogin>> GetLoginsAsync(string serverInstance)
    {
        return await _context.SqlServerLogins
            .Where(l => l.ServerInstance == serverInstance)
            .Include(l => l.Users)
            .Include(l => l.RoleAssignments)
            .ToListAsync();
    }

    public async Task<SqlServerLogin> UpdateLoginAsync(SqlServerLogin login)
    {
        _context.SqlServerLogins.Update(login);
        await _context.SaveChangesAsync();
        return login;
    }

    public async Task<bool> DeleteLoginAsync(int id)
    {
        var login = await _context.SqlServerLogins.FindAsync(id);
        if (login == null) return false;

        _context.SqlServerLogins.Remove(login);
        await _context.SaveChangesAsync();
        return true;
    }

    // User management
    public async Task<SqlServerUser> CreateUserAsync(SqlServerUser user)
    {
        _context.SqlServerUsers.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<SqlServerUser?> GetUserAsync(int id)
    {
        return await _context.SqlServerUsers
            .Include(u => u.Login)
            .Include(u => u.RoleAssignments)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<SqlServerUser?> GetUserByNameAsync(string userName, string databaseName, string serverInstance)
    {
        return await _context.SqlServerUsers
            .Include(u => u.Login)
            .Include(u => u.RoleAssignments)
            .FirstOrDefaultAsync(u => u.UserName == userName && 
                                   u.DatabaseName == databaseName && 
                                   u.ServerInstance == serverInstance);
    }

    public async Task<IEnumerable<SqlServerUser>> GetUsersAsync(string serverInstance, string? databaseName = null)
    {
        var query = _context.SqlServerUsers
            .Where(u => u.ServerInstance == serverInstance);

        if (!string.IsNullOrEmpty(databaseName))
        {
            query = query.Where(u => u.DatabaseName == databaseName);
        }

        return await query
            .Include(u => u.Login)
            .Include(u => u.RoleAssignments)
            .ToListAsync();
    }

    public async Task<SqlServerUser> UpdateUserAsync(SqlServerUser user)
    {
        _context.SqlServerUsers.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _context.SqlServerUsers.FindAsync(id);
        if (user == null) return false;

        _context.SqlServerUsers.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    // Role management
    public async Task<SqlServerRole> CreateRoleAsync(SqlServerRole role)
    {
        _context.SqlServerRoles.Add(role);
        await _context.SaveChangesAsync();
        return role;
    }

    public async Task<SqlServerRole?> GetRoleAsync(int id)
    {
        return await _context.SqlServerRoles
            .Include(r => r.RoleAssignments)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<SqlServerRole?> GetRoleByNameAsync(string roleName, string serverInstance, string? databaseName = null)
    {
        return await _context.SqlServerRoles
            .Include(r => r.RoleAssignments)
            .FirstOrDefaultAsync(r => r.RoleName == roleName && 
                               r.ServerInstance == serverInstance &&
                               r.DatabaseName == databaseName);
    }

    public async Task<IEnumerable<SqlServerRole>> GetRolesAsync(string serverInstance, string? databaseName = null)
    {
        var query = _context.SqlServerRoles
            .Where(r => r.ServerInstance == serverInstance);

        if (!string.IsNullOrEmpty(databaseName))
        {
            query = query.Where(r => r.DatabaseName == databaseName);
        }

        return await query
            .Include(r => r.RoleAssignments)
            .ToListAsync();
    }

    public async Task<SqlServerRole> UpdateRoleAsync(SqlServerRole role)
    {
        _context.SqlServerRoles.Update(role);
        await _context.SaveChangesAsync();
        return role;
    }

    public async Task<bool> DeleteRoleAsync(int id)
    {
        var role = await _context.SqlServerRoles.FindAsync(id);
        if (role == null) return false;

        _context.SqlServerRoles.Remove(role);
        await _context.SaveChangesAsync();
        return true;
    }

    // Role assignment management
    public async Task<SqlServerRoleAssignment> CreateRoleAssignmentAsync(SqlServerRoleAssignment assignment)
    {
        _context.SqlServerRoleAssignments.Add(assignment);
        await _context.SaveChangesAsync();
        return assignment;
    }

    public async Task<SqlServerRoleAssignment?> GetRoleAssignmentAsync(int id)
    {
        return await _context.SqlServerRoleAssignments
            .Include(ra => ra.Role)
            .Include(ra => ra.Login)
            .Include(ra => ra.User)
            .FirstOrDefaultAsync(ra => ra.Id == id);
    }

    public async Task<IEnumerable<SqlServerRoleAssignment>> GetRoleAssignmentsAsync(string serverInstance, string? databaseName = null)
    {
        var query = _context.SqlServerRoleAssignments
            .Where(ra => ra.ServerInstance == serverInstance);

        if (!string.IsNullOrEmpty(databaseName))
        {
            query = query.Where(ra => ra.DatabaseName == databaseName);
        }

        return await query
            .Include(ra => ra.Role)
            .Include(ra => ra.Login)
            .Include(ra => ra.User)
            .ToListAsync();
    }

    public async Task<IEnumerable<SqlServerRoleAssignment>> GetRoleAssignmentsByLoginAsync(int loginId)
    {
        return await _context.SqlServerRoleAssignments
            .Where(ra => ra.LoginId == loginId)
            .Include(ra => ra.Role)
            .Include(ra => ra.Login)
            .ToListAsync();
    }

    public async Task<IEnumerable<SqlServerRoleAssignment>> GetRoleAssignmentsByUserAsync(int userId)
    {
        return await _context.SqlServerRoleAssignments
            .Where(ra => ra.UserId == userId)
            .Include(ra => ra.Role)
            .Include(ra => ra.User)
            .ToListAsync();
    }

    public async Task<SqlServerRoleAssignment> UpdateRoleAssignmentAsync(SqlServerRoleAssignment assignment)
    {
        _context.SqlServerRoleAssignments.Update(assignment);
        await _context.SaveChangesAsync();
        return assignment;
    }

    public async Task<bool> DeleteRoleAssignmentAsync(int id)
    {
        var assignment = await _context.SqlServerRoleAssignments.FindAsync(id);
        if (assignment == null) return false;

        _context.SqlServerRoleAssignments.Remove(assignment);
        await _context.SaveChangesAsync();
        return true;
    }
}