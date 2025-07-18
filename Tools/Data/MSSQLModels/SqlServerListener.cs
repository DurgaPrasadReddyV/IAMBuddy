namespace IAMBuddy.Tools.Data.MSSQLModels;

// Entity: SQL Server Listener
public class SqlServerListener : AuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string IPAddress { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Protocol { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? Description { get; set; }

    // Navigation properties
    public virtual ICollection<SqlServerInstance> Instances { get; set; } = new List<SqlServerInstance>();
}
