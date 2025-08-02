namespace IAMBuddy.Domain.BusinessApp.MSSQL;
using IAMBuddy.Domain.BusinessApp;

public class BusinessAppMSSQLServerSQLAccountLoginRole : AppOwnedBusinessAppResource
{
    public DateTime AssignedDate { get; set; }
    public string? AssignedBy { get; set; }
    public int? BusinessAppMSSQLServerSQLAccountLoginId { get; set; }
    public virtual BusinessAppMSSQLServerSQLAccountLogin BusinessAppMSSQLServerSQLAccountLogin { get; set; } = null!;
    public int ServerRoleId { get; set; }
    public virtual BusinessAppMSSQLServerRole ServerRole { get; set; } = null!;
}
