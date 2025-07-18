namespace IAMBuddy.ProvisioningMCPServer.Models.Requests
{
    public class CreateDatabaseUserRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string ServerName { get; set; } = string.Empty;
        public string? InstanceName { get; set; }
        public string DatabaseName { get; set; } = string.Empty;
        public UserType UserType { get; set; }
        public Guid? SqlLoginId { get; set; }
        public string? LoginName { get; set; }
        public string? DefaultSchema { get; set; } = "dbo";
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
    }
}
