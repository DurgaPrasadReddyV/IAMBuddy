namespace IAMBuddy.Tools.Data.Entities;

// New Entities for Extended Functionality

// Entity: Human Identity
public class HumanIdentity : AuditableEntity
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? JobTitle { get; set; }
    public string? Department { get; set; }
    public string? Division { get; set; }
    public string? CostCenter { get; set; }
    public string? Location { get; set; }
    public string? Manager { get; set; }
    public DateTime? HireDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public HumanIdentityStatus Status { get; set; }
    public string? EmployeeId { get; set; }
    public string? Company { get; set; }
    public bool IsContractor { get; set; }
    public DateTime? ContractEndDate { get; set; }
    public string? Description { get; set; }

    // Navigation properties
    public virtual ICollection<ActiveDirectoryAccount> ActiveDirectoryAccounts { get; set; } = [];
    public virtual ICollection<NonHumanIdentity> PrimaryOwnedNonHumanIdentities { get; set; } = [];
    public virtual ICollection<NonHumanIdentity> AlternateOwnedNonHumanIdentities { get; set; } = [];
    public virtual ICollection<BusinessApplication> ApplicationOwned { get; set; } = [];
    public virtual ICollection<BusinessApplication> ApplicationAlternateOwned { get; set; } = [];
    public virtual ICollection<BusinessApplication> ProductOwned { get; set; } = [];
    public virtual ICollection<BusinessApplication> ProductAlternateOwned { get; set; } = [];
    public virtual ICollection<BusinessApplicationTeamMember> BusinessApplicationMemberships { get; set; } = [];
}
