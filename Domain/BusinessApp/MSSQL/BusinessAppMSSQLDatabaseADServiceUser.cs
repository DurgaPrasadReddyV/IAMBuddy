namespace IAMBuddy.Domain.BusinessApp.MSSQL;

public class BusinessAppMSSQLDatabaseADServiceUser : AppOwnedBusinessAppResource
{
    public string? DefaultSchema { get; set; }
    public string UserType { get; set; } = string.Empty;
    public int DatabaseId { get; set; }
    public virtual BusinessAppMSSQLDatabase Database { get; set; } = null!;
    public int? ADServiceLoginId { get; set; }
    public virtual BusinessAppMSSQLServerADServiceLogin ADServiceLogin { get; set; } = null!;
}
