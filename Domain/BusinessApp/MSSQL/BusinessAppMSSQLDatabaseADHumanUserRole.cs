namespace IAMBuddy.Domain.BusinessApp.MSSQL;

public class BusinessAppMSSQLDatabaseADHumanUserRole : BusinessAppUserOwnedResource
{
    public DateTime AssignedDate { get; set; }
    public string? AssignedBy { get; set; }
    public int DatabaseADHumanUserId { get; set; }
    public virtual BusinessAppMSSQLDatabaseADHumanUser DatabaseADHumanUser { get; set; } = null!;
    public int DatabaseRoleId { get; set; }
    public virtual BusinessAppMSSQLDatabaseRole DatabaseRole { get; set; } = null!;
}
