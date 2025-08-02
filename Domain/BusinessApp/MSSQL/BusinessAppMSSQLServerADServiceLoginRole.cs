namespace IAMBuddy.Domain.BusinessApp.MSSQL;
using IAMBuddy.Domain.BusinessApp;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory;

public class BusinessAppMSSQLServerADServiceLoginRole : AppOwnedBusinessAppResource
{
    public DateTime AssignedDate { get; set; }
    public string? AssignedBy { get; set; }
    public int? BusinessAppMSSQLServerADServiceLoginId { get; set; }
    public virtual BusinessAppMSSQLServerADServiceLogin BusinessAppMSSQLServerADServiceLogin { get; set; } = null!;
    public int ServerRoleId { get; set; }
    public virtual BusinessAppMSSQLServerRole ServerRole { get; set; } = null!;
}
