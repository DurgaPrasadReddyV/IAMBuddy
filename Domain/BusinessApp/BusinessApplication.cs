namespace IAMBuddy.Domain.BusinessApp;

using System.ComponentModel.DataAnnotations;
using IAMBuddy.Domain.Common;

public class BusinessApplication : IResource, IHasPrimaryOwner, IHasSecondaryOwner
{
    public string? BusinessPurpose { get; set; }
    public EBusinessAppStatus Status { get; set; }
    public EBusinessAppCriticality Criticality { get; set; }
    public string? VendorName { get; set; }
    public string? Version { get; set; }
    public DateTimeOffset? GoLiveDate { get; set; }
    public DateTimeOffset? EndOfLifeDate { get; set; }
    public decimal? AnnualCost { get; set; }
    public string? ComplianceRequirements { get; set; }
    public string? DataClassification { get; set; }
    public bool IsCustomDeveloped { get; set; }
    public string? SourceCodeRepository { get; set; }
    public string? DocumentationLink { get; set; }
    public int? TechnicalContactId { get; set; }
    public virtual BusinessAppUser TechnicalContact { get; set; } = null!;
    public int? BusinessContactId { get; set; }
    public virtual BusinessAppUser BusinessContact { get; set; } = null!;
    public virtual ICollection<BusinessAppUser> BusinessAppUsers { get; set; } = [];
    public virtual ICollection<BusinessAppEnvironment> BusinessAppEnvironments { get; set; } = [];
    public virtual ICollection<BusinessAppResourceIdentity> BusinessAppResourceIdentities { get; set; } = [];


    // IHasPrimaryOwner
    public int PrimaryOwnerId { get; set; }
    public virtual BusinessAppUser PrimaryOwner { get; set; } = null!;

    // IHasSecondaryOwner
    public int? SecondaryOwnerId { get; set; }
    public virtual BusinessAppUser? SecondaryOwner { get; set; }

    // IResource
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public IResource.EResourceType ResourceType { get; set; }
    public int AuthoritativeSourceId { get; set; }
    public virtual AuthoritativeSource AuthoritativeSource { get; set; } = null!;
    public int Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    [Timestamp] public byte[]? RowVersion { get; set; }
    public Dictionary<string, string> Attributes { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public enum EBusinessAppCriticality
    {
        Critical = 1,
        High = 2,
        Medium = 3,
        Low = 4
    }

    public enum EBusinessAppStatus
    {
        Active = 1,
        Inactive = 2,
        Deprecated = 3,
        UnderDevelopment = 4
    }
}
