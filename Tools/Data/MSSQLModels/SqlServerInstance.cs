namespace IAMBuddy.Tools.Data.MSSQLModels;

// Entity: SQL Server Instance
public class SqlServerInstance : AuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ServerId { get; set; }
    public int? ListenerId { get; set; }
    public string Port { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string ServiceAccount { get; set; } = string.Empty;
    public string Collation { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? Description { get; set; }

    // Navigation properties
    public virtual SqlServer Server { get; set; } = null!;
    public virtual SqlServerListener? Listener { get; set; }
    public virtual ICollection<Database> Databases { get; set; } = new List<Database>();
}
