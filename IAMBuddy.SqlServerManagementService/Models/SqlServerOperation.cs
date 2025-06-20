using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IAMBuddy.SqlServerManagementService.Models;

public class SqlServerOperation
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string OperationType { get; set; } = string.Empty; // CREATE, UPDATE, DELETE, GRANT, REVOKE
    
    [Required]
    [MaxLength(50)]
    public string ResourceType { get; set; } = string.Empty; // LOGIN, USER, ROLE, ROLE_ASSIGNMENT
    
    [Required]
    [MaxLength(128)]
    public string ServerInstance { get; set; } = string.Empty;
    
    [MaxLength(128)]
    public string? DatabaseName { get; set; }
    
    [MaxLength(255)]
    public string? ResourceName { get; set; }
    
    [Required]
    public string OperationDetails { get; set; } = string.Empty; // JSON with operation details
    
    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = string.Empty; // PENDING, IN_PROGRESS, SUCCESS, FAILED
    
    [MaxLength(1000)]
    public string? ErrorMessage { get; set; }
    
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    
    public DateTime? EndTime { get; set; }
    
    public int? DurationMs { get; set; }
    
    // Foreign keys for related entities
    public int? LoginId { get; set; }
    
    [ForeignKey(nameof(LoginId))]
    public SqlServerLogin? Login { get; set; }
    
    public int? UserId { get; set; }
    
    [ForeignKey(nameof(UserId))]
    public SqlServerUser? User { get; set; }
    
    public int? RoleId { get; set; }
    
    [ForeignKey(nameof(RoleId))]
    public SqlServerRole? Role { get; set; }
    
    public int? RoleAssignmentId { get; set; }
    
    [ForeignKey(nameof(RoleAssignmentId))]
    public SqlServerRoleAssignment? RoleAssignment { get; set; }
    
    // Audit fields
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    [MaxLength(100)]
    public string CreatedBy { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string RequestId { get; set; } = string.Empty; // Link to original request
}