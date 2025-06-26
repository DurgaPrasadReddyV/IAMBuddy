using System.ComponentModel.DataAnnotations;

namespace IAMBuddy.MSSQLAccountManager.Models
{
    public class ServerInstance : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        public string ServerName { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string? InstanceName { get; set; }
        
        [Required]
        [MaxLength(500)]
        public string ConnectionString { get; set; } = string.Empty;
        
        public int Port { get; set; } = 1433;
        
        public bool IsAvailabilityGroupListener { get; set; } = false;
        
        [MaxLength(128)]
        public string? AvailabilityGroupName { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime? LastHealthCheck { get; set; }
        
        [MaxLength(500)]
        public string? HealthStatus { get; set; }
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        // Related instances for AG scenarios
        public ICollection<ServerInstance> RelatedInstances { get; set; } = new List<ServerInstance>();
    }
}