namespace IAMBuddy.Domain.BusinessApp.MSSQL;

public class BusinessAppMSSQLDatabaseSQLAccountUser : BusinessAppOwnedResource
{
    public string? DefaultSchema { get; set; }
    public string UserType { get; set; } = string.Empty;
    public int DatabaseId { get; set; }
    public virtual BusinessAppMSSQLDatabase Database { get; set; } = null!;
    public int? SQLAccountLoginId { get; set; }
    public virtual BusinessAppMSSQLServerSQLAccountLogin SQLAccountLogin { get; set; } = null!;
}
