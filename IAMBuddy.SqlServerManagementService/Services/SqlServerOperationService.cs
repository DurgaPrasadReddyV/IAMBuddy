using IAMBuddy.SqlServerManagementService.Models;
using Microsoft.EntityFrameworkCore;

namespace IAMBuddy.SqlServerManagementService.Services;

public class SqlServerOperationService : ISqlServerOperationService
{
    private readonly SqlServerManagementDbContext _context;

    public SqlServerOperationService(SqlServerManagementDbContext context)
    {
        _context = context;
    }

    public async Task<SqlServerOperation> CreateOperationAsync(SqlServerOperation operation)
    {
        _context.SqlServerOperations.Add(operation);
        await _context.SaveChangesAsync();
        return operation;
    }

    public async Task<SqlServerOperation?> GetOperationAsync(int id)
    {
        return await _context.SqlServerOperations
            .Include(o => o.Login)
            .Include(o => o.User)
            .Include(o => o.Role)
            .Include(o => o.RoleAssignment)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<SqlServerOperation>> GetOperationsAsync(string? status = null, string? requestId = null)
    {
        var query = _context.SqlServerOperations.AsQueryable();

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(o => o.Status == status);
        }

        if (!string.IsNullOrEmpty(requestId))
        {
            query = query.Where(o => o.RequestId == requestId);
        }

        return await query
            .Include(o => o.Login)
            .Include(o => o.User)
            .Include(o => o.Role)
            .Include(o => o.RoleAssignment)
            .OrderByDescending(o => o.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<SqlServerOperation>> GetOperationsByServerAsync(string serverInstance)
    {
        return await _context.SqlServerOperations
            .Where(o => o.ServerInstance == serverInstance)
            .Include(o => o.Login)
            .Include(o => o.User)
            .Include(o => o.Role)
            .Include(o => o.RoleAssignment)
            .OrderByDescending(o => o.CreatedDate)
            .ToListAsync();
    }

    public async Task<SqlServerOperation> UpdateOperationAsync(SqlServerOperation operation)
    {
        _context.SqlServerOperations.Update(operation);
        await _context.SaveChangesAsync();
        return operation;
    }

    public async Task<SqlServerOperation> UpdateOperationStatusAsync(int id, string status, string? errorMessage = null)
    {
        var operation = await _context.SqlServerOperations.FindAsync(id);
        if (operation == null)
        {
            throw new ArgumentException($"Operation with ID {id} not found.");
        }

        operation.Status = status;
        if (!string.IsNullOrEmpty(errorMessage))
        {
            operation.ErrorMessage = errorMessage;
        }

        if (status == "IN_PROGRESS" && operation.StartTime == default)
        {
            operation.StartTime = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return operation;
    }

    public async Task<SqlServerOperation> CompleteOperationAsync(int id, bool success, string? errorMessage = null)
    {
        var operation = await _context.SqlServerOperations.FindAsync(id);
        if (operation == null)
        {
            throw new ArgumentException($"Operation with ID {id} not found.");
        }

        operation.Status = success ? "SUCCESS" : "FAILED";
        operation.EndTime = DateTime.UtcNow;
        operation.DurationMs = (int)(operation.EndTime.Value - operation.StartTime).TotalMilliseconds;

        if (!success && !string.IsNullOrEmpty(errorMessage))
        {
            operation.ErrorMessage = errorMessage;
        }

        await _context.SaveChangesAsync();
        return operation;
    }

    public async Task<IEnumerable<SqlServerOperation>> GetPendingOperationsAsync()
    {
        return await _context.SqlServerOperations
            .Where(o => o.Status == "PENDING")
            .Include(o => o.Login)
            .Include(o => o.User)
            .Include(o => o.Role)
            .Include(o => o.RoleAssignment)
            .OrderBy(o => o.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<SqlServerOperation>> GetFailedOperationsAsync()
    {
        return await _context.SqlServerOperations
            .Where(o => o.Status == "FAILED")
            .Include(o => o.Login)
            .Include(o => o.User)
            .Include(o => o.Role)
            .Include(o => o.RoleAssignment)
            .OrderByDescending(o => o.CreatedDate)
            .ToListAsync();
    }
}