using System.ComponentModel.DataAnnotations;

namespace IAMBuddy.MSSQLAccountManager.Models
{
    public class DatabaseRole : BaseEntity
    {
        [Required]
        [MaxLength(128)]
        public string RoleName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(255)]
        public string ServerName { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string? InstanceName { get; set; }
        
        [Required]
        [MaxLength(128)]
        public string DatabaseName { get; set; } = string.Empty;
        
        public bool IsFixedRole { get; set; } = false;
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        // Navigation properties
        public ICollection<DatabaseRoleAssignment> DatabaseRoleAssignments { get; set; } = new List<DatabaseRoleAssignment>();
    }
}