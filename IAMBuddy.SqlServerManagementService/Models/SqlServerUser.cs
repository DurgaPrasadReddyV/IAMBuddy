using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IAMBuddy.SqlServerManagementService.Models;

public class SqlServerUser
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(128)]
    public string UserName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(128)]
    public string DatabaseName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(128)]
    public string ServerInstance { get; set; } = string.Empty;
    
    [MaxLength(256)]
    public string? Sid { get; set; }
    
    public bool IsEnabled { get; set; } = true;
    
    [MaxLength(50)]
    public string UserType { get; set; } = string.Empty; // SQL_USER, WINDOWS_USER, etc.
    
    [MaxLength(128)]
    public string? DefaultSchema { get; set; }
    
    // Foreign key to SqlServerLogin
    public int? LoginId { get; set; }
    
    [ForeignKey(nameof(LoginId))]
    public SqlServerLogin? Login { get; set; }
    
    // Audit fields
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
    
    [MaxLength(100)]
    public string CreatedBy { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string ModifiedBy { get; set; } = string.Empty;
    
    // Navigation properties
    public ICollection<SqlServerRoleAssignment> RoleAssignments { get; set; } = new List<SqlServerRoleAssignment>();
    public ICollection<SqlServerOperation> Operations { get; set; } = new List<SqlServerOperation>();
}