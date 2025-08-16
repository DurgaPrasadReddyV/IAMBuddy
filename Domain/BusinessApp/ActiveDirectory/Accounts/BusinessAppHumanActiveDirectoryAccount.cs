namespace IAMBuddy.Domain.BusinessApp.ActiveDirectory.Accounts;

using System.ComponentModel.DataAnnotations;
using IAMBuddy.Domain.Common;

public class BusinessAppHumanActiveDirectoryAccount : IBusinessAppUserOwnedDirectoryPrincipal
{
    public string? GivenName { get; set; }
    public string? Surname { get; set; }
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
    public DateTimeOffset? PasswordLastSet { get; set; }
    public bool PasswordNeverExpires { get; set; }
    public bool MustChangePasswordAtNextLogon { get; set; }
    public bool SmartCardRequired { get; set; }
    public DateTimeOffset? AccountExpires { get; set; }
    public DateTimeOffset? LastLogon { get; set; }
    public string? ManagerSid { get; set; }
    public string? EmployeeId { get; set; }
    public string? Department { get; set; }
    public string? Title { get; set; }

    // IBusinessAppUserOwnedDirectoryPrincipal
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

    public int BusinessAppUserId { get; set; }
    public virtual BusinessAppUser BusinessAppUser { get; set; } = null!;

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
