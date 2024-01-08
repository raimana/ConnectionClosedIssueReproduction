using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace IsolatedFunctionsConnectionClosed.DurableFunctions.SshNet;

public class SshNetOrchestrator(ILogger<SshNetOrchestrator> logger)
{
    private static readonly int MaxIterations = int.Parse(Environment.GetEnvironmentVariable(nameof(MaxIterations))!);

    [Function(nameof(SshNetOrchestrator))]
    public async Task Run([OrchestrationTrigger] TaskOrchestrationContext context)
    {
        try
        {
            for (var i = 1; i <= MaxIterations; i++)
            {
                if (context.IsReplaying is false) logger.LogInformation("############### Download Iteration {Count}/{MaxIterations} ###############", i, MaxIterations);
                await context.CallActivityAsync(nameof(ListFilesWithSshNetFunction));
            }

            if (context.IsReplaying is false) logger.LogInformation("SSH.Net Orchestrator run ended: {@InstanceId}", context.InstanceId);
        }
        catch (Exception e)
        {
            if (context.IsReplaying is false) logger.LogError("SSH.Net Orchestrator failed {@Exception}", e);
        }
    }
}

public class SshNetTimeTrigger(ILogger<SshNetTimeTrigger> logger)
{
    [Function(nameof(SshNetTimeTrigger))]
    public async Task StartTimeTrigger([TimerTrigger("%SshNetSchedule%")] TimerInfo myTimer, [DurableClient] DurableTaskClient client, FunctionContext executionContext)
    {
        logger.LogInformation("Starting SSH.Net Orchestrator...");
        
        var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(SshNetOrchestrator));
        
        logger.LogInformation("Started SSH.Net Orchestrator: '{InstanceId}'", instanceId);
    }
}