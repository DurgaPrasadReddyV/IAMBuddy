using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql; // Add this using directive

public class Program
{
    public static async Task Main(string[] args)
    {
        var hostBuilder = Host.CreateApplicationBuilder(args);
        hostBuilder.AddNpgsqlDbContext<ToolsDbContext>("ToolsDb");
        hostBuilder.Services.AddTransient<DataSeeder>(); // Register the DataSeeder service

        var host = hostBuilder.Build();

        // Apply migrations and seed data on startup (for development/testing)
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<ToolsDbContext>();
            var seeder = services.GetRequiredService<DataSeeder>();

            await context.Database.EnsureCreatedAsync(); // Apply pending migrations

            // Seed dummy data with default counts
            await seeder.SeedAllDummyData(new DataSeeder.SeedConfiguration());

            // Seed default SQL Server specific values
            await seeder.SeedDefaultSqlServerValues();
        }

        Console.WriteLine("All database operations completed successfully. Application will now exit.");
    }
}
