using System.ComponentModel.DataAnnotations;

namespace IAMBuddy.ProvisioningMCPServer.Models
{
    public class DatabaseRoleAssignment : BaseEntity
    {
        [Required]
        public Guid DatabaseUserId { get; set; }
        
        [Required]
        public Guid DatabaseRoleId { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string ServerName { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string? InstanceName { get; set; }
        
        [Required]
        [MaxLength(128)]
        public string DatabaseName { get; set; } = string.Empty;
        
        public bool IsActive { get; set; } = true;
        
        public DateTime? RevokedAt { get; set; }
        
        [MaxLength(255)]
        public string? RevokedBy { get; set; }
        
        [MaxLength(500)]
        public string? RevokeReason { get; set; }
        
        // Navigation properties
        public DatabaseUser DatabaseUser { get; set; } = null!;
        public DatabaseRole DatabaseRole { get; set; } = null!;
    }
}