namespace IAMBuddy.Domain.BusinessApp.ActiveDirectory.DirectoryGroups;

using System.ComponentModel.DataAnnotations;
using IAMBuddy.Domain.BusinessApp;
using IAMBuddy.Domain.Common;

public class BusinessAppActiveDirectoryGroup : IBusinessAppOwnedDirectoryPrincipal
{
    public EActiveDirectoryGroupType GroupType { get; set; }
    public EActiveDirectoryGroupScope GroupScope { get; set; }
    public string? Mail { get; set; }

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


    public Dictionary<string, string> Attributes { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public IResource.EResourceType ResourceType { get; set; }

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

    public enum EActiveDirectoryGroupScope
    {
        DomainLocal = 1,
        Global = 2,
        Universal = 3
    }

    public enum EActiveDirectoryGroupType
    {
        Security = 1,
        Distribution = 2
    }
}
