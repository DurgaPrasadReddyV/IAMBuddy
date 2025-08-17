namespace IAMBuddy.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class ApprovalRequest : IAuditableEntity
{
    public string? ApproversEmail { get; set; }
    public string? ApproversDistributionList { get; set; }
    public string? RequestPayload { get; set; }
    public EApprovalStatus Status { get; set; }
    public string? RequestNotes { get; set; }
    public DateTime? ApprovalTimestamp { get; set; }
    public string? ApprovalNotes { get; set; }
    public int ApprovedById { get; set; }
    public virtual HumanIdentity ApprovedBy { get; set; } = null!;

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

    public enum EApprovalStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3
    }
}
