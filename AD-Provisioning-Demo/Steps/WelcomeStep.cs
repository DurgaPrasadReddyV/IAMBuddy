using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AD_Provisioning_Demo.Models;
using AD_Provisioning_Demo.Steps.Events;
using AD_Provisioning_Demo.Steps.Functions;
using AD_Provisioning_Demo.Steps.States;

namespace AD_Provisioning_Demo.Steps;

public sealed class WelcomeStep : KernelProcessStep<WelcomeState>
{
    public string _welcomeMessage = """
    Welcome! I'm here to help you provision user accounts and access permissions within active directory.
    
    I can assist you with the following tasks:
    1. Creating service accounts (CreateServiceAccount)
    2. Setting up user accounts (CreateUserAccount)
    3. Managing group memberships (ManageGroupMembership)
    4. Creating groups (CreateGroup)
    
    Please let me know which option you'd like to work with to get started. Type 'exit' to leave the process at any time.
    """;

    public WelcomeState? _state;

    public string _requestTypeSelectionSystemPrompt = """
        You are a helpful assistant designed to guide users through selecting active directory objects creation options. Your role is to help users choose the most appropriate option from the following menu:

        1. Creating service accounts
        2. Setting up user accounts
        3. Managing group memberships
        4. Creating and organizing groups

        <CURRENT_REQUEST_VALUE>
        {{current_request_value}}
        <CURRENT_REQUEST_VALUE>

        INSTRUCTIONS:
        - Always be friendly, patient, and clear in your responses
        - Ask clarifying questions to understand the user's specific needs
        - Provide brief explanations of what each option involves when needed
        - Guide users to the most appropriate choice based on their requirements
        - If a user is unsure, help them by asking about their goals or what they're trying to accomplish
        - Keep responses concise but informative
        - Once a user selects an option, display APP and Manager information provided in special note

        EXAMPLE INTERACTIONS:
        - If user says "I need to add a new employee": Guide them toward option 2 (Setting up user accounts) and display ERP as appName and technical manager is carol.
        - If user says "I need an account for our application": Guide them toward option 1 (Creating service accounts) and display ERP as appName and technical manager is carol.
        - If user says "I want to give someone access to a folder": Guide them toward option 3 (Managing group memberships) and display ERP as appName and technical manager is carol.
        - If user says "I'm not sure": Ask what they're trying to accomplish or who needs access to what and display ERP as appName and technical manager is carol.

        Remember: Your goal is to help users quickly identify which option best fits their needs through friendly conversation and targeted questions.

        SPECIALNOTE:
        The user is part of ERP app and Tehnical manager is carol for ERP app.        

        """;

    public string _requestTypeConfirmationSystemPrompt = """
        You are a helpful assistant that confirms user selections for Active Directory provisioning tasks. 
        
        The user has selected: {{selected_request_type}}
        
        Your job is to:
        1. Clearly state what the user has selected
        2. Ask for confirmation (yes/no) to proceed with this selection
        3. Be friendly and professional
        4. If they say no, let them know they can make a different selection
        
        Keep your response concise and clear.
        """;

    [KernelFunction(WelcomeFunctions.Greetings)]
    public async Task WelcomeMessageAsync(KernelProcessStepContext context, Kernel _kernel)
    {
        _state?.Conversation.Add(new ChatMessageContent { Role = AuthorRole.Assistant, Content = _welcomeMessage });
        await context.EmitEventAsync(new() { Id = WelcomeEvents.WelcomeMessageDisplayComplete, Data = _welcomeMessage });
    }

    [KernelFunction(WelcomeFunctions.RequestTypeSelection)]
    public async Task CompleteRequestTypeSelectionAsync(KernelProcessStepContext context, string userMessage, Kernel _kernel)
    {
        // Keeping track of all user interactions
        _state?.Conversation.Add(new ChatMessageContent { Role = AuthorRole.User, Content = userMessage });

        // Check if we're in confirmation mode
        if (_state?.RequestType?.IsValid() == true && _state.RequestTypeConfirmation != true)
        {
            await HandleConfirmationAsync(context, userMessage, _kernel);
            return;
        }

        // Normal request type selection flow
        await HandleRequestTypeSelectionAsync(context, userMessage, _kernel);
    }

