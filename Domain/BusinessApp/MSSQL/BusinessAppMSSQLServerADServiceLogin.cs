namespace IAMBuddy.Domain.BusinessApp.MSSQL;
using IAMBuddy.Domain.BusinessApp;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory;
using IAMBuddy.Domain.Enums;

public class BusinessAppMSSQLServerADServiceLogin : BusinessAppOwnedResource
{
    public AuthenticationType AuthenticationType { get; set; }
    public LoginStatus Status { get; set; }
    public string? DefaultDatabase { get; set; }
    public string? DefaultLanguage { get; set; }
    public DateTime? PasswordExpirationDate { get; set; }
    public bool IsPasswordExpired { get; set; }
    public bool IsLocked { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public int ServerId { get; set; }
    public virtual BusinessAppMSSQLServer Server { get; set; } = null!;
    public int? ActiveDirectoryHumanAccountId { get; set; }
    public virtual BusinessAppServiceActiveDirectoryAccount ActiveDirectoryServiceAccount { get; set; } = null!;
}
