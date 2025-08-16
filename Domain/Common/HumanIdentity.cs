namespace IAMBuddy.Domain.Common;

using System.ComponentModel.DataAnnotations;

public class HumanIdentity : IAuditableEntity, IHasAuthoritativeSource
{
    public string UserId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? JobTitle { get; set; }
    public string? Department { get; set; }
    public string? Division { get; set; }
    public string? CostCenter { get; set; }
    public string? Location { get; set; }
    public DateTimeOffset? HireDate { get; set; }
    public DateTimeOffset? TerminationDate { get; set; }
    public EHumanIdentityStatus Status { get; set; }
    public string? EmployeeId { get; set; }
    public string? Company { get; set; }
    public bool IsContractor { get; set; }
    public DateTimeOffset? ContractEndDate { get; set; }
    public string? Description { get; set; }
    public int? ManagerId { get; set; }
    public virtual HumanIdentity? Manager { get; set; }

    // IHasAuthoritativeSource
    public int AuthoritativeSourceId { get; set; }
    public virtual AuthoritativeSource AuthoritativeSource { get; set; } = null!;

    // IAuditableEntity
    public int Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    [Timestamp] public byte[]? RowVersion { get; set; }


    public Dictionary<string, string> Attributes { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public enum EHumanIdentityStatus
    {
        Active = 1,
        Inactive = 2,
        Terminated = 3,
        OnLeave = 4
    }
}
