namespace IAMBuddy.ProvisioningMCPServer.Models.Requests
{
    public class RegisterServerInstanceRequest
    {
        public string ServerName { get; set; } = string.Empty;
        public string? InstanceName { get; set; }
        public string ConnectionString { get; set; } = string.Empty;
        public int Port { get; set; } = 1433;
        public bool IsAvailabilityGroupListener { get; set; } = false;
        public string? AvailabilityGroupName { get; set; }
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
    }
}
