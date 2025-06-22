using IAMBuddy.Shared.Workflows;
using Microsoft.VisualBasic;
using System.Diagnostics.Metrics;
using Temporalio.Runtime;
using Temporalio.Activities;
using Temporalio.Extensions.Hosting;
using Temporalio.Workflows;

namespace IAMBuddy.TemporalWorker;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        using var meter = new Meter("Temporal.Client");

        var runtime = new TemporalRuntime(new()
        {
            Telemetry = new()
            {
            },
        });

        builder.Services
            .AddTemporalClient(opts =>
            {
                opts.TargetHost = builder.Configuration.GetConnectionString("temporal-server");
                opts.Runtime = runtime;
            })
            .AddHostedTemporalWorker("account-provisioning")
            .AddScopedActivities<MSSQLAccountProvisioningWorkflowActivities>()
            .AddWorkflow<MSSQLAccountProvisioningWorkflow>();

        var host = builder.Build();
        host.Run();
    }
}