using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IAMBuddy.SqlServerManagementService.Models;

public class SqlServerRoleAssignment
{
    [Key]
    public int Id { get; set; }
    
    // Foreign keys
    public int RoleId { get; set; }
    
    [ForeignKey(nameof(RoleId))]
    public SqlServerRole Role { get; set; } = null!;
    
    // Can be assigned to either a Login (server role) or User (database role)
    public int? LoginId { get; set; }
    
    [ForeignKey(nameof(LoginId))]
    public SqlServerLogin? Login { get; set; }
    
    public int? UserId { get; set; }
    
    [ForeignKey(nameof(UserId))]
    public SqlServerUser? User { get; set; }
    
    [Required]
    [MaxLength(128)]
    public string ServerInstance { get; set; } = string.Empty;
    
    [MaxLength(128)]
    public string? DatabaseName { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime? EffectiveDate { get; set; }
    
    public DateTime? ExpiryDate { get; set; }
    
    [MaxLength(500)]
    public string? AssignmentReason { get; set; }
    
    // Audit fields
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
    
    [MaxLength(100)]
    public string CreatedBy { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string ModifiedBy { get; set; } = string.Empty;
    
    // Navigation properties
    public ICollection<SqlServerOperation> Operations { get; set; } = new List<SqlServerOperation>();
}