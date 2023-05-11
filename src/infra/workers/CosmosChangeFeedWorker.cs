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

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        // string databaseName = _configuration["SourceDatabaseName"];
        // string sourceContainerName = _configuration["SourceContainerName"];
        // string leaseContainerName = _configuration["LeasesContainerName"];

        // Container leaseContainer = _cosmosClient.GetContainer(databaseName, leaseContainerName);
        // ChangeFeedProcessor changeFeedProcessor = _cosmosClient.GetContainer(databaseName, sourceContainerName)
        //     .GetChangeFeedProcessorBuilder<ToDoItem>(processorName: "changeFeedSample", onChangesDelegate: HandleChangesAsync)
        //         .WithInstanceName("consoleHost")
        //         .WithLeaseContainer(leaseContainer)
        //         .Build();

        // await changeFeedProcessor.StartAsync();
        // _changeFeedProcessor = changeFeedProcessor;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        return base.StopAsync(cancellationToken);
    }
}
