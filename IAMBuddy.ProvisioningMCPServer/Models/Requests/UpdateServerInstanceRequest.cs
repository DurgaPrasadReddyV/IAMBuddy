namespace IAMBuddy.ProvisioningMCPServer.Models.Requests
{
    public class UpdateServerInstanceRequest
    {
        public string ConnectionString { get; set; } = string.Empty;
        public int Port { get; set; } = 1433;
        public bool IsActive { get; set; } = true;
        public string? Description { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
