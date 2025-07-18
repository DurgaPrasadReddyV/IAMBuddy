using ModelContextProtocol.Client;
using System.Text.RegularExpressions;

namespace IAMBuddy.MCPClientsService.MCPClients
{
    public class ProvisioningMCPClient : IMCPClientBuilder
    {
        private readonly IMcpClient _client;
        public ProvisioningMCPClient()
        {
            _client = McpClientFactory.CreateAsync(new SseClientTransport(new SseClientTransportOptions
            {
                Endpoint = new Uri("http://localhost:8013/sse"),
                Name = Constants.ProvisioningMCPClientName,
                //UseStreamableHttp = true
            })).Result;
        }
        public string Name => Constants.ProvisioningMCPClientName;
        public async ValueTask<IList<McpClientTool>> GetTools() =>
            [.. (await _client.ListToolsAsync()).Select(tool => tool.WithName(Regex.Replace(tool.Name, "[^a-zA-Z0-9]", "")))];
    }
}
