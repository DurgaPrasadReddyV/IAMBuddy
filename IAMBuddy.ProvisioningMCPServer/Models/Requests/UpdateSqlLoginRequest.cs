namespace IAMBuddy.ProvisioningMCPServer.Models.Requests
{
    public class UpdateSqlLoginRequest
    {
        public string? DefaultDatabase { get; set; }
        public string? DefaultLanguage { get; set; }
        public string? Description { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
