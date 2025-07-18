using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.Configuration;
using A2A.Server.Infrastructure.Services;
using A2A.Server;
using A2A.Server.Infrastructure;
using IAMBuddy.MCPClientsService.MCPClients;

namespace IAMBuddy.MCPClientsService
{
    public static class Startup
    {
        public static void ConfigureService(this IServiceCollection services, IConfiguration configuration)
        {
            // Add Google Gemini AI service
            services.AddGoogleAIGeminiChatCompletion("gemini-2.5-flash", configuration["GEMINI_KEY"]!);
            // Add MCP Tools
            services.AddSingleton<IMCPClientBuilder, ApprovalMCPClient>();
            services.AddSingleton<IMCPClientBuilder, ProvisioningMCPClient>();
            services.AddSingleton<IMCPClientBuilder, RequestIntakeMCPClient>();
            services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Trace));
            // Add Semantic Kernel services
            services.AddSingleton(sp =>
            {
                Kernel kernel = new(sp, []);

                var allMCPs = sp.GetServices<IMCPClientBuilder>().ToArray();


                foreach (var mcp in allMCPs)
                {
                    var tools = mcp.GetTools().Result;

                    kernel.Plugins.AddFromFunctions(mcp.Name, tools.Select(tool =>
                    {
                        return tool.AsKernelFunction();
                    }));

                }

                return kernel;
            });

            services.AddDistributedMemoryCache();
            services.AddSingleton<IAgentRuntime, AgentRuntime>();
            services.AddA2AProtocolServer(builder =>
            {
                builder
                    .UseAgentRuntime(provider => provider.GetRequiredService<IAgentRuntime>())
                    .UseDistributedCacheTaskRepository()
                    .SupportsStreaming();
            });
        }


    }


}
