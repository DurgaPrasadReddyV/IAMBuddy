namespace IAMBuddy.Domain.Common;

using IAMBuddy.Domain.Enums;

public abstract class ApprovalRequest : AuditableEntity
{
    public string? RequestType { get; set; }
    public string? RequestPayload { get; set; }
    public ApprovalStatus Status { get; set; }
    public string? RequestNotes { get; set; }
    public DateTime? ApprovalTimestamp { get; set; }
    public string? ApprovalNotes { get; set; }
    public int ApprovedById { get; set; }
    public virtual HumanIdentity ApprovedBy { get; set; } = null!;
    public int RequestedById { get; set; }
    public virtual HumanIdentity RequestedBy { get; set; } = null!;
    public int LifecycleEventId { get; set; }
    public virtual LifecycleEvent LifecycleEvent { get; set; } = null!;
}
