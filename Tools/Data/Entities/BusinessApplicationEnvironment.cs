namespace IAMBuddy.Tools.Data.Entities;

// Entity: Business Application Environment
public class BusinessApplicationEnvironment : AuditableEntity
{
    public int Id { get; set; }
    public int BusinessApplicationId { get; set; }
    public ApplicationEnvironment Environment { get; set; }
    public string? EnvironmentName { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public string? Url { get; set; }

    // Navigation properties
    public virtual BusinessApplication BusinessApplication { get; set; } = null!;
    public virtual ICollection<NonHumanIdentity> NonHumanIdentities { get; set; } = [];
}
