namespace IAMBuddy.Domain.BusinessApp;

using IAMBuddy.Domain.Common;

internal interface IBusinessAppOwnedResource : IResource
{
    public int BusinessAppResourceIdentityId { get; set; }
    public BusinessAppResourceIdentity BusinessAppResourceIdentity { get; set; }
}
