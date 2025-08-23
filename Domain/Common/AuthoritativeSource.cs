namespace IAMBuddy.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;
using IAMBuddy.Domain.BusinessApp;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory.Accounts;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory.DirectoryGroups;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory.Memberships;

public class AuthoritativeSource : IAuditableEntity
{
    public string? SourceName { get; set; }
    public string? SourceType { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset? LastSynchronizationTimestamp { get; set; }
    public virtual ICollection<HumanIdentity> HumanIdentities { get; set; } = [];
    public virtual ICollection<BusinessApplication> BusinessApplications { get; set; } = [];
    public virtual ICollection<BusinessAppActiveDirectoryDirectoryForest> BusinessAppActiveDirectoryDirectoryForests { get; set; } = [];
    public virtual ICollection<BusinessAppActiveDirectoryDirectoryDomain> BusinessAppActiveDirectoryDirectoryDomains { get; set; } = [];
    public virtual ICollection<BusinessAppActiveDirectoryOrganizationalUnit> BusinessAppActiveDirectoryOrganizationalUnits { get; set; } = [];
    public virtual ICollection<BusinessAppHumanActiveDirectoryAccount> BusinessAppHumanActiveDirectoryAccounts { get; set; } = [];
    public virtual ICollection<BusinessAppServiceActiveDirectoryAccount> BusinessAppServiceActiveDirectoryAccounts { get; set; } = [];
    public virtual ICollection<BusinessAppActiveDirectoryGroup> BusinessAppActiveDirectoryGroups { get; set; } = [];
    public virtual ICollection<BusinessAppHumanActiveDirectoryGroupMembership> BusinessAppHumanActiveDirectoryGroupMemberships { get; set; } = [];
    public virtual ICollection<BusinessAppServiceActiveDirectoryGroupMembership> BusinessAppServiceActiveDirectoryGroupMemberships { get; set; } = [];

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
}
