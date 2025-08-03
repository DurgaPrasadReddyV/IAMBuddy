namespace IAMBuddy.Domain.Common;

using IAMBuddy.Domain.Enums;

public abstract class Resource : AuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public ResourceType Type { get; set; }
    public int AuthoritativeSourceId { get; set; }
    public virtual AuthoritativeSource AuthoritativeSource { get; set; } = null!;
    public ICollection<LifecycleEvent>? AssociatedLifecycleEvents { get; set; }
    public ICollection<Dependency>? SourceDependencies { get; set; }
    public ICollection<Dependency>? TargetDependencies { get; set; }
}
