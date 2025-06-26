namespace IAMBuddy.MSSQLAccountManager.Models
{
    public enum OperationType
    {
        Create,
        Update,
        Delete,
        Get,
        Assign,
        Remove
    }
    
    public enum OperationStatus
    {
        Success,
        Failed,
        PartialSuccess,
        InProgress,
        Cancelled
    }
    
    public class OperationResult : BaseEntity
    {
        public OperationType OperationType { get; set; }
        
        public OperationStatus Status { get; set; }
        
        public string ResourceType { get; set; } = string.Empty;
        
        public string ResourceName { get; set; } = string.Empty;
        
        public string ServerName { get; set; } = string.Empty;
        
        public string? DatabaseName { get; set; }
        
        public string? ErrorMessage { get; set; }
        
        public string? Details { get; set; }
        
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        
        public DateTime? EndTime { get; set; }
        
        public int Duration => EndTime.HasValue ? (int)(EndTime.Value - StartTime).TotalMilliseconds : 0;
    }
}