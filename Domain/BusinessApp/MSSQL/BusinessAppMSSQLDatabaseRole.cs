namespace IAMBuddy.Domain.BusinessApp.MSSQL;
using IAMBuddy.Domain.BusinessApp;
using IAMBuddy.Domain.Enums;

public class BusinessAppMSSQLDatabaseRole : AppOwnedBusinessAppResource
{
    public int DatabaseId { get; set; }
    public RoleType RoleType { get; set; }
    public bool IsFixedRole { get; set; }
    public virtual BusinessAppMSSQLDatabase Database { get; set; } = null!;
    public virtual ICollection<BusinessAppMSSQLDatabasePermission> Permissions { get; set; } = [];
}
