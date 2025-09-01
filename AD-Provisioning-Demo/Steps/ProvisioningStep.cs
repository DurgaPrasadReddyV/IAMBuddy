using Microsoft.SemanticKernel;
using StarodubOleg.GPPG.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AD_Provisioning_Demo.Steps.Events;
using AD_Provisioning_Demo.Steps.Functions;
using AD_Provisioning_Demo.Steps.States;

namespace AD_Provisioning_Demo.Steps
{
    internal class ProvisioningStep : KernelProcessStep<ProvisioningState>
    {
        public ProvisioningState? _state;

        [KernelFunction(ProvisioningFunctions.ProvisionEngineerReview)]
        public async Task ProvisionEngineerReviewAsync(KernelProcessStepContext context, Guid requestId)
        {
            if (_state is null) throw new InvalidOperationException("State is null.");
            _state.RequestId = requestId;
            Console.WriteLine();
            Console.WriteLine("****************************************************************************");
            Console.WriteLine($"[SIMULATION] Sending to provisioning team for review.");
            Console.WriteLine("****************************************************************************");
            Console.WriteLine();
            await Program.ProcessProvisionEngineerReviewAsync(requestId);
        }

        [KernelFunction(ProvisioningFunctions.ProcessProvisionEngineerReview)]
        public async Task ProcessProvisionEngineerReviewAsync(KernelProcessStepContext context, bool provisionEngineerReview)
        {
            if (_state is null) throw new InvalidOperationException("State is null.");

            if (provisionEngineerReview)
            {
                await Task.Delay(2000);
                Console.WriteLine();
                Console.WriteLine("****************************************************************************");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Provisioning Team Approved");
                Console.ResetColor();
                Console.WriteLine("****************************************************************************");
                Console.WriteLine();
                await context.EmitEventAsync(new() { Id = ProvisioningEvents.ProvisionEngineerReviewApproved, Data = _state.RequestId });
            }
            else
            {
                Console.WriteLine($"Provisioning Team Rejected");
                await context.EmitEventAsync(new() { Id = ProvisioningEvents.ProcessCompleted, Data = false });
            }
        }

        [KernelFunction(ProvisioningFunctions.ProvisionResource)]
        public async Task ProvisionResourceAsync(KernelProcessStepContext context, Guid requestId)
        {
            await Task.Delay(2000);
            Console.WriteLine();
            Console.WriteLine("****************************************************************************");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[SIMULATION] Provisioning of Resource {requestId} Successful");
            Console.ResetColor();
            Console.WriteLine("****************************************************************************");
            Console.WriteLine();
            await context.EmitEventAsync(new() { Id = ProvisioningEvents.ProcessCompleted, Data = true });
        }

        public override ValueTask ActivateAsync(KernelProcessStepState<ProvisioningState> state)
        {
            _state = state.State;
            return ValueTask.CompletedTask;
        }
    }
}
