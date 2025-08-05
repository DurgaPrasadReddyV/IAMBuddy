using IAMBuddy.ApprovalMCPServer.Models;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace IAMBuddy.ApprovalMCPServer.Tools.Notifications
{
    [McpServerToolType]
    public class ProvisioningApprovalNotificationTool
    {
        [McpServerTool, Description("Send provisioning approval notification")]
        public static string Send(EmailRequest request)
        {
            return "notification sent successfully";
        }
    }
}
