namespace IAMBuddy.Domain.Common;

using IAMBuddy.Domain.Enums;

public abstract class Approval<T> : HumanIdentityOwnedResource
    where T : Resource
{
    public ApprovalStatus Status { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string? Comments { get; set; }
    public int RequestId { get; set; }
    public virtual ProvisionRequest<T> Request { get; set; } = null!;
    public int ApproverId { get; set; }
    public virtual HumanIdentity Approver { get; set; } = null!;
}
