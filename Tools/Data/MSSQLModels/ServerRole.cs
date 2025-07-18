namespace IAMBuddy.Tools.Data.MSSQLModels;

// Entity: Server Role
public class ServerRole : AuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ServerId { get; set; }
    public RoleType Type { get; set; }
    public bool IsFixedRole { get; set; }
    public string? Description { get; set; }

    // Navigation properties
    public virtual SqlServer Server { get; set; } = null!;
    public virtual ICollection<ServerLoginRole> ServerLoginRoles { get; set; } = new List<ServerLoginRole>();
    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}
