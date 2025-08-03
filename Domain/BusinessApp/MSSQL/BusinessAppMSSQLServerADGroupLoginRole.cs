namespace IAMBuddy.Domain.BusinessApp.MSSQL;
using IAMBuddy.Domain.BusinessApp;

public class BusinessAppMSSQLServerADGroupLoginRole : BusinessAppOwnedResource
{
    public DateTime AssignedDate { get; set; }
    public string? AssignedBy { get; set; }
    public int? BusinessAppMSSQLServerADGroupLoginId { get; set; }
    public virtual BusinessAppMSSQLServerADGroupLogin BusinessAppMSSQLServerADGroupLogin { get; set; } = null!;
    public int ServerRoleId { get; set; }
    public virtual BusinessAppMSSQLServerRole ServerRole { get; set; } = null!;
}
