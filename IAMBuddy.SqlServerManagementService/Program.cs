using IAMBuddy.SqlServerManagementService.Services;
using IAMBuddy.SqlServerManagementService.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using Temporalio.Extensions.DiagnosticSource;
using Temporalio.Extensions.OpenTelemetry;
using Temporalio.Runtime;

namespace IAMBuddy.SqlServerManagementService;

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
        builder.Services.AddDbContext<SqlServerManagementDbContext>(options =>
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                // Fallback connection string for development
                connectionString = "Server=(localdb)\\mssqllocaldb;Database=SqlServerManagementDb;Trusted_Connection=true;MultipleActiveResultSets=true";
            }
            options.UseSqlServer(connectionString)
                   .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
                   .EnableDetailedErrors(builder.Environment.IsDevelopment())
                   .LogTo(message => System.Diagnostics.Debug.WriteLine(message), LogLevel.Information);
        });

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        
        // Add CORS
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });
        
        // Register services
        builder.Services.AddScoped<ISqlServerManagementService, SqlServerManagementService.Services.SqlServerManagementService>();
        builder.Services.AddScoped<ISqlServerOperationService, SqlServerOperationService>();
        
        // Register connection service
        builder.Services.AddScoped<ISqlServerConnectionService, SqlServerConnectionService>();
        
        // Register business logic services
        builder.Services.AddScoped<ISqlLoginService, SqlLoginService>();
        builder.Services.AddScoped<IServerRoleService, ServerRoleService>();
        builder.Services.AddScoped<IDatabaseUserService, DatabaseUserService>();
        builder.Services.AddScoped<IDatabaseRoleService, DatabaseRoleService>();
        
        // Register repositories
        builder.Services.AddScoped<ISqlLoginRepository, SqlLoginRepository>();
        builder.Services.AddScoped<IServerRoleRepository, ServerRoleRepository>();
        builder.Services.AddScoped<IDatabaseUserRepository, DatabaseUserRepository>();
        builder.Services.AddScoped<IDatabaseRoleRepository, DatabaseRoleRepository>();
        
        var app = builder.Build();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseCors();
        app.UseAuthorization();
        app.MapControllers();

        // Initialize database with migrations
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<SqlServerManagementDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            
            try
            {
                context.Database.Migrate();
                logger.LogInformation("Database migration completed successfully for SqlServerManagementService");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating the SqlServerManagementService database");
                throw;
            }
        }

        app.Run();
    }
}