namespace IAMBuddy.ProvisioningMCPServer.Models.Requests
{
    public class CreateServerRoleRequest
    {
        public string RoleName { get; set; } = string.Empty;
        public string ServerName { get; set; } = string.Empty;
        public string? InstanceName { get; set; }
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
    }
}
