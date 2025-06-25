#pragma warning disable SKEXP0070
#pragma warning disable SKEXP0001
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using ModelContextProtocol.Client;

namespace IAMBuddy.ConsoleApp
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var kernelBuilder = Kernel.CreateBuilder()
            .AddGoogleAIGeminiChatCompletion(modelId: "gemini-2.5-flash", apiKey: "Provide Gemini API Key");

            var approvalMCPClient = await McpClientFactory.CreateAsync(
                new SseClientTransport(new SseClientTransportOptions()
                {
                    Endpoint = new Uri("http://localhost:8011/sse")
                }),
                new McpClientOptions()
                {
                    ClientInfo = new() { Name = "ApprovalMCPClient", Version = "1.0.0" }
                });

            var approvalTools = await approvalMCPClient.ListToolsAsync();

            // Print approval mcp tools
            foreach (var tool in approvalTools)
            {
                Console.WriteLine($" - {tool.Name}: {tool.Description}");
            }

            kernelBuilder.Plugins.AddFromFunctions(
                "approvalTools", approvalTools.Select(aiFunc => aiFunc.AsKernelFunction()));

            var provisioningMCPClient = await McpClientFactory.CreateAsync(
                new SseClientTransport(new SseClientTransportOptions()
                {
                    Endpoint = new Uri("http://localhost:8013/sse")
                }),
                new McpClientOptions()
                {
                    ClientInfo = new() { Name = "ProvisioningMCPClient", Version = "1.0.0" }
                });

            var provisioningTools = await provisioningMCPClient.ListToolsAsync();

            // Print provisioning mcp tools
            foreach (var tool in provisioningTools)
            {
                Console.WriteLine($" - {tool.Name}: {tool.Description}");
            }

            kernelBuilder.Plugins.AddFromFunctions(
                "provisioningTools", provisioningTools.Select(aiFunc => aiFunc.AsKernelFunction()));

            var requestIntakeMCPClient = await McpClientFactory.CreateAsync(
                new SseClientTransport(new SseClientTransportOptions()
                {
                    Endpoint = new Uri("http://localhost:8014/sse")
                }),
                new McpClientOptions()
                {
                    ClientInfo = new() { Name = "RequestIntakeMCPClient", Version = "1.0.0" }
                });

            var requestIntakeTools = await requestIntakeMCPClient.ListToolsAsync();

            // Print provisioning mcp tools
            foreach (var tool in requestIntakeTools)
            {
                Console.WriteLine($" - {tool.Name}: {tool.Description}");
            }

            kernelBuilder.Plugins.AddFromFunctions(
                "requestIntakeTools", requestIntakeTools.Select(aiFunc => aiFunc.AsKernelFunction()));

            var kernel = kernelBuilder.Build();

            var executionSettings = new GeminiPromptExecutionSettings
            {
                Temperature = 0,
                ToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions,
            };

            // Create chat history
            ChatHistory history = [];
            history.AddSystemMessage(@"You are a helpful assistant.
            You are restricted to using only the provided plugins.
            Please explain your reasoning with the response.");

            // Get chat completion service
            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

            const string prompt = "What's the weather in Huelva?";
            Console.WriteLine(prompt);
            var weatherInvokeResponse = await kernel.InvokePromptAsync(prompt, new KernelArguments(executionSettings));
            Console.WriteLine(weatherInvokeResponse);

            // Start the conversation
            while (true)
            {
                // Get user input
                Console.Write("User > ");
                var userMessage = Console.ReadLine()!;
                if (userMessage == "exit" || userMessage == "quit") break;
                if (userMessage == "") continue;
                history.AddUserMessage(userMessage);

                try
                {
                    // Get the response from the AI
                    var result = chatCompletionService.GetStreamingChatMessageContentsAsync(
                        history,
                        executionSettings: executionSettings,
                        kernel);

                    // Stream the results
                    string fullMessage = "";
                    var first = true;
                    await foreach (var content in result)
                    {
                        if (content.Role.HasValue && first)
                        {
                            Console.Write("Assistant > ");
                            first = false;
                        }
                        Console.Write(content.Content);
                        fullMessage += content.Content;
                    }
                    Console.WriteLine();

                    // Add the message from the agent to the chat history
                    history.AddAssistantMessage(fullMessage);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

        }
    }
}
