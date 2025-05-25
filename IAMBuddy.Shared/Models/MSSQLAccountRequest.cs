using System.ComponentModel.DataAnnotations;

namespace IAMBuddy.Shared.Models
{
    public class MSSQLAccountRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        public string DatabaseName { get; set; } = string.Empty;
        
        [Required]
        public string ServerName { get; set; } = string.Empty;
        
        [Required]
        public string RequestorEmail { get; set; } = string.Empty;
        
        [Required]
        public string BusinessJustification { get; set; } = string.Empty;

        public DateTime RequestedDate { get; set; } = DateTime.UtcNow;
        
        public WorkflowStatus Status { get; set; } = WorkflowStatus.Pending;
        
        public string? ApproverEmail { get; set; }
        
        public string? WorkflowId { get; set; }
    }
}
