namespace IAMBuddy.Tools.Data.Entities;

// Modified Entity: Server Login (updated to support AD accounts and groups)
public class ServerLogin : AuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ServerId { get; set; }
    public AuthenticationType AuthenticationType { get; set; }
    public LoginStatus Status { get; set; }
    public string? DefaultDatabase { get; set; }
    public string? DefaultLanguage { get; set; }
    public DateTime? PasswordExpirationDate { get; set; }
    public bool IsPasswordExpired { get; set; }
    public bool IsLocked { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public string? Description { get; set; }

    // New properties for AD integration
    public int? ActiveDirectoryAccountId { get; set; } // For AD user accounts
    public int? ActiveDirectoryGroupId { get; set; } // For AD groups
    public int? NonHumanIdentityId { get; set; } // For service accounts

    // Navigation properties
    public virtual SqlServer Server { get; set; } = null!;
    public virtual ActiveDirectoryAccount? ActiveDirectoryAccount { get; set; }
    public virtual ActiveDirectoryGroup? ActiveDirectoryGroup { get; set; }
    public virtual NonHumanIdentity? NonHumanIdentity { get; set; }
    public virtual ICollection<DatabaseUser> DatabaseUsers { get; set; } = [];
    public virtual ICollection<ServerLoginRole> ServerLoginRoles { get; set; } = [];
}