    private async Task HandleRequestTypeSelectionAsync(KernelProcessStepContext context, string userMessage, Kernel _kernel)
    {
        Kernel kernel = CreateNewRequestTypeKernel(_kernel);

        GeminiPromptExecutionSettings settings = new()
        {
            ToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions,
            Temperature = 0.7,
            MaxTokens = 4096
        };

        ChatHistory chatHistory = new();
        chatHistory.AddSystemMessage(_requestTypeSelectionSystemPrompt
            .Replace("{{current_request_value}}", JsonSerializer.Serialize(_state?.RequestType, _jsonOptions)));
        chatHistory.AddRange(_state?.Conversation ?? new List<ChatMessageContent>());

        IChatCompletionService chatService = kernel.Services.GetRequiredService<IChatCompletionService>();
        ChatMessageContent response = await chatService.GetChatMessageContentAsync(chatHistory, settings, kernel).ConfigureAwait(false);
        var assistantResponse = "";

        if (response != null)
        {
            assistantResponse = response.Items[0].ToString();
            // Keeping track of all assistant interactions
            _state?.Conversation.Add(new ChatMessageContent { Role = AuthorRole.Assistant, Content = assistantResponse });
        }

        if (_state?.RequestType != null && _state.RequestType.IsValid())
        {
            // Request type identified, now ask for confirmation
            var confirmationMessage = GetRequestTypeConfirmationMessage(_state.RequestType.Type);
            _state?.Conversation.Add(new ChatMessageContent { Role = AuthorRole.Assistant, Content = confirmationMessage });

            await context.EmitEventAsync(new() { Id = WelcomeEvents.RequestTypeConfirmationNeeded, Data = confirmationMessage });
            return;
        }

        // emit event: request type is not valid yet
        await context.EmitEventAsync(new() { Id = WelcomeEvents.RequestTypeIsNotValid, Data = assistantResponse });
    }

    private async Task HandleConfirmationAsync(KernelProcessStepContext context, string userMessage, Kernel _kernel)
    {
        Kernel kernel = CreateConfirmationKernel(_kernel);

        GeminiPromptExecutionSettings settings = new()
        {
            ToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions,
            Temperature = 0.3,
            MaxTokens = 1024
        };

        ChatHistory chatHistory = new();
        var requestTypeName = GetRequestTypeDisplayName(_state?.RequestType?.Type);
        chatHistory.AddSystemMessage(_requestTypeConfirmationSystemPrompt
            .Replace("{{selected_request_type}}", requestTypeName));

        // Only add the recent confirmation conversation
        chatHistory.AddUserMessage(userMessage);

        IChatCompletionService chatService = kernel.Services.GetRequiredService<IChatCompletionService>();
        ChatMessageContent response = await chatService.GetChatMessageContentAsync(chatHistory, settings, kernel).ConfigureAwait(false);

        var assistantResponse = "";
        if (response != null)
        {
            assistantResponse = response.Items[0].ToString();
            _state?.Conversation.Add(new ChatMessageContent { Role = AuthorRole.Assistant, Content = assistantResponse });
        }

        // Check if confirmation was provided
        if (_state?.RequestTypeConfirmation == true)
        {
            Console.WriteLine($"[REQUEST_TYPE_SELECTION_COMPLETED]: {JsonSerializer.Serialize(_state?.RequestType, _jsonOptions)}");
            // Confirmed - proceed to next step
            await context.EmitEventAsync(new() { Id = WelcomeEvents.RequestTypeSelectionComplete, Data = _state?.RequestType, Visibility = KernelProcessEventVisibility.Public });
            await context.EmitEventAsync(new() { Id = WelcomeEvents.RequestTypeCustomerInteractionTranscriptReady, Data = _state?.Conversation, Visibility = KernelProcessEventVisibility.Public });
            return;
        }
        else if (_state?.RequestTypeConfirmation == false)
        {
            // User said no - reset and start over
            ResetRequestTypeSelection();
            var restartMessage = "No problem! Let's start over. Please select which option you'd like to work with from the menu above.";
            _state?.Conversation.Add(new ChatMessageContent { Role = AuthorRole.Assistant, Content = restartMessage });
            await context.EmitEventAsync(new() { Id = WelcomeEvents.RequestTypeIsNotValid, Data = restartMessage });
            return;
        }

        // Still waiting for clear confirmation - emit the assistant response
        await context.EmitEventAsync(new() { Id = WelcomeEvents.RequestTypeConfirmationNeeded, Data = assistantResponse });
    }

