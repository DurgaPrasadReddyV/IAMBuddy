using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using System.Net.Http;

namespace IAMBuddy.WebUI.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services.AddHttpClient();
            builder.Services.AddMudServices();
            // Register HttpClient for API calls
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7175/api") });
            await builder.Build().RunAsync();
        }
    }
}
