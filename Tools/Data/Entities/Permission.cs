namespace IAMBuddy.Tools.Data.Entities;

public class Permission : AuditableEntity
{
    public int Id { get; set; }
    public string PermissionName { get; set; } = string.Empty;
    public PermissionType Type { get; set; }
    public SecurableType SecurableType { get; set; }
    public string SecurableName { get; set; } = string.Empty;
    public int? DatabaseId { get; set; }
    public int? DatabaseUserId { get; set; }
    public int? DatabaseRoleId { get; set; }
    public int? ServerRoleId { get; set; }
    public string? GrantedBy { get; set; }
    public DateTime? GrantedDate { get; set; }
    public string? Description { get; set; }

    // Navigation properties
    public virtual Database? Database { get; set; }
    public virtual DatabaseUser? DatabaseUser { get; set; }
    public virtual DatabaseRole? DatabaseRole { get; set; }
    public virtual ServerRole? ServerRole { get; set; }
}
