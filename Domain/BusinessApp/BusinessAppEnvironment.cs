namespace IAMBuddy.Domain.BusinessApp;

using IAMBuddy.Domain.Common;

public class BusinessAppEnvironment : AuditableEntity
{
    public int Id { get; set; }
    public Enums.BusinessAppEnvironment Environment { get; set; }
    public string? EnvironmentName { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public string? Url { get; set; }
    public int BusinessApplicationId { get; set; }
    public virtual BusinessApplication BusinessApplication { get; set; } = null!;
}
