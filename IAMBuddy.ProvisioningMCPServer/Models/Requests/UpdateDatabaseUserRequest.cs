namespace IAMBuddy.ProvisioningMCPServer.Models.Requests
{
    public class UpdateDatabaseUserRequest
    {
        public string? DefaultSchema { get; set; }
        public string? Description { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
