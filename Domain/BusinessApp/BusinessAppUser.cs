namespace IAMBuddy.Domain.BusinessApp;

using IAMBuddy.Domain.Common;

public class BusinessAppUser : AuditableEntity
{
    public int Id { get; set; }
    public string Role { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int BusinessApplicationId { get; set; }
    public virtual BusinessApplication BusinessApplication { get; set; } = null!;
    public int HumanIdentityId { get; set; }
    public virtual HumanIdentity HumanIdentity { get; set; } = null!;
}
