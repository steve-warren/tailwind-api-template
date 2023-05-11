using System.Text.Json;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Warrensoft.Reminders.Workers;

public class CosmosChangeFeedWorker : BackgroundService
{
    private readonly ILogger<CosmosChangeFeedWorker> _logger;
    private readonly IConfiguration _configuration;
    private readonly CosmosClient _cosmosClient;
    private ChangeFeedProcessor _changeFeedProcessor = null!;

    public CosmosChangeFeedWorker(ILogger<CosmosChangeFeedWorker> logger, IConfiguration configuration, CosmosClient cosmosClient)
    {
        _logger = logger;
        _configuration = configuration;
        _cosmosClient = cosmosClient;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var databaseName = _configuration["Cosmos:DatabaseName"];
        var sourceContainerName = _configuration["Cosmos:ContainerName"];
        var leaseContainerName = _configuration["Cosmos:LeaseContainerName"];

        var leaseContainer = _cosmosClient.GetContainer(databaseName, leaseContainerName);
        var changeFeedProcessor = _cosmosClient.GetContainer(databaseName, sourceContainerName)
            .GetChangeFeedProcessorBuilder<JsonDocument>(processorName: "changeFeedSample", onChangesDelegate: HandleChangesAsync)
                .WithInstanceName(Environment.MachineName)
                .WithLeaseContainer(leaseContainer)
                .Build();

        await changeFeedProcessor.StartAsync();
        _changeFeedProcessor = changeFeedProcessor;

        _logger.LogInformation("Cosmos DB change feed processor started.");
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _changeFeedProcessor.StopAsync();

        _logger.LogInformation("Cosmos DB change feed processor stopped.");
    }
    
    private Task HandleChangesAsync(ChangeFeedProcessorContext context, IReadOnlyCollection<JsonDocument> changes, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Changes received at {dateTime}, consuming {requestCharge} RU.", DateTimeOffset.UtcNow, context.Headers.RequestCharge);

        foreach(var document in changes)
        {
            document.RootElement.TryGetProperty("id", out JsonElement id);
            _logger.LogInformation("Document {id} has been updated.", id);
        }
        
        return Task.CompletedTask;
    }
}