    private void ResetRequestTypeSelection()
    {
        if (_state?.RequestType != null)
        {
            _state.RequestType.Type = ERequestType.Unknown;
            _state.RequestTypeConfirmation = null;
        }
    }

    private string GetRequestTypeConfirmationMessage(ERequestType requestType)
    {
        var displayName = GetRequestTypeDisplayName(requestType);
        return $"""
            I understand you want to work with: **{displayName}**\n\n. I see that you are part of ERP app and Technical Manager is Carol. Is this correct? Please reply with 'yes' to confirm or 'no' to select a different option.
            """;
    }

    private string GetRequestTypeDisplayName(ERequestType? requestType)
    {
        return requestType switch
        {
            ERequestType.CreateServiceAccount => "Creating service accounts",
            ERequestType.CreateUserAccount => "Setting up user accounts",
            ERequestType.ManageGroupMembership => "Managing group memberships",
            ERequestType.CreateGroup => "Creating groups",
            _ => "Unknown"
        };
    }

    public override ValueTask ActivateAsync(KernelProcessStepState<WelcomeState> state)
    {
        _state = state.State;
        return ValueTask.CompletedTask;
    }

    private Kernel CreateNewRequestTypeKernel(Kernel _baseKernel)
    {
        // Creating another kernel that only makes use private functions to get the request type from the user
        Kernel kernel = new(_baseKernel.Services);
        kernel.ImportPluginFromFunctions("FetchRequestType", [
            KernelFunctionFactory.CreateFromMethod(OnUserProvidedRequestType, functionName: nameof(OnUserProvidedRequestType)),
        ]);

        return kernel;
    }

    [Description("User provided details of request type value. " +
        "Allowed: 1/2/3/4 or CreateServiceAccount/CreateUserAccount/ManageGroupMembership/CreateGroup ")]
    private void OnUserProvidedRequestType(string requestType)
    {
        if (TryParseRequestType(requestType, out var eRequestType))
        {
            if (_state != null && _state.RequestType != null)
                _state.RequestType.Type = eRequestType;
        }
    }

    private Kernel CreateConfirmationKernel(Kernel _baseKernel)
    {
        // Creating kernel for handling confirmation
        Kernel kernel = new(_baseKernel.Services);
        kernel.ImportPluginFromFunctions("HandleConfirmation", [
            KernelFunctionFactory.CreateFromMethod(OnUserProvidedConfirmation, functionName: nameof(OnUserProvidedConfirmation)),
        ]);

        return kernel;
    }

    [Description("User provided confirmation response. Call this when user confirms or denies their selection. " +
        "Use 'yes', 'true', 'confirm', 'correct', 'proceed' for positive confirmation. " +
        "Use 'no', 'false', 'incorrect', 'wrong', 'different' for negative confirmation.")]
    private void OnUserProvidedConfirmation(string confirmationResponse)
    {
        if (_state != null)
        {
            var response = confirmationResponse.ToLowerInvariant().Trim();
            if (response.Contains("yes") || response.Contains("true") || response.Contains("confirm") ||
                response.Contains("correct") || response.Contains("proceed") || response == "y")
            {
                _state.RequestTypeConfirmation = true;
            }
            else if (response.Contains("no") || response.Contains("false") || response.Contains("incorrect") ||
                     response.Contains("wrong") || response.Contains("different") || response == "n")
            {
                _state.RequestTypeConfirmation = false;
            }
        }
    }

    private static bool TryParseRequestType(string input, out ERequestType eRequestType)
    {
        // Try numeric first
        if (int.TryParse(input, out var num) && Enum.IsDefined(typeof(ERequestType), num))
        {
            eRequestType = (ERequestType)num;
            return true;
        }

        // Then try enum name (case-insensitive)
        if (Enum.TryParse(input, ignoreCase: true, out eRequestType))
            return true;

        eRequestType = ERequestType.Unknown;
        return false;
    }

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false) }
    };
}


