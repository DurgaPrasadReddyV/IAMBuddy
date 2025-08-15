namespace IAMBuddy.Domain.BusinessApp;

using IAMBuddy.Domain.Common;

internal interface IBusinessAppOwnedResource : IResource, IHasBusinessApplication
{
    public int BusinessAppResourceIdentityId { get; set; }
    public BusinessAppResourceIdentity BusinessAppResourceIdentity { get; set; }
}
