namespace IAMBuddy.Domain.Common;

using IAMBuddy.Domain.BusinessApp.ActiveDirectory;

internal interface IHasBusinessAppHumanActiveDirectoryAccount
{
    public int BusinessAppHumanActiveDirectoryAccountId { get; set; }
    public BusinessAppHumanActiveDirectoryAccount BusinessAppHumanActiveDirectoryAccount { get; set; }
}
