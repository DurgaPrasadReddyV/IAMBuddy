namespace IAMBuddy.Orchestrator.API.Models
{
    public class AgentResponse
    {
        public string Message { get; set; }
        public int PromptTokenCount { get; set; }
        public int CurrentCandidateTokenCount { get; set; }
        public int CandidatesTokenCount { get; set; }
        public int TotalTokenCount { get; set; }
    }
}
