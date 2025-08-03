namespace IAMBuddy.Domain.BusinessApp.MSSQL;

public class BusinessAppMSSQLServerADHumanLoginRole : BusinessAppUserOwnedResource
{
    public DateTime AssignedDate { get; set; }
    public string? AssignedBy { get; set; }
    public int? BusinessAppMSSQLServerADHumanLoginId { get; set; }
    public virtual BusinessAppMSSQLServerADHumanLogin BusinessAppMSSQLServerADHumanLogin { get; set; } = null!;
    public int ServerRoleId { get; set; }
    public virtual BusinessAppMSSQLServerRole ServerRole { get; set; } = null!;
}
