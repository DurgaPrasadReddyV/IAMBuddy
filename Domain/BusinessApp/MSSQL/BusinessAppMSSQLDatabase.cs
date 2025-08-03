namespace IAMBuddy.Domain.BusinessApp.MSSQL;
using IAMBuddy.Domain.BusinessApp;

public class BusinessAppMSSQLDatabase : BusinessAppOwnedResource
{
    public string Collation { get; set; } = string.Empty;
    public string RecoveryModel { get; set; } = string.Empty;
    public string CompatibilityLevel { get; set; } = string.Empty;
    public int InstanceId { get; set; }
    public virtual BusinessAppMSSQLServerInstance Instance { get; set; } = null!;
}
