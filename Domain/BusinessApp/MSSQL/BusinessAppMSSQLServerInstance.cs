namespace IAMBuddy.Domain.BusinessApp.MSSQL;
using IAMBuddy.Domain.BusinessApp;

public class BusinessAppMSSQLServerInstance : AppOwnedBusinessAppResource
{
    public string Port { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string ServiceAccount { get; set; } = string.Empty;
    public string Collation { get; set; } = string.Empty;
    public int ServerId { get; set; }
    public virtual BusinessAppMSSQLServer Server { get; set; } = null!;
    public int? ListenerId { get; set; }
    public virtual BusinessAppMSSQLServerListener? Listener { get; set; }
    public virtual ICollection<BusinessAppMSSQLDatabase> Databases { get; set; } = [];
}
