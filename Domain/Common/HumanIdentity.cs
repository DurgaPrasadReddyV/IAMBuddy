namespace IAMBuddy.Domain.Common;

using IAMBuddy.Domain.Enums;

public class HumanIdentity : AuditableEntity
{
    public int Id { get; set; }
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
    public DateTime? HireDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public HumanIdentityStatus Status { get; set; }
    public string? EmployeeId { get; set; }
    public string? Company { get; set; }
    public bool IsContractor { get; set; }
    public DateTime? ContractEndDate { get; set; }
    public string? Description { get; set; }
    public int? ManagerId { get; set; }
    public virtual HumanIdentity? Manager { get; set; }
    public virtual ICollection<HumanIdentity> DirectReports { get; set; } = [];
}
