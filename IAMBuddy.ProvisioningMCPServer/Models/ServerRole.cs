using System.ComponentModel.DataAnnotations;

namespace IAMBuddy.ProvisioningMCPServer.Models
{
    public class ServerRole : BaseEntity
    {
        [Required]
        [MaxLength(128)]
        public string RoleName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(255)]
        public string ServerName { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string? InstanceName { get; set; }
        
        public bool IsFixedRole { get; set; } = false;
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        // Navigation properties
        public ICollection<ServerRoleAssignment> ServerRoleAssignments { get; set; } = new List<ServerRoleAssignment>();
    }
}