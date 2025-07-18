namespace IAMBuddy.Tools.Data.MSSQLModels;

// Entity: SQL Server
public class SqlServer : AuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string HostName { get; set; } = string.Empty;
    public string IPAddress { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Edition { get; set; } = string.Empty;
    public string ServicePack { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? Description { get; set; }

    // Navigation properties
    public virtual ICollection<SqlServerInstance> Instances { get; set; } = new List<SqlServerInstance>();
    public virtual ICollection<ServerLogin> ServerLogins { get; set; } = new List<ServerLogin>();
    public virtual ICollection<ServerRole> ServerRoles { get; set; } = new List<ServerRole>();
}
