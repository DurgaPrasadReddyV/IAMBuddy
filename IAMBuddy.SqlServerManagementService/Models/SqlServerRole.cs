using System.ComponentModel.DataAnnotations;

namespace IAMBuddy.SqlServerManagementService.Models;

public class SqlServerRole
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(128)]
    public string RoleName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string RoleType { get; set; } = string.Empty; // Server, Database, Application
    
    [MaxLength(128)]
    public string? DatabaseName { get; set; }
    
    [Required]
    [MaxLength(128)]
    public string ServerInstance { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    public bool IsBuiltIn { get; set; } = false;
    
    public bool IsEnabled { get; set; } = true;
    
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