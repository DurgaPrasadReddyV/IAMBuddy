
using IAMBuddy.RequestIntakeService.Services;

using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
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
        });

        builder.Services.AddTemporalClient(opts =>
        {
            opts.TargetHost = builder.Configuration["ConnectionStrings:temporal"];
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
