namespace IAMBuddy.Tools.Data.MSSQLModels;

// Entity: Database
public class Database : AuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int InstanceId { get; set; }
    public string Collation { get; set; } = string.Empty;
    public string RecoveryModel { get; set; } = string.Empty;
    public string CompatibilityLevel { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? Description { get; set; }

    // Navigation properties
    public virtual SqlServerInstance Instance { get; set; } = null!;
    public virtual ICollection<DatabaseUser> DatabaseUsers { get; set; } = new List<DatabaseUser>();
    public virtual ICollection<DatabaseRole> DatabaseRoles { get; set; } = new List<DatabaseRole>();
    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}
