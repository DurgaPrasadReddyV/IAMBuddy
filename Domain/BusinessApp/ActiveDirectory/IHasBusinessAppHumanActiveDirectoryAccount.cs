namespace IAMBuddy.Domain.Common;

using IAMBuddy.Domain.BusinessApp.ActiveDirectory.Accounts;

internal interface IHasBusinessAppHumanActiveDirectoryAccount
{
    public int BusinessAppHumanActiveDirectoryAccountId { get; set; }
    public BusinessAppHumanActiveDirectoryAccount BusinessAppHumanActiveDirectoryAccount { get; set; }
}
