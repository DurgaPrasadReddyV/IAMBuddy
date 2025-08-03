namespace IAMBuddy.Domain.BusinessApp;

using IAMBuddy.Domain.Common;

public abstract class BusinessAppUserOwnedResource : Resource
{
    public int BusinessApplicationId { get; set; }
    public virtual BusinessApplication BusinessApplication { get; set; } = null!;
    public int BusinessAppUserId { get; set; }
    public virtual BusinessAppUserIdentity BusinessAppUser { get; set; } = null!;
}
