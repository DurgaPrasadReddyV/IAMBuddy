namespace IAMBuddy.Domain.BusinessApp.MSSQL.Database.Roles;

using System.ComponentModel.DataAnnotations;
using IAMBuddy.Domain.Common;

public class AddBusinessAppMSSQLDatabaseADServiceUserRole : IRequest
{
    public int BusinessAppMSSQLDatabaseADServiceUserRoleId { get; set; }
    public virtual BusinessAppMSSQLDatabaseADServiceUserRole BusinessAppMSSQLDatabaseADServiceUserRole { get; set; } = null!;


    // IRequest
    public int Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    [Timestamp] public byte[]? RowVersion { get; set; }
    public Dictionary<string, string> Attributes { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public string RequestedBy { get; set; } = string.Empty;
    public DateTimeOffset RequestedAt { get; set; }
    public string? CorrelationId { get; set; }
    public bool DryRun { get; set; }
    public string? Notes { get; set; }
    public IRequest.ERequestType RequestType { get; set; }
    public IRequest.ERequestStatus RequestStatus { get; set; }
}

