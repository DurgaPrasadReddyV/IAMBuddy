namespace IAMBuddy.Domain.Common;
public abstract class HumanIdentityOwnedResource : Resource
{
    public int HumanIdentityId { get; set; }
    public virtual HumanIdentity HumanIdentity { get; set; } = null!;
}
