namespace IAMBuddy.Tools.Data.Entities;

// Entity: Active Directory Account
public class ActiveDirectoryAccount : AuditableEntity
{
    public int Id { get; set; }
    public string SamAccountName { get; set; } = string.Empty;
    public string? UserPrincipalName { get; set; }
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public ActiveDirectoryAccountType AccountType { get; set; }
    public string Domain { get; set; } = string.Empty;
    public string DistinguishedName { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public DateTime? LastLogonDate { get; set; }
    public DateTime? PasswordLastSetDate { get; set; }
    public DateTime? AccountExpirationDate { get; set; }
    public bool PasswordNeverExpires { get; set; }
    public bool UserCannotChangePassword { get; set; }
    public int? HumanIdentityId { get; set; } // For human accounts
    public int? NonHumanIdentityId { get; set; } // For service accounts
    public string? ServicePrincipalNames { get; set; } // JSON array of SPNs
    public string? ManagedBy { get; set; }

    // Navigation properties
    public virtual HumanIdentity? HumanIdentity { get; set; }
    public virtual NonHumanIdentity? NonHumanIdentity { get; set; }
    public virtual ICollection<ActiveDirectoryGroupMembership> GroupMemberships { get; set; } = [];
    public virtual ICollection<ServerLogin> ServerLogins { get; set; } = [];
}
