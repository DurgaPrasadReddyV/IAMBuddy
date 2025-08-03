namespace IAMBuddy.Domain.Common;
using System;
using IAMBuddy.Domain.Enums;

public class LifecycleEvent : AuditableEntity
{
    public EventType EventType { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public string? PreviousState { get; set; }
    public string? NewState { get; set; }
    public string? Justification { get; set; }
    public string? InitiatorType { get; set; }
    public string? Status { get; set; }
    public string? Description { get; set; }
    public int InitiatorIdentityId { get; set; }
    public Identity? InitiatorIdentity { get; set; }
    public int AssociatedIdentityId { get; set; }
    public Identity? AssociatedIdentity { get; set; }
    public int AssociatedResourceId { get; set; }
    public Resource? AssociatedResource { get; set; }
}
