using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Warrensoft.Reminders.Workers;

public class AzureServiceBusWorker : BackgroundService
{
    private readonly ILogger<AzureServiceBusWorker> _logger;

    public AzureServiceBusWorker(ILogger<AzureServiceBusWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
}
