using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using StarodubOleg.GPPG.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AD_Provisioning_Demo.Models;
using AD_Provisioning_Demo.Steps.Events;
using AD_Provisioning_Demo.Steps.Functions;
using AD_Provisioning_Demo.Steps.States;

namespace AD_Provisioning_Demo.Steps
{
    internal class ApprovalCollectionStep : KernelProcessStep<ApprovalCollectionState>
    {
        public ApprovalCollectionState? _state;

        [KernelFunction(ApprovalCollectionFunctions.InitializeApprovalsCollection)]
        public async Task InitializeCollectionAsync(KernelProcessStepContext context, ApprovalSpec approvalSpec)
        {
            if (_state is null)
                throw new InvalidOperationException("State is null.");
            _state.RequestId = approvalSpec.RequestId;
            _state.Pending = approvalSpec.RequiredApprovals.Select(a => a.Id).ToHashSet();

            // Sending approvals right from here for demo purposes
            Console.WriteLine();
            Console.WriteLine("****************************************************************************");
            foreach (var approval in approvalSpec.RequiredApprovals)
            {
                Console.WriteLine($"[SIMULATION] Approval request sent to '{approval.Approver}' for Approval ID '{approval.Id}'.");
                await Program.ProcessApprovalsAsync(approval);
            }
            Console.WriteLine("****************************************************************************");
            Console.WriteLine();
        }

        [KernelFunction(ApprovalCollectionFunctions.RecordApprovals)]
        public async Task RecordApprovalsAsync(KernelProcessStepContext context, Approval approval)
        {
            if (_state is null)
                throw new InvalidOperationException("State is null.");

            if (!_state.Pending.Contains(approval.Id))
            {
                Console.WriteLine($"[WARNING] Approval from '{approval.Id}' with ID '{approval.Id}' is not expected or already recorded.");
            }

            _state.Received[approval.Id] = approval;
            _state.Pending.Remove(approval.Id);

            await context.EmitEventAsync(new() { Id = ApprovalCollectionEvents.ApprovalRecorded, Data = null, Visibility = KernelProcessEventVisibility.Public });
        }

        [KernelFunction(ApprovalCollectionFunctions.CheckApprovalsCompletion)]
        public async Task CheckApprovalsCompletionAsync(KernelProcessStepContext context)
        {
            if (_state is null)
                throw new InvalidOperationException("State is null.");
            if (_state.AllRequiredReceived)
            {
                await context.EmitEventAsync(new() { Id = ApprovalCollectionEvents.AllApprovalsReceived, Data = null, Visibility = KernelProcessEventVisibility.Public });
            }
        }

        [KernelFunction(ApprovalCollectionFunctions.FinalOutcome)]
        public async Task FinalOutcomeAsync(KernelProcessStepContext context)
        {
            if (_state is null)
                throw new InvalidOperationException("State is null.");

            // Print received approvals
            Console.WriteLine();
            Console.WriteLine("============================ Approvals Received =============================");
            foreach (var approval in _state.Received.Values)
            {
                await Task.Delay(2000);
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine($"ID:         {approval.Id}");
                Console.WriteLine($"Approver:   {approval.Approver}");
                Console.WriteLine($"Role:       {approval.Role}");
                Console.WriteLine($"Approved:   {approval.Approved}");
                Console.WriteLine($"Timestamp:  {approval.Timestamp}");
                Console.WriteLine("--------------------------------------------------");
            }

            // Print pending approvals
            if (_state.Pending.Count > 0)
            {
                Console.WriteLine("=== Pending Approvals ===");
                foreach (var pendingId in _state.Pending)
                {
                    Console.WriteLine($"Pending Approval ID: {pendingId}");
                }
            }

            if (_state.AllRequiredReceived && _state.AnyRejected)
            {
                Console.WriteLine();
                Console.WriteLine("[FINAL OUTCOME] The request has been rejected due to one or more rejections.");
                Console.WriteLine();
            }
            else if (_state.AllRequiredReceived && !_state.AnyRejected)
            {
                await Task.Delay(2000);
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[FINAL OUTCOME] The request has been fully approved.");
                Console.ResetColor();
                Console.WriteLine();
                await context.EmitEventAsync(new() { Id = ApprovalCollectionEvents.AllApprovalsApproved, Data = _state.RequestId, Visibility = KernelProcessEventVisibility.Public });
            }
            else
            {
                throw new InvalidOperationException("FinalOutcome called but not all required approvals have been received.");
            }
        }

        public override ValueTask ActivateAsync(KernelProcessStepState<ApprovalCollectionState> state)
        {
            _state = state.State;
            return ValueTask.CompletedTask;
        }
    }
}
