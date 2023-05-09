using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Warrensoft.Reminders.Workers;

public class CosmosChangeFeedWorker : BackgroundService
{
    private readonly ILogger<CosmosChangeFeedWorker> _logger;

    public CosmosChangeFeedWorker(ILogger<CosmosChangeFeedWorker> logger)
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
