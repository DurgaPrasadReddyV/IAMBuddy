namespace IAMBuddy.Tools;

using IAMBuddy.Infrastructure;
using IAMBuddy.Tools.OpenApi;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        builder.Services.AddMcpServer()
                .WithHttpTransport(o => o.Stateless = true)
                .WithToolsFromAssembly();

        builder.AddNpgsqlDbContext<IAMBuddyDbContext>("IAMBuddyDb", null, x => { x.EnableSensitiveDataLogging(); x.EnableDetailedErrors(); });

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddOpenApi("openapi", o => o.AddDocumentTransformer<McpDocumentTransformer>());

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        //app.UseHttpsRedirection();

        app.MapOpenApi("/{documentName}.json");

        app.MapMcp("/mcp");

        app.Run();
    }
}
