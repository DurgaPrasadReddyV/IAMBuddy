using System.ComponentModel.DataAnnotations;

namespace IAMBuddy.Shared.Models
{
    public class MSSQLAccountRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string ServerName { get; set; } = string.Empty;

        [Required]
        public string DatabaseName { get; set; } = string.Empty;

        [Required]
        public string ServerAccountName { get; set; } = string.Empty;

        [Required]
        public string DatabaseAccountName { get; set; } = string.Empty;

        [Required]
        public string RequestorEmail { get; set; } = string.Empty;

        public AccountRequestStatus Status { get; set; } = AccountRequestStatus.Initiated;

        public DateTime RequestedDate { get; set; } = DateTime.UtcNow;
    }
}
