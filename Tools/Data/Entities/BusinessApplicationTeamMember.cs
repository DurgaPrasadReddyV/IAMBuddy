namespace IAMBuddy.Tools.Data.Entities;

// Entity: Business Application Team Member
public class BusinessApplicationTeamMember : AuditableEntity
{
    public int Id { get; set; }
    public int BusinessApplicationId { get; set; }
    public int HumanIdentityId { get; set; }
    public string Role { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    // Navigation properties
    public virtual BusinessApplication BusinessApplication { get; set; } = null!;
    public virtual HumanIdentity HumanIdentity { get; set; } = null!;
}
