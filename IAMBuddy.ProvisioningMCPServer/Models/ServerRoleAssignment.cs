using System.ComponentModel.DataAnnotations;

namespace IAMBuddy.ProvisioningMCPServer.Models
{
    public class ServerRoleAssignment : BaseEntity
    {
        [Required]
        public Guid SqlLoginId { get; set; }
        
        [Required]
        public Guid ServerRoleId { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string ServerName { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string? InstanceName { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime? RevokedAt { get; set; }
        
        [MaxLength(255)]
        public string? RevokedBy { get; set; }
        
        [MaxLength(500)]
        public string? RevokeReason { get; set; }
        
        // Navigation properties
        public SqlLogin SqlLogin { get; set; } = null!;
        public ServerRole ServerRole { get; set; } = null!;
    }
}