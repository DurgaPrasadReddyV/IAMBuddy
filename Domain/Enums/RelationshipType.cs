namespace IAMBuddy.Domain.Enums;

public enum RelationshipType
{
    PrerequisiteForProvisioning,
    ParentOf,
    ChildOf,
    LinkedAccount,
    FunctionalDependency,
    SynchronizationDependency,
    Hierarchical,
    ManagedBy,
    OwnerOf
}
