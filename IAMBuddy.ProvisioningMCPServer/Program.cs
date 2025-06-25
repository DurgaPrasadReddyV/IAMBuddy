using IAMBuddy.ProvisioningMCPServer.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IAMBuddy.ProvisioningMCPServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddMcpServer()
                .WithHttpTransport()
                .WithTools<MathTool>()
                .WithTools<WeatherTool>();

            var app = builder.Build();

            app.MapMcp();

            app.Run();

        }
    }
}
