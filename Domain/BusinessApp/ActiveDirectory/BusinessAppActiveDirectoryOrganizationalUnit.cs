namespace IAMBuddy.Domain.BusinessApp.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IAMBuddy.Domain.Common;

public class BusinessAppActiveDirectoryOrganizationalUnit : IBusinessAppOwnedResource
{
    [Required] public string DistinguishedName { get; set; } = null!;
    public int? ParentOuId { get; set; }
    public List<BusinessAppActiveDirectoryOrganizationalUnit> Children { get; set; } = [];
    public int DomainId { get; set; }
    public virtual BusinessAppActiveDirectoryDirectoryDomain Domain { get; set; } = null!;

    // IBusinessAppOwnedResource
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

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public IResource.EResourceType ResourceType { get; set; }

    public int AuthoritativeSourceId { get; set; }
    public virtual AuthoritativeSource AuthoritativeSource { get; set; } = null!;

    public int BusinessAppResourceIdentityId { get; set; }
    public virtual BusinessAppResourceIdentity BusinessAppResourceIdentity { get; set; } = null!;
}
