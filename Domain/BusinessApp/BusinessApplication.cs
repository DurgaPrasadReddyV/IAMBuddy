namespace IAMBuddy.Domain.BusinessApp;

using IAMBuddy.Domain.Common;
using IAMBuddy.Domain.Enums;

public class BusinessApplication : HumanIdentityOwnedResource
{
    public string? ShortName { get; set; }
    public string? BusinessPurpose { get; set; }
    public BusinessAppStatus Status { get; set; }
    public BusinessAppCriticality Criticality { get; set; }
    public string? TechnicalContact { get; set; }
    public string? BusinessContact { get; set; }
    public string? VendorName { get; set; }
    public string? Version { get; set; }
    public DateTime? GoLiveDate { get; set; }
    public DateTime? EndOfLifeDate { get; set; }
    public decimal? AnnualCost { get; set; }
    public string? ComplianceRequirements { get; set; }
    public string? DataClassification { get; set; }
    public bool IsCustomDeveloped { get; set; }
    public string? SourceCodeRepository { get; set; }
    public string? DocumentationLink { get; set; }
    public virtual ICollection<BusinessAppEnvironment> Environments { get; set; } = [];
    public virtual ICollection<BusinessAppResourceIdentity> BusinessAppResourceIdentities { get; set; } = [];
    public virtual ICollection<BusinessAppUser> BusinessAppHumanIdentities { get; set; } = [];
}
