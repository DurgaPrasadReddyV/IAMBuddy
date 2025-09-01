using Microsoft.SemanticKernel;
using AD_Provisioning_Demo.Steps.Events;
using AD_Provisioning_Demo.Steps.Functions;
using AD_Provisioning_Demo.Steps.States;

namespace AD_Provisioning_Demo.Steps;

public class RequestTypeSelectionUserInputStep : KernelProcessStep
{
    [KernelFunction(UserInputFunctions.GetUserInput)]
    public virtual async ValueTask GetUserInputAsync(KernelProcessStepContext context)
    {
        Console.Write("USER: ");
        var userMessage = Console.ReadLine();
        // Emit the user input
        if (userMessage?.Equals("Exit", StringComparison.InvariantCultureIgnoreCase) ?? false)
        {
            await context.EmitEventAsync(new() { Id = UserInputEvents.Exit });
            return;
        }

        await context.EmitEventAsync(new() { Id = UserInputEvents.UserInputReceived, Data = userMessage });
    }
}
