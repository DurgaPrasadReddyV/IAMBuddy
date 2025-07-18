using ModelContextProtocol.Client;
using System.Text.RegularExpressions;

namespace IAMBuddy.MCPClientsService.MCPClients
{
    public class ApprovalMCPClient : IMCPClientBuilder
    {
        private readonly IMcpClient _client;
        public ApprovalMCPClient()
        {
            _client = McpClientFactory.CreateAsync(new SseClientTransport(new SseClientTransportOptions
            {
                Endpoint = new("http://localhost:8011/sse"),
                Name = Constants.ApprovalMCPClientName,
                //UseStreamableHttp = true
            })).Result;
        }

        public string Name => Constants.ApprovalMCPClientName;

        async ValueTask<IList<McpClientTool>> IMCPClientBuilder.GetTools() =>
            [.. (await _client.ListToolsAsync()).Select(tool => tool.WithName(Regex.Replace(tool.Name, "[^a-zA-Z0-9]", "")))];
    }
}
