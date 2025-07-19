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
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // Get connection string from appsettings.json
                var connectionString = context.Configuration.GetConnectionString("DefaultConnection");

                services.AddDbContext<ToolsDbContext>(options =>
                    options.UseNpgsql(connectionString,
                        b => b.MigrationsAssembly(typeof(ToolsDbContext).Assembly.FullName)));

                services.AddTransient<DataSeeder>(); // Register the seeder
            })
            .Build();

        // Apply migrations and seed data on startup (for development/testing)
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<ToolsDbContext>();
            var seeder = services.GetRequiredService<DataSeeder>();

            await context.Database.MigrateAsync(); // Apply pending migrations

            // Seed dummy data with default counts
            await seeder.SeedAllDummyData(new DataSeeder.SeedConfiguration());

            // Seed default SQL Server specific values
            await seeder.SeedDefaultSqlServerValues();
        }

        await host.RunAsync();
    }
}
