namespace IAMBuddy.Domain.Common;
using System;

public class AuthoritativeSource : AuditableEntity
{
    public string? SourceName { get; set; }
    public string? SourceType { get; set; }
    public string? Description { get; set; }
    public DateTime? LastSynchronizationTimestamp { get; set; }
}
