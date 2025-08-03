namespace IAMBuddy.Domain.BusinessApp.MSSQL;
using IAMBuddy.Domain.BusinessApp;
using IAMBuddy.Domain.Enums;

public class BusinessAppMSSQLServerRole : BusinessAppOwnedResource
{
    public RoleType RoleType { get; set; }
    public bool IsFixedRole { get; set; }
    public int InstanceId { get; set; }
    public virtual BusinessAppMSSQLServerInstance Instance { get; set; } = null!;
}
