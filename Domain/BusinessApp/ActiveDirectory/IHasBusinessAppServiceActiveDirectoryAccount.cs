namespace IAMBuddy.Domain.Common;

using IAMBuddy.Domain.BusinessApp.ActiveDirectory;

internal interface IHasBusinessAppServiceActiveDirectoryAccount
{
    public int BusinessAppServiceActiveDirectoryAccountId { get; set; }
    public BusinessAppServiceActiveDirectoryAccount BusinessAppServiceActiveDirectoryAccount { get; set; }
}
