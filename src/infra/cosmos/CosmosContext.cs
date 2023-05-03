using Microsoft.Azure.Cosmos;
using WarrenSoft.Reminders.Infra;

namespace Warrensoft.Reminders.Infra;

public class CosmosContext
{
    private readonly CosmosClient _client;
    private readonly Lazy<CosmosContainer> _reminders;
    private readonly List<Lazy<CosmosContainer>> _containers = new();

    public CosmosContext(CosmosClient client, string databaseName)
    {
        _client = client;
        DatabaseName  = databaseName;

        _reminders = CreateUnitOfWork("reminders");
    }

    public string DatabaseName { get; }
    public CosmosContainer Reminders { get => _reminders.Value; }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var initializedContainers = _containers.Where(container => container.IsValueCreated)
                                               .Select(container => container.Value);
        
        foreach(var container in initializedContainers)
            await container.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private Lazy<CosmosContainer> CreateUnitOfWork(string containerName)
    {
        var container = _client.GetContainer(DatabaseName, containerName);

        var lazy = new Lazy<CosmosContainer>(() => new(container));

        _containers.Add(lazy);

        return lazy;
    }
}