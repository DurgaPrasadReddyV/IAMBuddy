namespace IAMBuddy.Domain.BusinessApp;

using IAMBuddy.Domain.Common;
using IAMBuddy.Domain.Enums;

public class BusinessAppOwnedResourceIdentity : Identity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public BusinessAppResourceIdentityType Type { get; set; }
    public BusinessAppResourceIdentityStatus Status { get; set; }
    public string? Purpose { get; set; }
    public string? TechnicalContact { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public DateTime? LastAccessDate { get; set; }
    public string? AccessFrequency { get; set; }
    public string? Description { get; set; }
    public int BusinessApplicationId { get; set; }
    public virtual BusinessApplication BusinessApplication { get; set; } = null!;
    public int? BusinessAppEnvironmentId { get; set; }
    public virtual BusinessAppEnvironment? BusinessAppEnvironment { get; set; }
    public int PrimaryOwnerId { get; set; }
    public virtual BusinessAppUserIdentity PrimaryOwner { get; set; } = null!;
    public int? AlternateOwnerId { get; set; }
    public virtual BusinessAppUserIdentity? AlternateOwner { get; set; }
}
