using System.ComponentModel.DataAnnotations;

namespace IAMBuddy.MSSQLAccountManager.Models
{
    public abstract class BaseEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string CreatedBy { get; set; } = string.Empty;
        
        [MaxLength(255)]
        public string? UpdatedBy { get; set; }
    }
}