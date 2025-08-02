namespace IAMBuddy.Domain.BusinessApp.ActiveDirectory;

using IAMBuddy.Domain.BusinessApp;
using IAMBuddy.Domain.Enums;

public class BusinessAppServiceActiveDirectoryAccount : AppOwnedBusinessAppResource
{
    public string SamAccountName { get; set; } = string.Empty;
    public string? UserPrincipalName { get; set; }
    public ActiveDirectoryAccountType AccountType { get; set; }
    public string DistinguishedName { get; set; } = string.Empty;
    public DateTime? LastLogonDate { get; set; }
    public DateTime? PasswordLastSetDate { get; set; }
    public DateTime? AccountExpirationDate { get; set; }
    public bool PasswordNeverExpires { get; set; }
    public bool UserCannotChangePassword { get; set; }
    public string? ServicePrincipalNames { get; set; }
    public int ActiveDirectoryInstanceId { get; set; }
    public virtual BusinessAppActiveDirectoryInstance ActiveDirectoryInstance { get; set; } = null!;
    public virtual ICollection<BusinessAppServiceActiveDirectoryGroupMembership> GroupMemberships { get; set; } = [];
}
