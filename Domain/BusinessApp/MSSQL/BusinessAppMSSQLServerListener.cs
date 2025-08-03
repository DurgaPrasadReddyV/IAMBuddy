namespace IAMBuddy.Domain.BusinessApp.MSSQL;
using IAMBuddy.Domain.BusinessApp;

public class BusinessAppMSSQLServerListener : BusinessAppOwnedResource
{
    public string IPAddress { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Protocol { get; set; } = string.Empty;
    public virtual ICollection<BusinessAppMSSQLServerInstance> Instances { get; set; } = [];
}
