using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace IsolatedFunctionsConnectionClosed.DurableFunctions.WinScp;

public class WinScpOrchestrator(ILogger<WinScpOrchestrator> logger)
{
    private static readonly int MaxIterations = int.Parse(Environment.GetEnvironmentVariable(nameof(MaxIterations))!);

    [Function(nameof(WinScpOrchestrator))]
    public async Task Run([OrchestrationTrigger] TaskOrchestrationContext context)
    {
        try
        {
            for (var i = 1; i <= MaxIterations; i++)
            {
                if (context.IsReplaying is false) logger.LogInformation("############### Download Iteration {Count}/{MaxIterations} ###############", i, MaxIterations);
                await context.CallActivityAsync(nameof(ListFilesWithWinScpFunction));
            }
        
            if (context.IsReplaying is false) logger.LogInformation("WinSCP Orchestrator run ended: {@InstanceId}", context.InstanceId);
        }
        catch (Exception e)
        {
            if (context.IsReplaying is false) logger.LogError("WinSCP Orchestrator failed {@Exception}", e);
        }
    }
}

public class WinScpTimeTrigger(ILogger<WinScpTimeTrigger> logger)
{
    [Function(nameof(WinScpTimeTrigger))]
    public async Task StartTimeTrigger([TimerTrigger("%WinScpSchedule%")] TimerInfo myTimer, [DurableClient] DurableTaskClient client, FunctionContext executionContext)
    {
        logger.LogInformation("Starting WinScp Orchestrator...");
        
        var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(WinScpOrchestrator));
        
        logger.LogInformation("Started WinScp Orchestrator: '{InstanceId}'", instanceId);
    }
}