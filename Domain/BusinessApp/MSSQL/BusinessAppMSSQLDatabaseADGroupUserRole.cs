namespace IAMBuddy.Domain.BusinessApp.MSSQL;
using IAMBuddy.Domain.BusinessApp;

public class BusinessAppMSSQLDatabaseADGroupUserRole : AppOwnedBusinessAppResource
{
    public DateTime AssignedDate { get; set; }
    public string? AssignedBy { get; set; }
    public int DatabaseADGroupUserId { get; set; }
    public virtual BusinessAppMSSQLDatabaseADGroupUser DatabaseADGroupUser { get; set; } = null!;
    public int DatabaseRoleId { get; set; }
    public virtual BusinessAppMSSQLDatabaseRole DatabaseRole { get; set; } = null!;
}
