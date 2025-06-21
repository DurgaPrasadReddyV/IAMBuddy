namespace IAMBuddy.Shared.Models
{
    public class MSSQLApprovalRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid AccountRequestId { get; set; }
        public string ApproverEmail { get; set; } = string.Empty;
        public string RequestorEmail { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string ServerName { get; set; } = string.Empty;
        public string BusinessJustification { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new List<string>();
        public DateTime RequestedDate { get; set; }
        public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;
        public string? Comments { get; set; }
        public DateTime? ResponseDate { get; set; }
        public int ReminderCount { get; set; } = 0;
    }
    
    public enum ApprovalStatus
    {
        Pending,
        Approved,
        Rejected,
        Expired
    }
}
