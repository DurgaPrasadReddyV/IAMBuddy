namespace IAMBuddy.Tools.DatabaseGenerator;
using System;
using IAMBuddy.DatabaseGenerator;
using IAMBuddy.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static async Task Main(string[] args)
    {
        var hostBuilder = Host.CreateApplicationBuilder(args);
        hostBuilder.AddNpgsqlDbContext<IAMBuddyDbContext>("IAMBuddyDb", null, x => { x.EnableSensitiveDataLogging(); x.EnableDetailedErrors(); });
        hostBuilder.Services.AddTransient<DataSeeder>(); // Register the DataSeeder service

        var host = hostBuilder.Build();

        // Apply migrations and seed data on startup (for development/testing)
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<IAMBuddyDbContext>();
            var seeder = services.GetRequiredService<DataSeeder>();
            await context.Database.EnsureCreatedAsync(); // Create Tables

            // Seed dummy data with default counts
            await seeder.SeedAllDummyData(new DataSeeder.SeedConfiguration());

            // Seed default SQL Server specific values
            await seeder.SeedDefaultSqlServerValues();
        }

        Console.WriteLine("All database operations completed successfully. Application will now exit.");
    }
}
