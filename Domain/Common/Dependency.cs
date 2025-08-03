namespace IAMBuddy.Domain.Common;
using System;
using IAMBuddy.Domain.Enums;

public class Dependency : AuditableEntity
{
    public RelationshipType RelationshipType { get; set; }
    public string? Description { get; set; }
    public int OrderOfOperation { get; set; }
    public DateTimeOffset? LastValidatedTimestamp { get; set; }
    public int SourceIdentityId { get; set; }
    public Identity? SourceIdentity { get; set; }
    public int TargetIdentityId { get; set; }
    public Identity? TargetIdentity { get; set; }
    public int SourceResourceId { get; set; }
    public Resource? SourceResource { get; set; }
    public int TargetResourceId { get; set; }
    public Resource? TargetResource { get; set; }
}
