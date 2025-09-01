using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AD_Provisioning_Demo.Steps.Events;
using AD_Provisioning_Demo.Steps.Functions;

namespace AD_Provisioning_Demo.Steps;

public class DisplayRequestTypeSelectionAssistantMessageStep : KernelProcessStep
{
    [KernelFunction(DisplayAssistantMessageFunctions.ShowOnConsole)]
    public async ValueTask DisplayAssistantMessageAsync(KernelProcessStepContext context, string assistantMessage)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine();
        Console.WriteLine($"ASSISTANT: {assistantMessage}\n");
        Console.ResetColor();

        // Emit the assistantMessageGenerated
        await context.EmitEventAsync(new() { Id = DisplayAssistantMessageEvents.AssistantResponseGenerated, Data = assistantMessage });
    }
}
