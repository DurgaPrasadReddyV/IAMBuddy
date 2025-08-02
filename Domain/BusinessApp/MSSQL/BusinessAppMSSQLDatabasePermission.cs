namespace IAMBuddy.Domain.BusinessApp.MSSQL;

using IAMBuddy.Domain.Enums;

public class BusinessAppMSSQLDatabasePermission : AppOwnedBusinessAppResource
{
    public string PermissionName { get; set; } = string.Empty;
    public PermissionType PermissionType { get; set; }
    public SecurableType SecurableType { get; set; }
    public string SecurableName { get; set; } = string.Empty;
    public string? GrantedBy { get; set; }
    public DateTime? GrantedDate { get; set; }
    public int? DatabaseRoleId { get; set; }
    public virtual BusinessAppMSSQLDatabaseRole? DatabaseRole { get; set; }
}
