using IAMBuddy.Orchestrator.API.Models;

namespace IAMBuddy.Orchestrator.API.Services
{
    public class ApprovalCheckerService : BackgroundService
    {
        private readonly HttpClient _httpClient;

        public ApprovalCheckerService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var response = await _httpClient.GetAsync("https://localhost:7175/api/ApprovalStatus/1");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadFromJsonAsync<RequestModel>();
                    if (content.Status == "Approved")
                    {
                        Console.WriteLine("✅ Tool Approved!");
                        break; // Stop polling
                    }
                    else
                    {
                        Console.WriteLine("⏳ Waiting for approval...");
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); // Poll every 5 seconds
            }
        }
    }
}
