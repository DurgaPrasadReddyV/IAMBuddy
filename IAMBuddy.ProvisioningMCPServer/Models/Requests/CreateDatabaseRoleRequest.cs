namespace IAMBuddy.ProvisioningMCPServer.Models.Requests
{
    public class CreateDatabaseRoleRequest
    {
        public string RoleName { get; set; } = string.Empty;
        public string ServerName { get; set; } = string.Empty;
        public string? InstanceName { get; set; }
        public string DatabaseName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
    }
}
