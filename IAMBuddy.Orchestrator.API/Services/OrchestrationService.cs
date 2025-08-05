

using A2A.Client;
using A2A.Client.Configuration;
using A2A.Client.Services;
using A2A.Models;
using IAMBuddy.Orchestrator;
using IAMBuddy.Orchestrator.API.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;

namespace IAMBuddy.Orchestrator.API.Services;

public class OrchestratorService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly Kernel _kernel;
    private readonly ChatCompletionAgent _chatCompletionAgent;

    public OrchestratorService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;

        var builder = Kernel.CreateBuilder()
            .AddGoogleAIGeminiChatCompletion("gemini-2.5-flash", _configuration["GEMINI_KEY"]!);

        builder.Services.AddHttpClient();
        _kernel = builder.Build();

        var agents = DiscoverAgents().GetAwaiter().GetResult();
        foreach (var agentData in agents)
        {
            _kernel.ImportPluginFromFunctions(agentData.AgentName, [
                .. agentData.AgentCard.Skills.Select(skill => {
                    return _kernel.CreateFunctionFromMethod(async (string prompt) => {
                        var client = new A2AProtocolHttpClient(
                            Options.Create(new A2AProtocolClientOptions { Endpoint = agentData.URL }),
                            _httpClientFactory.CreateClient(agentData.Name));
                        var resp = await client.SendTaskAsync(new A2A.Requests.SendTaskRequest {
                            Params = new() {
                                Id = Guid.NewGuid().ToString(),
                                SessionId = Guid.NewGuid().ToString(),
                                Message = new Message {
                                    Role = A2A.MessageRole.User,
                                    Parts = [new TextPart(prompt)]
                                }
                            }
                        }, default).ConfigureAwait(false);
                        return resp.Result!.Artifacts!.Last().Parts!.OfType<TextPart>().Last().Text;
                    }, skill.Name, skill.Description, returnParameter: new() {
                        Description = "Prompt response as a JSON object or array to be inferred upon.",
                        ParameterType = typeof(string)
                    });
                })
            ]);
        }

        _chatCompletionAgent = new ChatCompletionAgent
        {
            Name = "OrchestratorAgent",
            Instructions = "You are an helpful agent that will use its tools to answer user queries",
            Kernel = _kernel,
            Arguments = new KernelArguments(new GeminiPromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                ToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions,
            })
        };
    }

    private async Task<List<AgentData>> DiscoverAgents()
    {
        var agents = new List<AgentData>();
        var servers = new[] { "http://localhost:5151", "http://localhost:5191", "http://localhost:5243" };
        foreach (var serverUrl in servers)
        {
            var server = new Uri(serverUrl);
            using var httpClient = _httpClientFactory.CreateClient("A2AProtocolDiscoverer");
            var discoveryDocument = await httpClient.GetA2ADiscoveryDocumentAsync(server);
            var agent = discoveryDocument.Agents[0];
            agents.Add(new AgentData
            {
                AgentCard = discoveryDocument.Agents[0],
                AgentName = agent.Name.Replace(" ", string.Empty),
                URL = agent.Url.IsAbsoluteUri ? agent.Url : new(server, agent.Url),
                Name = agent.Name
            });
        }
        return agents;
    }

    public async Task<AgentResponse> GetAgentResponseAsync(string input)
    {
        var agentThread = new ChatHistoryAgentThread();
        var message = new ChatMessageContent(AuthorRole.User, input);
        await foreach (var agentResponse in _chatCompletionAgent.InvokeAsync(message, agentThread))
        {
            var response=new AgentResponse
            {
                Message = agentResponse.Message?.Content,
                PromptTokenCount = Convert.ToInt32(agentResponse.Message?.Metadata?["PromptTokenCount"]),
                CurrentCandidateTokenCount = Convert.ToInt32(agentResponse.Message?.Metadata?["CurrentCandidateTokenCount"]),
                CandidatesTokenCount = Convert.ToInt32(agentResponse.Message?.Metadata?["CandidatesTokenCount"]),
                TotalTokenCount = Convert.ToInt32(agentResponse.Message?.Metadata?["TotalTokenCount"])
            };           
            return response;
        }
        return new AgentResponse();
    }
}