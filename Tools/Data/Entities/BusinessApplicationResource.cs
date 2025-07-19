namespace IAMBuddy.Tools.Data.Entities;

// Entity: Business Application Resource (to link servers, databases to applications)
public class BusinessApplicationResource : AuditableEntity
{
    public int Id { get; set; }
    public int BusinessApplicationId { get; set; }
    public string ResourceType { get; set; } = string.Empty; // Server, Database, Instance, etc.
    public int ResourceId { get; set; }
    public string? ResourceName { get; set; }
    public ApplicationEnvironment? Environment { get; set; }
    public string? Purpose { get; set; }
    public bool IsCritical { get; set; }

    // Navigation properties
    public virtual BusinessApplication BusinessApplication { get; set; } = null!;
}
