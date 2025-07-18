using ModelContextProtocol.Client;
using System.Text.RegularExpressions;

namespace IAMBuddy.MCPClientsService.MCPClients
{
    public class RequestIntakeMCPClient: IMCPClientBuilder
    {
        private readonly IMcpClient _client;
        public RequestIntakeMCPClient()
        {
            _client = McpClientFactory.CreateAsync(
                new SseClientTransport(new SseClientTransportOptions
                {
                    Endpoint = new Uri("http://localhost:8014/sse"),
                    Name = Constants.RequestIntakeMCPClientName,
                    //UseStreamableHttp = true
                })).Result;
        }
        public string Name => Constants.RequestIntakeMCPClientName;
        public async ValueTask<IList<McpClientTool>> GetTools() =>
            [.. (await _client.ListToolsAsync()).Select(tool => tool.WithName(Regex.Replace(tool.Name, "[^a-zA-Z0-9]", "")))];
    }
}
