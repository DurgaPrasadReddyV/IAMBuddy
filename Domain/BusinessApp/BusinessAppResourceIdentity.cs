namespace IAMBuddy.Domain.BusinessApp;

using System.ComponentModel.DataAnnotations;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory.Accounts;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory.DirectoryGroups;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory.Memberships;
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
    public DateTimeOffset? ExpirationDate { get; set; }
    public DateTimeOffset? LastAccessDate { get; set; }
    public string? AccessFrequency { get; set; }
    public string? Description { get; set; }
    public virtual ICollection<BusinessAppActiveDirectoryDirectoryForest> BusinessAppActiveDirectoryDirectoryForests { get; set; } = [];
    public virtual ICollection<BusinessAppActiveDirectoryDirectoryDomain> BusinessAppActiveDirectoryDirectoryDomains { get; set; } = [];
    public virtual ICollection<BusinessAppActiveDirectoryOrganizationalUnit> BusinessAppActiveDirectoryOrganizationalUnits { get; set; } = [];
    public virtual ICollection<BusinessAppServiceActiveDirectoryAccount> BusinessAppServiceActiveDirectoryAccounts { get; set; } = [];
    public virtual ICollection<BusinessAppActiveDirectoryGroup> BusinessAppActiveDirectoryGroups { get; set; } = [];
    public virtual ICollection<BusinessAppServiceActiveDirectoryGroupMembership> BusinessAppServiceActiveDirectoryGroupMemberships { get; set; } = [];

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
    public string CreatedBy { get; set; } = string.Empty;
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    [Timestamp] public byte[]? RowVersion { get; set; }
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
