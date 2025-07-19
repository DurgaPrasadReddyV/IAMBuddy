namespace IAMBuddy.Tools.Data.Entities;

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
    public virtual ICollection<DatabaseUserRole> DatabaseUserRoles { get; set; } = [];
    public virtual ICollection<DatabasePermission> Permissions { get; set; } = [];
}
