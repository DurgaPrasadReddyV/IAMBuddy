namespace IAMBuddy.Domain.BusinessApp.MSSQL;

using System.ComponentModel.DataAnnotations;
using IAMBuddy.Domain.BusinessApp;
using IAMBuddy.Domain.Common;

public class BusinessAppMSSQLServerInstance : IBusinessAppOwnedResource
{
    public string? Host { get; set; }                              // e.g., FQDN
    public int? Port { get; set; }                                 // if non-default
    public string? Version { get; set; }                           // e.g., 16.0.1000
    public string? Edition { get; set; }                           // e.g., Enterprise
    public string? Collation { get; set; }
    public int ServerId { get; set; }
    public virtual BusinessAppMSSQLServer Server { get; set; } = null!;
    public int? ListenerId { get; set; }
    public virtual BusinessAppMSSQLServerListener? Listener { get; set; }
    public virtual ICollection<BusinessAppMSSQLDatabase> Databases { get; set; } = [];

    // IBusinessAppOwnedResource
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
}
