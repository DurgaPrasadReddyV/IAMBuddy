namespace IAMBuddy.Tools.Data.MSSQLModels;

// Entity: Server Login
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

    // Navigation properties
    public virtual SqlServer Server { get; set; } = null!;
    public virtual ICollection<DatabaseUser> DatabaseUsers { get; set; } = new List<DatabaseUser>();
    public virtual ICollection<ServerLoginRole> ServerLoginRoles { get; set; } = new List<ServerLoginRole>();
}
