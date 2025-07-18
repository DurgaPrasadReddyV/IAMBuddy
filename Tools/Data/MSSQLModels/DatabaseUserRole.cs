namespace IAMBuddy.Tools.Data.MSSQLModels;

// Junction Entity: Database User - Database Role
public class DatabaseUserRole : AuditableEntity
{
    public int Id { get; set; }
    public int DatabaseUserId { get; set; }
    public int DatabaseRoleId { get; set; }
    public DateTime AssignedDate { get; set; }
    public string? AssignedBy { get; set; }

    // Navigation properties
    public virtual DatabaseUser DatabaseUser { get; set; } = null!;
    public virtual DatabaseRole DatabaseRole { get; set; } = null!;
}
