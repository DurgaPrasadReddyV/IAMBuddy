namespace IAMBuddy.Tools.Data.MSSQLModels;

// Entity: Database Role
public class DatabaseRole : AuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DatabaseId { get; set; }
    public RoleType Type { get; set; }
    public bool IsFixedRole { get; set; }
    public string? Description { get; set; }

    // Navigation properties
    public virtual Database Database { get; set; } = null!;
    public virtual ICollection<DatabaseUserRole> DatabaseUserRoles { get; set; } = new List<DatabaseUserRole>();
    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}
