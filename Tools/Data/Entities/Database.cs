namespace IAMBuddy.Tools.Data.Entities;

// Modified Entity: Database (existing)
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
    public virtual ICollection<DatabaseUser> DatabaseUsers { get; set; } = [];
    public virtual ICollection<DatabaseRole> DatabaseRoles { get; set; } = [];
    public virtual ICollection<DatabasePermission> Permissions { get; set; } = [];
}
