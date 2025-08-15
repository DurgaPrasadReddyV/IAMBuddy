namespace IAMBuddy.Domain.BusinessApp.ActiveDirectory;

using System.ComponentModel.DataAnnotations;
using IAMBuddy.Domain.BusinessApp;
using IAMBuddy.Domain.Common;

public class BusinessAppServiceActiveDirectoryAccount : IBusinessAppOwnedDirectoryPrincipal
{
    public string ServiceAccountType { get; set; } = "Traditional"; // Traditional|Managed|GroupManaged
    public string? ManagedBySid { get; set; }
    public List<string> ServicePrincipalNames { get; set; } = [];
    public string? SecretRef { get; set; } // pointer in a vault
    public bool PasswordNeverExpires { get; set; } = true;

    // IBusinessAppOwnedDirectoryPrincipal
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

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public IResource.ResourceType Type { get; set; }

    public int AuthoritativeSourceId { get; set; }
    public virtual AuthoritativeSource AuthoritativeSource { get; set; } = null!;

    public int BusinessApplicationId { get; set; }
    public virtual BusinessApplication BusinessApplication { get; set; } = null!;

    public int BusinessAppResourceIdentityId { get; set; }
    public virtual BusinessAppResourceIdentity BusinessAppResourceIdentity { get; set; } = null!;

    public int DomainId { get; set; }
    public virtual BusinessAppActiveDirectoryDirectoryDomain Domain { get; set; } = null!;
    [Required] public string SamAccountName { get; set; } = null!;
    public string? UserPrincipalName { get; set; }
    [Required] public string DistinguishedName { get; set; } = null!;
    [Required] public string Sid { get; set; } = null!;
    public int? OrganizationalUnitId { get; set; }
    public virtual BusinessAppActiveDirectoryOrganizationalUnit? OrganizationalUnit { get; set; }
    public bool Enabled { get; set; } = true;
    public DateTimeOffset? WhenCreated { get; set; }
    public DateTimeOffset? WhenChanged { get; set; }
}
