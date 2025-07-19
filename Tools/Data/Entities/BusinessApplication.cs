namespace IAMBuddy.Tools.Data.Entities;

// Entity: Business Application
public class BusinessApplication : AuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ShortName { get; set; }
    public string? Description { get; set; }
    public string? BusinessPurpose { get; set; }
    public BusinessApplicationStatus Status { get; set; }
    public BusinessApplicationCriticality Criticality { get; set; }
    public int ApplicationOwnerId { get; set; }
    public int? AlternateApplicationOwnerId { get; set; }
    public int ProductOwnerId { get; set; }
    public int? AlternateProductOwnerId { get; set; }
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

    // Navigation properties
    public virtual HumanIdentity ApplicationOwner { get; set; } = null!;
    public virtual HumanIdentity? AlternateApplicationOwner { get; set; }
    public virtual HumanIdentity ProductOwner { get; set; } = null!;
    public virtual HumanIdentity? AlternateProductOwner { get; set; }
    public virtual ICollection<BusinessApplicationEnvironment> Environments { get; set; } = [];
    public virtual ICollection<BusinessApplicationTeamMember> TeamMembers { get; set; } = [];
    public virtual ICollection<NonHumanIdentity> NonHumanIdentities { get; set; } = [];
    public virtual ICollection<BusinessApplicationResource> Resources { get; set; } = [];
}
