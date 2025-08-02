namespace IAMBuddy.Domain.BusinessApp;

using IAMBuddy.Domain.Common;

public abstract class AppOwnedBusinessAppResource : Resource
{
    public int BusinessApplicationId { get; set; }
    public virtual BusinessApplication BusinessApplication { get; set; } = null!;
    public int BusinessAppResourceIdentityId { get; set; }
    public virtual BusinessAppResourceIdentity BusinessAppResourceIdentity { get; set; } = null!;
}
