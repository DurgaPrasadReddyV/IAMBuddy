namespace IAMBuddy.Domain.BusinessApp.MSSQL;

public class BusinessAppMSSQLDatabaseADHumanUser : BusinessAppUserOwnedResource
{
    public string? DefaultSchema { get; set; }
    public string UserType { get; set; } = string.Empty;
    public int DatabaseId { get; set; }
    public virtual BusinessAppMSSQLDatabase Database { get; set; } = null!;
    public int? ADHumanLoginId { get; set; }
    public virtual BusinessAppMSSQLServerADHumanLogin ADHumanLogin { get; set; } = null!;
}
