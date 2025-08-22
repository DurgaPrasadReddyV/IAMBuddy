namespace IAMBuddy.Domain.BusinessApp;

using IAMBuddy.Domain.Common;

internal interface IBusinessAppUserOwnedResource : IResource
{
    public int BusinessAppUserId { get; set; }
    public BusinessAppUser BusinessAppUser { get; set; }
}
