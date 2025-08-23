namespace Agents;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        // Add services to the container.
        builder.Services.AddAuthorization();


        var app = builder.Build();


        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.

        app.UseHttpsRedirection();

        app.Run();
    }
}
