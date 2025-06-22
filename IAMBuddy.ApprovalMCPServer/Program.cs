
using IAMBuddy.ApprovalMCPServer.Tools;

namespace IAMBuddy.ApprovalMCPServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddMcpServer()
                .WithHttpTransport()
                .WithTools<MSSQLProvisioningTool>();

            var app = builder.Build();

            app.MapMcp();

            app.Run();

        }
    }
}
