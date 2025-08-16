namespace IAMBuddy.Domain.BusinessApp.ActiveDirectory.Memberships;

using System.ComponentModel.DataAnnotations;
using IAMBuddy.Domain.BusinessApp;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory.Accounts;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory.DirectoryGroups;
using IAMBuddy.Domain.Common;

public class BusinessAppServiceActiveDirectoryGroupMembership : IBusinessAppOwnedDirectoryPrincipal
{
    public DateTimeOffset? MemberSince { get; set; }
    public int GroupId { get; set; }
    public virtual BusinessAppActiveDirectoryGroup Group { get; set; } = null!;
    public int? BusinessAppServiceActiveDirectoryAccountId { get; set; }
    public virtual BusinessAppServiceActiveDirectoryAccount? BusinessAppServiceActiveDirectoryAccount { get; set; }
    public bool IsDirect { get; set; } = true;
    public DateTimeOffset? AddedAt { get; set; }
    public string? AddedBy { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
    public string? SourceTag { get; set; }
    public DateTimeOffset? RemovedAt { get; set; }
    public string? RemovedBy { get; set; }

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
}
