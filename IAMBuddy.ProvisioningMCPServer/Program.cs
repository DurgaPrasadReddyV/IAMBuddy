using IAMBuddy.ProvisioningMCPServer.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics.Metrics;
using System.Reflection;

namespace IAMBuddy.ProvisioningMCPServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.AddOpenTelemetry(logging =>
            {
                logging.IncludeFormattedMessage = true;
                logging.IncludeScopes = true;
            });

            builder.Services.AddOpenTelemetry()
                .WithTracing(tracerProviderBuilder =>
                    tracerProviderBuilder.AddSource("*")
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation())
                .WithMetrics(metricsProviderBuilder =>
                    metricsProviderBuilder.AddMeter("*")
                    .AddRuntimeInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation())
                .UseOtlpExporter();

            builder.Services.AddMcpServer()
                .WithHttpTransport()
                .WithTools<MathTool>()
                .WithTools<MSSQLTool>()
                .WithTools<WeatherTool>();

            var app = builder.Build();

            app.MapMcp();

            app.Run();

        }
    }
}
