namespace IAMBuddy.Domain.Common;

using System.ComponentModel.DataAnnotations;

public interface IRequest : IAuditableEntity
{
    [Required] public string RequestedBy { get; set; }
    public DateTimeOffset RequestedAt { get; set; }
    public string? CorrelationId { get; set; }
    public bool DryRun { get; set; }
    public string? Notes { get; set; }
    public ERequestType RequestType { get; set; }
    public ERequestStatus RequestStatus { get; set; }

    public enum ERequestType
    {
        ProvisionIdentity,
        UpdateIdentity,
        DeprovisionIdentity,
        ProvisionAccount,
        UpdateAccount,
        DeprovisionAccount,
        AssignRole,
        UnassignRole,
        GrantAccess,
        RevokeAccess,
        ReviewAccess,
        CreatePolicy,
        ModifyPolicy,
        DeletePolicy,
        RotateCredential
    }

    public enum ERequestStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3,
        Fulfilled = 4,
        Cancelled = 5
    }
}
