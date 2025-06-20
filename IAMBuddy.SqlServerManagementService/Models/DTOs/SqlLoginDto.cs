using System.ComponentModel.DataAnnotations;

namespace IAMBuddy.SqlServerManagementService.Models.DTOs;

/// <summary>
/// Data transfer object for SQL Server Login operations
/// </summary>
public class SqlLoginDto
{
    /// <summary>
    /// Gets or sets the login identifier
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the login name
    /// </summary>
    [Required]
    [MaxLength(128)]
    public string LoginName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the login type (SQL, Windows, Certificate, etc.)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string LoginType { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the Security Identifier (SID)
    /// </summary>
    [MaxLength(256)]
    public string? Sid { get; set; }
    
    /// <summary>
    /// Gets or sets whether the login is enabled
    /// </summary>
    public bool IsEnabled { get; set; } = true;
    
    /// <summary>
    /// Gets or sets whether the login is locked
    /// </summary>
    public bool IsLocked { get; set; } = false;
    
    /// <summary>
    /// Gets or sets the password expiry date
    /// </summary>
    public DateTime? PasswordExpiryDate { get; set; }
    
    /// <summary>
    /// Gets or sets the last login date
    /// </summary>
    public DateTime? LastLoginDate { get; set; }
    
    /// <summary>
    /// Gets or sets the server instance
    /// </summary>
    [Required]
    [MaxLength(128)]
    public string ServerInstance { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the created date
    /// </summary>
    public DateTime CreatedDate { get; set; }
    
    /// <summary>
    /// Gets or sets the modified date
    /// </summary>
    public DateTime ModifiedDate { get; set; }
    
    /// <summary>
    /// Gets or sets who created the login
    /// </summary>
    [MaxLength(100)]
    public string CreatedBy { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets who last modified the login
    /// </summary>
    [MaxLength(100)]
    public string ModifiedBy { get; set; } = string.Empty;
}

/// <summary>
/// Request model for creating a new SQL Server login
/// </summary>
public class CreateSqlLoginRequest
{
    /// <summary>
    /// Gets or sets the login name
    /// </summary>
    [Required]
    [MaxLength(128)]
    public string LoginName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the password for SQL logins
    /// </summary>
    [MaxLength(256)]
    public string? Password { get; set; }
    
    /// <summary>
    /// Gets or sets the login type (SQL, Windows, Certificate, etc.)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string LoginType { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the server instance
    /// </summary>
    [Required]
    [MaxLength(128)]
    public string ServerInstance { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the default database
    /// </summary>
    [MaxLength(128)]
    public string? DefaultDatabase { get; set; }
    
    /// <summary>
    /// Gets or sets whether the login is enabled
    /// </summary>
    public bool IsEnabled { get; set; } = true;
}

/// <summary>
/// Request model for updating a SQL Server login
/// </summary>
public class UpdateSqlLoginRequest
{
    /// <summary>
    /// Gets or sets whether the login is enabled
    /// </summary>
    public bool? IsEnabled { get; set; }
    
    /// <summary>
    /// Gets or sets the new password
    /// </summary>
    [MaxLength(256)]
    public string? NewPassword { get; set; }
    
    /// <summary>
    /// Gets or sets the default database
    /// </summary>
    [MaxLength(128)]
    public string? DefaultDatabase { get; set; }
}

/// <summary>
/// Request model for bulk login operations
/// </summary>
public class BulkLoginOperationRequest
{
    /// <summary>
    /// Gets or sets the list of login names to operate on
    /// </summary>
    [Required]
    public List<string> LoginNames { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the server instance
    /// </summary>
    [Required]
    [MaxLength(128)]
    public string ServerInstance { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the operation type (enable, disable, delete)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Operation { get; set; } = string.Empty;
}