namespace IAMBuddy.Domain.BusinessApp.MSSQL;

using System.ComponentModel.DataAnnotations;
using IAMBuddy.Domain.BusinessApp;
using IAMBuddy.Domain.BusinessApp.MSSQL.Server;
using IAMBuddy.Domain.Common;

public class BusinessAppMSSQLServerListener : IBusinessAppOwnedResource
{
    public string IPAddress { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Protocol { get; set; } = string.Empty;
    public virtual ICollection<BusinessAppMSSQLServerInstance> Instances { get; set; } = [];

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
