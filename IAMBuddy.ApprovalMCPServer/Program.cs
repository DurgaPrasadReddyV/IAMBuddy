
using IAMBuddy.ApprovalMCPServer.Tools;
using IAMBuddy.ApprovalMCPServer.Tools.Notifications;

namespace IAMBuddy.ApprovalMCPServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddMcpServer()
                .WithHttpTransport()
                .WithTools<ScientificMathTool>()
                .WithTools<ProvisioningApprovalNotificationTool>()
                .WithTools<EchoTool>();

            var app = builder.Build();

            app.MapMcp();

            app.Run();

        }
    }
}
