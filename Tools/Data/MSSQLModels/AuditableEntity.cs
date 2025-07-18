namespace IAMBuddy.Tools.Data.MSSQLModels;

// Base entity for auditing
public abstract class AuditableEntity
{
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }
}
