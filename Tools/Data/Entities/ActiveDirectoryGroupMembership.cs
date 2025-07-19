namespace IAMBuddy.Tools.Data.Entities;

// Entity: Active Directory Group Membership
public class ActiveDirectoryGroupMembership : AuditableEntity
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public int? AccountId { get; set; } // For account members
    public int? ChildGroupId { get; set; } // For nested group members
    public DateTime? MemberSince { get; set; }
    public string? AddedBy { get; set; }

    // Navigation properties
    public virtual ActiveDirectoryGroup Group { get; set; } = null!;
    public virtual ActiveDirectoryAccount? Account { get; set; }
    public virtual ActiveDirectoryGroup? ChildGroup { get; set; }
}
