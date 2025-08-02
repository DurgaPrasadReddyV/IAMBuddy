namespace IAMBuddy.Domain.Common;

using IAMBuddy.Domain.Enums;

public abstract class ProvisionRequest<T> : HumanIdentityOwnedResource
    where T : Resource
{
    public string RequestType { get; set; } = string.Empty;
    public RequestStatus Status { get; set; }
    public DateTime RequestedDate { get; set; }
    public string? Justification { get; set; }
    public int RequestedById { get; set; }
    public virtual HumanIdentity RequestedBy { get; set; } = null!;
    public T Resource { get; set; } = default!;
}
