namespace IAMBuddy.Domain.BusinessApp.MSSQL.Server.Logins;

using System.ComponentModel.DataAnnotations;
using IAMBuddy.Domain.BusinessApp;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory.Accounts;
using IAMBuddy.Domain.BusinessApp.MSSQL.Server;
using IAMBuddy.Domain.Common;

public class BusinessAppMSSQLServerADServiceLogin : IBusinessAppOwnedResource, IHasBusinessAppServiceActiveDirectoryAccount
{
    public EAuthenticationType AuthenticationType { get; set; }
    public ELoginStatus LoginStatus { get; set; }
    public string? DefaultDatabase { get; set; }
    public string? DefaultLanguage { get; set; }
    public DateTimeOffset? PasswordExpirationDate { get; set; }
    public bool IsPasswordExpired { get; set; }
    public bool IsLocked { get; set; }
    public DateTimeOffset? LastLoginDate { get; set; }
    public int ServerId { get; set; }
    public virtual BusinessAppMSSQLServer Server { get; set; } = null!;
    public int BusinessAppServiceActiveDirectoryAccountId { get; set; }
    public virtual BusinessAppServiceActiveDirectoryAccount BusinessAppServiceActiveDirectoryAccount { get; set; } = null!;

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

    public enum ELoginStatus
    {
        Active = 1,
        Disabled = 2,
        Locked = 3,
        Expired = 4
    }

    public enum EAuthenticationType
    {
        SqlServer = 1,
        Windows = 2,
        AzureAD = 3
    }
}
