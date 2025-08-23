namespace IAMBuddy.Domain.BusinessApp;

using IAMBuddy.Domain.Common;

internal interface IBusinessAppUserOwnedResource : IResource
{
    public int BusinessApplicationId { get; set; }
    public int HumanIdentityId { get; set; }
    public BusinessAppUser BusinessAppUser { get; set; }
}
