namespace IAMBuddy.Domain.BusinessApp.MSSQL;

public class BusinessAppMSSQLDatabaseSQLAccountUserRole : AppOwnedBusinessAppResource
{
    public DateTime AssignedDate { get; set; }
    public string? AssignedBy { get; set; }
    public int DatabaseSQLAccountUserId { get; set; }
    public virtual BusinessAppMSSQLDatabaseSQLAccountUser DatabaseSQLAccountUser { get; set; } = null!;
    public int DatabaseRoleId { get; set; }
    public virtual BusinessAppMSSQLDatabaseRole DatabaseRole { get; set; } = null!;
}
