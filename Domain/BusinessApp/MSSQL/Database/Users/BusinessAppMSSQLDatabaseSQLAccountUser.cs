namespace IAMBuddy.Domain.BusinessApp.MSSQL.Database.Users;
using System.ComponentModel.DataAnnotations;
using IAMBuddy.Domain.BusinessApp.MSSQL.Database;
using IAMBuddy.Domain.BusinessApp.MSSQL.Server.Logins;
using IAMBuddy.Domain.Common;

public class BusinessAppMSSQLDatabaseSQLAccountUser : IBusinessAppOwnedResource
{
    public string? DefaultSchema { get; set; }
    public string UserType { get; set; } = string.Empty;
    public int DatabaseId { get; set; }
    public virtual BusinessAppMSSQLDatabase Database { get; set; } = null!;
    public int? SQLAccountLoginId { get; set; }
    public virtual BusinessAppMSSQLServerSQLAccountLogin SQLAccountLogin { get; set; } = null!;

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

    public int BusinessApplicationId { get; set; }
    public virtual BusinessApplication BusinessApplication { get; set; } = null!;

    public int BusinessAppResourceIdentityId { get; set; }
    public virtual BusinessAppResourceIdentity BusinessAppResourceIdentity { get; set; } = null!;
}
