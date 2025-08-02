namespace IAMBuddy.Domain.BusinessApp.MSSQL;

using IAMBuddy.Domain.Common;

public class BusinessAppMSSQLDatabaseADHumanUserRole : HumanIdentityOwnedResource
{
    public DateTime AssignedDate { get; set; }
    public string? AssignedBy { get; set; }
    public int DatabaseADHumanUserId { get; set; }
    public virtual BusinessAppMSSQLDatabaseADHumanUser DatabaseADHumanUser { get; set; } = null!;
    public int DatabaseRoleId { get; set; }
    public virtual BusinessAppMSSQLDatabaseRole DatabaseRole { get; set; } = null!;
}
