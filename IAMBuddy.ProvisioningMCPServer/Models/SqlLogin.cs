using System.ComponentModel.DataAnnotations;

namespace IAMBuddy.ProvisioningMCPServer.Models
{
    public enum LoginType
    {
        SqlLogin,
        WindowsLogin,
        ActiveDirectoryLogin
    }
    
    public class SqlLogin : BaseEntity
    {
        [Required]
        [MaxLength(128)]
        public string LoginName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(255)]
        public string ServerName { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string? InstanceName { get; set; }
        
        public LoginType LoginType { get; set; }
        
        [MaxLength(255)]
        public string? DefaultDatabase { get; set; }
        
        [MaxLength(255)]
        public string? DefaultLanguage { get; set; }
        
        public bool IsEnabled { get; set; } = true;
        
        public bool IsPasswordExpired { get; set; } = false;
        
        public bool IsMustChangePassword { get; set; } = false;
        
        public bool IsPasswordPolicyEnforced { get; set; } = true;
        
        public DateTime? LastLoginTime { get; set; }
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        // Navigation properties
        public ICollection<ServerRoleAssignment> ServerRoleAssignments { get; set; } = new List<ServerRoleAssignment>();
        public ICollection<DatabaseUser> DatabaseUsers { get; set; } = new List<DatabaseUser>();
    }
}