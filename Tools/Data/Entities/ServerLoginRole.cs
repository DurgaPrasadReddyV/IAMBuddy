namespace IAMBuddy.Tools.Data.Entities;

public class ServerLoginRole : AuditableEntity
{
    public int Id { get; set; }
    public int ServerLoginId { get; set; }
    public int ServerRoleId { get; set; }
    public DateTime AssignedDate { get; set; }
    public string? AssignedBy { get; set; }

    // Navigation properties
    public virtual ServerLogin ServerLogin { get; set; } = null!;
    public virtual ServerRole ServerRole { get; set; } = null!;
}
