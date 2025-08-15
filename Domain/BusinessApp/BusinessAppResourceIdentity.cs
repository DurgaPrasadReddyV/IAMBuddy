namespace IAMBuddy.Domain.BusinessApp;

using System.ComponentModel.DataAnnotations;
using IAMBuddy.Domain.Common;

public class BusinessAppResourceIdentity : IAuditableEntity, IHasBusinessApplication,
    IHasBusinessAppEnvironment, IHasPrimaryOwner, IHasSecondaryOwner
{
    public string Name { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public EBusinessAppResourceIdentityType Type { get; set; }
    public EBusinessAppResourceIdentityStatus Status { get; set; }
    public string? Purpose { get; set; }
    public string? TechnicalContact { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public DateTime? LastAccessDate { get; set; }
    public string? AccessFrequency { get; set; }
    public string? Description { get; set; }

    // IHasBusinessApplication
    public int BusinessApplicationId { get; set; }
    public virtual BusinessApplication BusinessApplication { get; set; } = null!;

    // IHasBusinessAppEnvironment
    public int BusinessAppEnvironmentId { get; set; }
    public virtual BusinessAppEnvironment BusinessAppEnvironment { get; set; } = null!;

    // IHasPrimaryOwner
    public int PrimaryOwnerId { get; set; }
    public virtual BusinessAppUser PrimaryOwner { get; set; } = null!;

    // IHasSecondaryOwner
    public int? SecondaryOwnerId { get; set; }
    public virtual BusinessAppUser? SecondaryOwner { get; set; }

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
    public string? SourceSystem { get; set; }
    public string? SourceObjectId { get; set; }
    public Dictionary<string, string> Attributes { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public enum EBusinessAppResourceIdentityStatus
    {
        Active = 1,
        Inactive = 2,
        Expired = 3,
        Disabled = 4
    }

    public enum EBusinessAppResourceIdentityType
    {
        ServiceAccount = 1,
        SystemAccount = 2,
        ApplicationAccount = 3,
        SharedAccount = 4,
        GenericAccount = 5
    }
}
