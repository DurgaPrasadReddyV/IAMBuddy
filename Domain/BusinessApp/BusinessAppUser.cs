namespace IAMBuddy.Domain.BusinessApp;

using System.ComponentModel.DataAnnotations;
using IAMBuddy.Domain.Common;

public class BusinessAppUser : IAuditableEntity, IHasHumanIdentity, IHasBusinessApplication
{
    public string Role { get; set; } = string.Empty;
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }

    // IHasBusinessApplication
    public int BusinessApplicationId { get; set; }
    public virtual BusinessApplication BusinessApplication { get; set; } = null!;

    // IHasHumanIdentity
    public int HumanIdentityId { get; set; }
    public virtual HumanIdentity HumanIdentity { get; set; } = null!;

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
