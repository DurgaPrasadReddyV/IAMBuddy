namespace IAMBuddy.Domain.BusinessApp.MSSQL.Database.Roles;

using System.ComponentModel.DataAnnotations;
using IAMBuddy.Domain.Common;

public class BusinessAppMSSQLDatabasePermission : IBusinessAppOwnedResource
{
    public string PermissionName { get; set; } = string.Empty;
    public EPermissionType PermissionType { get; set; }
    public ESecurableType SecurableType { get; set; }
    public string SecurableName { get; set; } = string.Empty;
    public string? GrantedBy { get; set; }
    public DateTimeOffset? GrantedDate { get; set; }
    public int? DatabaseRoleId { get; set; }
    public virtual BusinessAppMSSQLDatabaseRole? DatabaseRole { get; set; }

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

    public enum ESecurableType
    {
        Server = 1,
        Database = 2,
        Schema = 3,
        Table = 4,
        View = 5,
        StoredProcedure = 6,
        Function = 7,
        Column = 8
    }

    public enum EPermissionType
    {
        Grant = 1,
        Deny = 2,
        Revoke = 3
    }
}
