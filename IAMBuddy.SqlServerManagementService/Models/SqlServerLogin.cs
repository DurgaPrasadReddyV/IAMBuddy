using System.ComponentModel.DataAnnotations;

namespace IAMBuddy.SqlServerManagementService.Models;

public class SqlServerLogin
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(128)]
    public string LoginName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string LoginType { get; set; } = string.Empty; // SQL, Windows, Certificate, etc.
    
    [MaxLength(256)]
    public string? Sid { get; set; }
    
    public bool IsEnabled { get; set; } = true;
    
    public bool IsLocked { get; set; } = false;
    
    public DateTime? PasswordExpiryDate { get; set; }
    
    public DateTime? LastLoginDate { get; set; }
    
    [Required]
    [MaxLength(128)]
    public string ServerInstance { get; set; } = string.Empty;
    
    // Audit fields
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
    
    [MaxLength(100)]
    public string CreatedBy { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string ModifiedBy { get; set; } = string.Empty;
    
    // Navigation properties
    public ICollection<SqlServerUser> Users { get; set; } = new List<SqlServerUser>();
    public ICollection<SqlServerRoleAssignment> RoleAssignments { get; set; } = new List<SqlServerRoleAssignment>();
    public ICollection<SqlServerOperation> Operations { get; set; } = new List<SqlServerOperation>();
}