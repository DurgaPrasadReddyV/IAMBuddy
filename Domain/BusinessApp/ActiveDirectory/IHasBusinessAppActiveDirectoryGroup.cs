namespace IAMBuddy.Domain.Common;

using IAMBuddy.Domain.BusinessApp.ActiveDirectory;

internal interface IHasBusinessAppActiveDirectoryGroup
{
    public int BusinessAppActiveDirectoryGroupId { get; set; }
    public BusinessAppActiveDirectoryGroup BusinessAppActiveDirectoryGroup { get; set; }
}
