namespace IAMBuddy.Domain.BusinessApp.MSSQL;
using IAMBuddy.Domain.BusinessApp;

public class BusinessAppMSSQLDatabaseADGroupUser : AppOwnedBusinessAppResource
{
    public string? DefaultSchema { get; set; }
    public string UserType { get; set; } = string.Empty;
    public int DatabaseId { get; set; }
    public virtual BusinessAppMSSQLDatabase Database { get; set; } = null!;
    public int? ADGroupLoginId { get; set; }
    public virtual BusinessAppMSSQLServerADGroupLogin ADGroupLogin { get; set; } = null!;
}
