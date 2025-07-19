namespace IAMBuddy.Tools.Data.Entities;

// Entity: Non-Human Identity
public class NonHumanIdentity : AuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public NonHumanIdentityType Type { get; set; }
    public NonHumanIdentityStatus Status { get; set; }
    public int BusinessApplicationId { get; set; }
    public int? BusinessApplicationEnvironmentId { get; set; }
    public int PrimaryOwnerId { get; set; }
    public int? AlternateOwnerId { get; set; }
    public string? Purpose { get; set; }
    public string? TechnicalContact { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public DateTime? LastAccessDate { get; set; }
    public string? AccessFrequency { get; set; }
    public bool IsGeneric { get; set; } // For generic non-human identities that don't fit environment-based categories
    public string? Description { get; set; }

    // Navigation properties
    public virtual BusinessApplication BusinessApplication { get; set; } = null!;
    public virtual BusinessApplicationEnvironment? BusinessApplicationEnvironment { get; set; }
    public virtual HumanIdentity PrimaryOwner { get; set; } = null!;
    public virtual HumanIdentity? AlternateOwner { get; set; }
    public virtual ICollection<ActiveDirectoryAccount> ActiveDirectoryAccounts { get; set; } = [];
    public virtual ICollection<ServerLogin> ServerLogins { get; set; } = [];
}
