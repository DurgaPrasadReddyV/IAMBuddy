namespace IAMBuddy.ProvisioningMCPServer.Models.Requests
{
    public class CreateSqlLoginRequest
    {
        public string LoginName { get; set; } = string.Empty;
        public string ServerName { get; set; } = string.Empty;
        public string? InstanceName { get; set; }
        public LoginType LoginType { get; set; }
        public string? Password { get; set; }
        public string? DefaultDatabase { get; set; }
        public string? DefaultLanguage { get; set; }
        public bool IsPasswordPolicyEnforced { get; set; } = true;
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
    }
}
