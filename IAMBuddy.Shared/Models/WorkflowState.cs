namespace IAMBuddy.Shared.Models
{
    public class WorkflowState
    {
        public Guid AccountRequestId { get; set; }
        public string CurrentStage { get; set; } = string.Empty;
        public WorkflowStatus Status { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
        public List<string> CompletedStages { get; set; } = new List<string>();
        public string? ErrorMessage { get; set; }
    }
}
