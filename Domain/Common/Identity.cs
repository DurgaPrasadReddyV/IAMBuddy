namespace IAMBuddy.Domain.Common;

public class Identity : AuditableEntity
{
    public int AuthoritativeSourceId { get; set; }
    public virtual AuthoritativeSource AuthoritativeSource { get; set; } = null!;
    public ICollection<Dependency>? SourceDependencies { get; set; }
    public ICollection<Dependency>? TargetDependencies { get; set; }
    public ICollection<LifecycleEvent>? InitiatedLifecycleEvents { get; set; }
    public ICollection<LifecycleEvent>? AssociatedLifecycleEvents { get; set; }
}
