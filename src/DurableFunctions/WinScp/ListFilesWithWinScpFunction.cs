using IsolatedFunctionsConnectionClosed.SftpClients;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;

namespace IsolatedFunctionsConnectionClosed.DurableFunctions.WinScp;

public partial class ListFilesWithWinScpFunction(WinScpSftp winScpSftp, ILogger<WinScpOrchestrator> logger)
{
    private static readonly string SftpDirectory = Environment.GetEnvironmentVariable(nameof(SftpDirectory))!;
    
    [Function(nameof(ListFilesWithWinScpFunction))]
    public Task Run([ActivityTrigger] TaskActivityContext? context = null)
    {
        try
        {
            logger.LogInformation("Listing files in '{@FolderPath}'", SftpDirectory);
        
            var files = winScpSftp.ListFiles(SftpDirectory);
        
            logger.LogInformation("Found '{FilesCount}' files in folder '{@FolderPath}'", files.Count(), SftpDirectory);
        }
        catch (Exception e)
        {
            logger.LogError("Failed to list files {@Exception}", e);
        }
        
        return Task.CompletedTask;
    }
}