using System.Net;
using Microsoft.Azure.Cosmos;
using WarrenSoft.Reminders.Domain;

namespace Warrensoft.Reminders.Infra;

public class Account: IEntity, IEventEmitter
{
    public string Id { get; init; }
    public string Name { get; set; }

    public List<IDomainEvent> DomainEvents { get; } = new();

    public void Foo() =>
        DomainEvents.Add(new AccountCreatedEvent(DateTimeOffset.UtcNow, this));
}

public record AccountCreatedEvent(DateTimeOffset CreatedOn, Account Account) : IDomainEvent;

// Similar to DbSet
public class CosmosEntityContainer<TEntity> where TEntity : class, IEntity
{
    private readonly Dictionary<string, EntityEntry> _entryMap = new();
    private readonly Container _container = null!;
    private readonly Func<TEntity, string?> _partitionKeySelector = null!;

    public CosmosEntityContainer(Container container, Func<TEntity, string?> partitionKeySelector)
    {
        _container = container;
        _partitionKeySelector = partitionKeySelector;
    }

    private IEnumerable<EntityEntry> Entries => _entryMap.Values;

    public async ValueTask<TEntity?> FindAsync(string id, string partitionKey, CancellationToken cancellationToken = default)
    {
        _entryMap.TryGetValue(key: id, out var entry);

        if (entry is null) // see EntityFinder.FindAsync
        {
            var response = await _container.ReadItemAsync<TEntity>(id, new PartitionKey(partitionKey), cancellationToken: cancellationToken);

            entry = response.StatusCode switch
            {
                HttpStatusCode.OK => new EntityEntry { Entity = response.Resource, State = EntityState.Unchanged },
                HttpStatusCode.NotFound => new EntityEntry { Entity = null!, State = EntityState.Unchanged },
                _ => throw new NotImplementedException()
            };

            _entryMap.Add(key: id, value: entry);
        }

        return (TEntity?) entry?.Entity;
    }

    public void Add(Account entity)
    {
        _entryMap.TryAdd(key: entity.Id, value: new EntityEntry { Entity = entity, State = EntityState.Added });
    }

    public void Remove(Account entity)
    {
        _entryMap.TryGetValue(key: entity.Id, out var entry);

        if (entry is not null)
            entry.State = EntityState.Removed;
    }

    public void Update(Account entity)
    {
        _entryMap.TryGetValue(key: entity.Id, out var entry);

        if (entry is not null)
            entry.State = EntityState.Modified;
    }

    public void Attach(Account entity)
    {
        _entryMap.TryAdd(key: entity.Id, value: new EntityEntry { Entity = entity, State = EntityState.Detached });
    }

    public void Clear() =>
        _entryMap.Clear();

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (_entryMap.Count == 0)
            return Task.CompletedTask;

        if(_entryMap.Count == 1 && 
           Entries.First().Entity is IEventEmitter emitter &&
           emitter.DomainEvents.Count == 0)
        {
            return SaveOneWithoutEventsAsync(cancellationToken);
        }

        else
        {
            EnsureAllPartitionKeysAreEqual();
            return SaveOneorMoreWithEventsAsync(cancellationToken);
        }
    }

    private async Task SaveOneWithoutEventsAsync(CancellationToken cancellationToken)
    {
        var entry = _entryMap.First().Value;

        switch(entry.State)
        {
            case EntityState.Added:
                var response = await _container.CreateItemAsync((TEntity) entry.Entity, GetPartitionKey(entry), cancellationToken: cancellationToken);

                if (response.StatusCode == HttpStatusCode.Created)
                    entry.State = EntityState.Unchanged;
                break;

            case EntityState.Modified:
                response = await _container.UpsertItemAsync((TEntity) entry.Entity, GetPartitionKey(entry), cancellationToken: cancellationToken);

                if (response.StatusCode == HttpStatusCode.OK)
                    entry.State = EntityState.Unchanged;
                break;

            case EntityState.Removed:
                response = await _container.DeleteItemAsync<TEntity>(id: entry.Entity.Id, partitionKey: GetPartitionKey(entry), cancellationToken: cancellationToken);

                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    entry.State = EntityState.Unchanged;
                    _entryMap.Remove(key: entry.Entity.Id);
                }

                break;

            default:
                throw new NotImplementedException();
        }
    }

    private async Task SaveOneorMoreWithEventsAsync(CancellationToken cancellationToken)
    {
        var partitionKey = GetPartitionKey(Entries.First());

        var transaction = _container.CreateTransactionalBatch(partitionKey);

        foreach(var entry in Entries)
        {
            var entity = entry.Entity;

            if (entity is IEventEmitter emitter)
            {
                foreach(var domainEvent in emitter.DomainEvents)
                    transaction.CreateItem(domainEvent);
            }

            switch(entry.State)
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
                    throw new NotImplementedException();
            }
        }

        var response = await transaction.ExecuteAsync(cancellationToken);

        if (response.IsSuccessStatusCode is false)
            throw new InvalidOperationException("Database error.");

        var removed = new List<string>();

        foreach(var entry in Entries)
        {
            if (entry.Entity is IEventEmitter emitter)
                emitter.ClearEvents();

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
                    removed.Add(entry.Entity.Id);
                    break;
                case EntityState.Detached:
                    break;
                case EntityState.Unchanged:
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        foreach(var id in removed)
            _entryMap.Remove(key: id);
    }

    private void EnsureAllPartitionKeysAreEqual()
    {
        var firstPartitionKey = GetPartitionKey(_entryMap.Values.First());

        if (_entryMap.Values.All(entry => GetPartitionKey(entry).Equals(firstPartitionKey)))
            return;

        throw new InvalidOperationException("All entities must belong to the same partition.");
    }

    private PartitionKey GetPartitionKey(EntityEntry entry)
    {
        var entity = (TEntity)entry.Entity;
        var partitionKey = _partitionKeySelector(entity);
        
        return partitionKey is null ? PartitionKey.Null : new PartitionKey(_partitionKeySelector(entity));
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