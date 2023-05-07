using System.Net;
using KsuidDotNet;
using Microsoft.Azure.Cosmos;
using WarrenSoft.Reminders.Domain;

namespace Warrensoft.Reminders.Infra;

// Similar to DbSet
public class CosmosEntityContainer<TAggregateRoot> where TAggregateRoot : class, IAggregateRoot
{
    private readonly Dictionary<string, EntityEntry> _entryMap = new();
    private readonly Container _container = null!;
    private readonly Func<TAggregateRoot, string?> _partitionKeySelector = null!;

    public CosmosEntityContainer(Container container, Func<TAggregateRoot, string?> partitionKeySelector)
    {
        _container = container;
        _partitionKeySelector = partitionKeySelector;
    }

    private IEnumerable<EntityEntry> Entries => _entryMap.Values;

    public async ValueTask<TAggregateRoot?> FindAsync(string id, string partitionKey, CancellationToken cancellationToken = default)
    {
        _entryMap.TryGetValue(key: id, out var entry);

        if (entry is null) // see EntityFinder.FindAsync
        {
            try
            {
                var response = await _container.ReadItemAsync<TAggregateRoot>(id, new PartitionKey(partitionKey), cancellationToken: cancellationToken);

                if (response.StatusCode == HttpStatusCode.OK)
                    entry = new EntityEntry { Entity = response.Resource, State = EntityState.Unchanged };
                
                else
                    throw new InvalidOperationException();
            }

            catch(CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                entry = new EntityEntry { Entity = null!, State = EntityState.Unchanged };
            }

            _entryMap.Add(key: id, value: entry);
        }

        return (TAggregateRoot?) entry?.Entity;
    }

    public void Add(TAggregateRoot entity)
    {
        _entryMap.TryAdd(key: entity.Id, value: new EntityEntry { Entity = entity, State = EntityState.Added });
    }

    public void Remove(TAggregateRoot entity)
    {
        _entryMap.TryGetValue(key: entity.Id, out var entry);

        if (entry is not null)
            entry.State = EntityState.Removed;
    }

    public void Update(TAggregateRoot entity)
    {
        _entryMap.TryGetValue(key: entity.Id, out var entry);

        if (entry is not null)
            entry.State = EntityState.Modified;
    }

    public void Attach(TAggregateRoot entity)
    {
        _entryMap.TryAdd(key: entity.Id, value: new EntityEntry { Entity = entity, State = EntityState.Detached });
    }

    public void Clear() =>
        _entryMap.Clear();

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (_entryMap.Count == 0)
            return;

        foreach (var entry in Entries)
        {
            var entity = (TAggregateRoot)entry.Entity;
            var partitionKey = _partitionKeySelector(entity);
            var transaction = _container.CreateTransactionalBatch(new PartitionKey(partitionKey));

            switch (entry.State)
            {
                case EntityState.Added:
                    transaction.CreateItem(entity);
                    break;
                case EntityState.Modified:
                    transaction.ReplaceItem(entity.Id, entity);
                    break;
                case EntityState.Removed:
                    transaction.DeleteItem(entity.Id);
                    break;
                default:
                    break;
            }

            var emitter = entity as IEventEmitter;

            if (emitter is not null)
                foreach (object domainEvent in emitter.DomainEvents)
                    transaction.CreateItem(new { Id = Ksuid.NewKsuid("ev_"), domainEvent, PartitionKey = partitionKey });

            var response = await transaction.ExecuteAsync(cancellationToken);

            if (response.IsSuccessStatusCode is false)
                throw new InvalidOperationException("Database error.");

            emitter?.ClearEvents();

            switch (entry.State)
            {
                case EntityState.Added:
                    entry.State = EntityState.Unchanged;
                    break;

                case EntityState.Modified:
                    entry.State = EntityState.Unchanged;
                    break;

                case EntityState.Removed:
                    entry.State = EntityState.Unchanged;
                    _entryMap.Remove(entry.Entity.Id);
                    break;
                case EntityState.Detached:
                    break;
                case EntityState.Unchanged:
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }

    private enum EntityState
    {
        Detached,
        Unchanged,
        Removed,
        Modified,
        Added
    }

    private sealed class EntityEntry
    {
        public IEntity Entity { get; init; } = null!;
        public EntityState State { get; set; } = EntityState.Unchanged;
    }
}