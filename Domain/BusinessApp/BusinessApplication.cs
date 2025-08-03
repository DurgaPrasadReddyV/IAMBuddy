namespace IAMBuddy.Domain.BusinessApp;

using IAMBuddy.Domain.Common;
using IAMBuddy.Domain.Enums;

public class BusinessApplication : Resource
{
    public string? ShortName { get; set; }
    public string? BusinessPurpose { get; set; }
    public BusinessAppStatus Status { get; set; }
    public BusinessAppCriticality Criticality { get; set; }
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
    public int? TechnicalContactId { get; set; }
    public virtual HumanIdentity TechnicalContact { get; set; } = null!;
    public int? BusinessContactId { get; set; }
    public virtual HumanIdentity BusinessContact { get; set; } = null!;
    public int PrimaryOwnerId { get; set; }
    public virtual HumanIdentity PrimaryOwner { get; set; } = null!;
    public int? AlternateOwnerId { get; set; }
    public virtual HumanIdentity? AlternateOwner { get; set; }
}
