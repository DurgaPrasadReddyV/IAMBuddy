namespace IAMBuddy.Domain.BusinessApp.MSSQL;

using System.ComponentModel.DataAnnotations;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory;
using IAMBuddy.Domain.Common;

public class BusinessAppMSSQLServerADHumanLogin : IBusinessAppUserOwnedResource, IHasBusinessAppHumanActiveDirectoryAccount
{
    public AuthenticationType Authentication { get; set; }
    public LoginStatus Status { get; set; }
    public string? DefaultDatabase { get; set; }
    public string? DefaultLanguage { get; set; }
    public DateTime? PasswordExpirationDate { get; set; }
    public bool IsPasswordExpired { get; set; }
    public bool IsLocked { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public int ServerId { get; set; }
    public virtual BusinessAppMSSQLServer Server { get; set; } = null!;
    public int BusinessAppHumanActiveDirectoryAccountId { get; set; }
    public virtual BusinessAppHumanActiveDirectoryAccount BusinessAppHumanActiveDirectoryAccount { get; set; } = null!;

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

    public int BusinessAppUserId { get; set; }
    public virtual BusinessAppUser BusinessAppUser { get; set; } = null!;

    public enum LoginStatus
    {
        Active = 1,
        Disabled = 2,
        Locked = 3,
        Expired = 4
    }

    public enum AuthenticationType
    {
        SqlServer = 1,
        Windows = 2,
        AzureAD = 3
    }
}
