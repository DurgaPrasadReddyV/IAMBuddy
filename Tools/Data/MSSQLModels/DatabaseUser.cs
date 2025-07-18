namespace IAMBuddy.Tools.Data.MSSQLModels;

// Entity: Database User
public class DatabaseUser : AuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DatabaseId { get; set; }
    public int? ServerLoginId { get; set; }
    public string? DefaultSchema { get; set; }
    public string UserType { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? Description { get; set; }

    // Navigation properties
    public virtual Database Database { get; set; } = null!;
    public virtual ServerLogin? ServerLogin { get; set; }
    public virtual ICollection<DatabaseUserRole> DatabaseUserRoles { get; set; } = new List<DatabaseUserRole>();
    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}
