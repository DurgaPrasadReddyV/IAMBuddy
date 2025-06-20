using System.ComponentModel.DataAnnotations;

namespace IAMBuddy.SqlServerManagementService.Models.DTOs;

/// <summary>
/// Data transfer object for SQL Server Database User operations
/// </summary>
public class DatabaseUserDto
{
    /// <summary>
    /// Gets or sets the user identifier
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the user name
    /// </summary>
    [Required]
    [MaxLength(128)]
    public string UserName { get; set; } = string.Empty;
    
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
    /// Gets or sets the Security Identifier (SID)
    /// </summary>
    [MaxLength(256)]
    public string? Sid { get; set; }
    
    /// <summary>
    /// Gets or sets whether the user is enabled
    /// </summary>
    public bool IsEnabled { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the user type (SQL_USER, WINDOWS_USER, etc.)
    /// </summary>
    [MaxLength(50)]
    public string UserType { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the default schema
    /// </summary>
    [MaxLength(128)]
    public string? DefaultSchema { get; set; }
    
    /// <summary>
    /// Gets or sets the associated login ID
    /// </summary>
    public int? LoginId { get; set; }
    
    /// <summary>
    /// Gets or sets the login name (read-only)
    /// </summary>
    public string? LoginName { get; set; }
    
    /// <summary>
    /// Gets or sets the created date
    /// </summary>
    public DateTime CreatedDate { get; set; }
    
    /// <summary>
    /// Gets or sets the modified date
    /// </summary>
    public DateTime ModifiedDate { get; set; }
    
    /// <summary>
    /// Gets or sets who created the user
    /// </summary>
    [MaxLength(100)]
    public string CreatedBy { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets who last modified the user
    /// </summary>
    [MaxLength(100)]
    public string ModifiedBy { get; set; } = string.Empty;
}

/// <summary>
/// Request model for creating a new database user
/// </summary>
public class CreateDatabaseUserRequest
{
    /// <summary>
    /// Gets or sets the user name
    /// </summary>
    [Required]
    [MaxLength(128)]
    public string UserName { get; set; } = string.Empty;
    
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
    /// Gets or sets the login name to associate with the user
    /// </summary>
    [MaxLength(128)]
    public string? LoginName { get; set; }
    
    /// <summary>
    /// Gets or sets the user type (SQL_USER, WINDOWS_USER, etc.)
    /// </summary>
    [MaxLength(50)]
    public string UserType { get; set; } = "SQL_USER";
    
    /// <summary>
    /// Gets or sets the default schema
    /// </summary>
    [MaxLength(128)]
    public string? DefaultSchema { get; set; } = "dbo";
    
    /// <summary>
    /// Gets or sets whether the user is enabled
    /// </summary>
    public bool IsEnabled { get; set; } = true;
}

/// <summary>
/// Request model for updating a database user
/// </summary>
public class UpdateDatabaseUserRequest
{
    /// <summary>
    /// Gets or sets whether the user is enabled
    /// </summary>
    public bool? IsEnabled { get; set; }
    
    /// <summary>
    /// Gets or sets the default schema
    /// </summary>
    [MaxLength(128)]
    public string? DefaultSchema { get; set; }
    
    /// <summary>
    /// Gets or sets the new login name to associate
    /// </summary>
    [MaxLength(128)]
    public string? LoginName { get; set; }
}

/// <summary>
/// Request model for bulk user operations
/// </summary>
public class BulkUserOperationRequest
{
    /// <summary>
    /// Gets or sets the list of user names to operate on
    /// </summary>
    [Required]
    public List<string> UserNames { get; set; } = new();
    
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