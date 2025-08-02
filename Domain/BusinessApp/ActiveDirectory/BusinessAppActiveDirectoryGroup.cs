namespace IAMBuddy.Domain.BusinessApp.ActiveDirectory;

using IAMBuddy.Domain.BusinessApp;
using IAMBuddy.Domain.Enums;

public class BusinessAppActiveDirectoryGroup : AppOwnedBusinessAppResource
{
    public string SamAccountName { get; set; } = string.Empty;
    public ActiveDirectoryGroupType GroupType { get; set; }
    public ActiveDirectoryGroupScope GroupScope { get; set; }
    public string DistinguishedName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public int ActiveDirectoryInstanceId { get; set; }
    public virtual BusinessAppActiveDirectoryInstance ActiveDirectoryInstance { get; set; } = null!;
}
