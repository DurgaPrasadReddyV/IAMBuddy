using System.ComponentModel.DataAnnotations;

namespace IAMBuddy.SqlServerManagementService.Models.DTOs;

/// <summary>
/// Data transfer object for SQL Server Database Role operations
/// </summary>
public class DatabaseRoleDto
{
    /// <summary>
    /// Gets or sets the role identifier
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the role name
    /// </summary>
    [Required]
    [MaxLength(128)]
    public string RoleName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the database name
    /// </summary>
    [Required]
    [MaxLength(128)]
    public string DatabaseName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the server instance
    /// </summary>
    [Required]
    [MaxLength(128)]
    public string ServerInstance { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the role description
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets whether the role is built-in
    /// </summary>
    public bool IsBuiltIn { get; set; } = false;
    
    /// <summary>
    /// Gets or sets whether the role is enabled
    /// </summary>
    public bool IsEnabled { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the created date
    /// </summary>
    public DateTime CreatedDate { get; set; }
    
    /// <summary>
    /// Gets or sets the modified date
    /// </summary>
    public DateTime ModifiedDate { get; set; }
    
    /// <summary>
    /// Gets or sets who created the role
    /// </summary>
    [MaxLength(100)]
    public string CreatedBy { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets who last modified the role
    /// </summary>
    [MaxLength(100)]
    public string ModifiedBy { get; set; } = string.Empty;
}

/// <summary>
/// Request model for creating a new database role
/// </summary>
public class CreateDatabaseRoleRequest
{
    /// <summary>
    /// Gets or sets the role name
    /// </summary>
    [Required]
    [MaxLength(128)]
    public string RoleName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the database name
    /// </summary>
    [Required]
    [MaxLength(128)]
    public string DatabaseName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the server instance
    /// </summary>
    [Required]
    [MaxLength(128)]
    public string ServerInstance { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the role description
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }
}

/// <summary>
/// Request model for database role member operations
/// </summary>
public class DatabaseRoleMemberRequest
{
    /// <summary>
    /// Gets or sets the role name
    /// </summary>
    [Required]
    [MaxLength(128)]
    public string RoleName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the member name (user or role)
    /// </summary>
    [Required]
    [MaxLength(128)]
    public string MemberName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the database name
    /// </summary>
    [Required]
    [MaxLength(128)]
    public string DatabaseName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the server instance
    /// </summary>
    [Required]
    [MaxLength(128)]
    public string ServerInstance { get; set; } = string.Empty;
}

/// <summary>
/// Request model for database role permission operations
/// </summary>
public class DatabaseRolePermissionRequest
{
    /// <summary>
    /// Gets or sets the role name
    /// </summary>
    [Required]
    [MaxLength(128)]
    public string RoleName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the permission
    /// </summary>
    [Required]
    [MaxLength(128)]
    public string Permission { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the database name
    /// </summary>
    [Required]
    [MaxLength(128)]
    public string DatabaseName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the server instance
    /// </summary>
    [Required]
    [MaxLength(128)]
    public string ServerInstance { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the object name (table, view, etc.)
    /// </summary>
    [MaxLength(128)]
    public string? ObjectName { get; set; }
    
    /// <summary>
    /// Gets or sets the schema name
    /// </summary>
    [MaxLength(128)]
    public string? SchemaName { get; set; } = "dbo";
}

/// <summary>
/// Request model for bulk database role operations
/// </summary>
public class BulkDatabaseRoleOperationRequest
{
    /// <summary>
    /// Gets or sets the list of role names to operate on
    /// </summary>
    [Required]
    public List<string> RoleNames { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the database name
    /// </summary>
    [Required]
    [MaxLength(128)]
    public string DatabaseName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the server instance
    /// </summary>
    [Required]
    [MaxLength(128)]
    public string ServerInstance { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the operation type (delete, enable, disable)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Operation { get; set; } = string.Empty;
}