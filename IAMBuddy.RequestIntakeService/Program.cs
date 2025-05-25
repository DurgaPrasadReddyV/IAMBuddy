
using IAMBuddy.RequestIntakeService.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Diagnostics.Metrics;
using Temporalio.Extensions.DiagnosticSource;
using Temporalio.Extensions.OpenTelemetry;
using Temporalio.Runtime;

namespace IAMBuddy.RequestIntakeService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        using var meter = new Meter("Temporal.Client");

        var runtime = new TemporalRuntime(new()
        {
            Telemetry = new()
            {
                Metrics = new() { CustomMetricMeter = new CustomMetricMeter(meter) },
            },
        });

        builder.AddServiceDefaults();

        builder.Services.AddTemporalClient(opts =>
        {
            opts.TargetHost = builder.Configuration["ConnectionStrings:temporal"];
            opts.Interceptors = [new TracingInterceptor()];
            opts.Runtime = runtime;
        });

        // Add services to the container.
        builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("RequestIntakeServiceDb"));

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddScoped<RequestValidationService>();
        builder.Services.AddScoped<TemporalClientService>();
        var app = builder.Build();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.EnsureCreated();
        }
        app.Run();
    }
}
