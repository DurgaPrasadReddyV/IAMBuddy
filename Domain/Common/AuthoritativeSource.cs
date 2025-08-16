namespace IAMBuddy.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;

public class AuthoritativeSource : IAuditableEntity
{
    public string? SourceName { get; set; }
    public string? SourceType { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset? LastSynchronizationTimestamp { get; set; }

    // IAuditableEntity
    public int Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    [Timestamp] public byte[]? RowVersion { get; set; }


    public Dictionary<string, string> Attributes { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}
