namespace IAMBuddy.Domain.BusinessApp.MSSQL;

public class BusinessAppMSSQLDatabaseADServiceUserRole : AppOwnedBusinessAppResource
{
    public DateTime AssignedDate { get; set; }
    public string? AssignedBy { get; set; }
    public int DatabaseADServiceUserId { get; set; }
    public virtual BusinessAppMSSQLDatabaseADServiceUser DatabaseADServiceUser { get; set; } = null!;
    public int DatabaseRoleId { get; set; }
    public virtual BusinessAppMSSQLDatabaseRole DatabaseRole { get; set; } = null!;
}
