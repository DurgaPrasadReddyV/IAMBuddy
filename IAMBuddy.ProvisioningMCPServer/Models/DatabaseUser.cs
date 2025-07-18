using System.ComponentModel.DataAnnotations;

namespace IAMBuddy.ProvisioningMCPServer.Models
{
    public enum UserType
    {
        SqlUser,
        WindowsUser,
        ContainedUser,
        ExternalUser
    }
    
    public class DatabaseUser : BaseEntity
    {
        [Required]
        [MaxLength(128)]
        public string UserName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(255)]
        public string ServerName { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string? InstanceName { get; set; }
        
        [Required]
        [MaxLength(128)]
        public string DatabaseName { get; set; } = string.Empty;
        
        public UserType UserType { get; set; }
        
        public Guid? SqlLoginId { get; set; }
        
        [MaxLength(128)]
        public string? LoginName { get; set; }
        
        [MaxLength(128)]
        public string? DefaultSchema { get; set; } = "dbo";
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        // Navigation properties
        public SqlLogin? SqlLogin { get; set; }
        public ICollection<DatabaseRoleAssignment> DatabaseRoleAssignments { get; set; } = new List<DatabaseRoleAssignment>();
    }
}