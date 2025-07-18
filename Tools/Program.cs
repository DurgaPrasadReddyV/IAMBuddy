namespace IAMBuddy.Tools;

using IAMBuddy.Tools.OpenApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        builder.Services.AddMcpServer()
                .WithHttpTransport(o => o.Stateless = true)
                .WithToolsFromAssembly();

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddOpenApi("openapi", o => o.AddDocumentTransformer<McpDocumentTransformer>());

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseHttpsRedirection();

        app.MapOpenApi("/{documentName}.json");

        app.MapMcp("/mcp");

        app.Run();
    }
}
