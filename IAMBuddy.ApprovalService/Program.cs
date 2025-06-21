
using Scalar.AspNetCore;

namespace IAMBuddy.ApprovalService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        // Add services to the container.

        builder.Services.AddControllers();
        // Configure OpenAPI
        builder.Services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, _) =>
            {
                document.Info = new()
                {
                    Title = "Approval API",
                    Version = "v1",
                };
                return Task.CompletedTask;
            });
        });

        var app = builder.Build();

        app.MapDefaultEndpoints();

        // Enable OpenAPI and Scalar
        app.MapOpenApi().CacheOutput();
        app.MapScalarApiReference();

        // Redirect root to Scalar UI
        app.MapGet("/", () => Results.Redirect("/scalar/v1"))
           .ExcludeFromDescription();

        app.Run();
    }
}
