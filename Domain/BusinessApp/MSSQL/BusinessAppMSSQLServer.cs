namespace IAMBuddy.Domain.BusinessApp.MSSQL;
using IAMBuddy.Domain.BusinessApp;

public class BusinessAppMSSQLServer : AppOwnedBusinessAppResource
{
    public string HostName { get; set; } = string.Empty;
    public string IPAddress { get; set; } = string.Empty;
    public string Edition { get; set; } = string.Empty;
    public string ServicePack { get; set; } = string.Empty;
    public virtual ICollection<BusinessAppMSSQLServerInstance> Instances { get; set; } = [];
}
