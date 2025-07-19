namespace IAMBuddy.Tools.Data.Entities;

// Entity: Active Directory Group
public class ActiveDirectoryGroup : AuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SamAccountName { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public ActiveDirectoryGroupType GroupType { get; set; }
    public ActiveDirectoryGroupScope GroupScope { get; set; }
    public string Domain { get; set; } = string.Empty;
    public string DistinguishedName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? ManagedBy { get; set; }
    public int? NonHumanIdentityId { get; set; } // Groups are managed by non-human identities
    public bool IsActive { get; set; }

    // Navigation properties
    public virtual NonHumanIdentity? NonHumanIdentity { get; set; }
    public virtual ICollection<ActiveDirectoryGroupMembership> GroupMemberships { get; set; } = [];
    public virtual ICollection<ServerLogin> ServerLogins { get; set; } = [];
}
