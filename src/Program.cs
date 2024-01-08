using IsolatedFunctionsConnectionClosed.SftpClients;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureLogging((_, loggingBuilder) => { loggingBuilder.AddFile("Logs/{Date}.txt"); })
    .ConfigureServices((_, services) =>
    {
        services.AddOptions<SftpSettings>().BindConfiguration(SftpSettings.Section);
        services.AddSingleton<WinScpSftp>();
        services.AddSingleton<SshNetSftp>();
    })
    .Build();

host.Run();