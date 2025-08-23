namespace IAMBuddy.Domain.Common;

public class AdminAuditLog
{
    public int Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public DateTimeOffset ActionDate { get; set; }
    public string ActionBy { get; set; } = string.Empty;
    public string? Description { get; set; }
}
