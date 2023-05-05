using System.Net;
using Microsoft.Azure.Cosmos;

namespace Warrensoft.Reminders.Infra;

public record Foo(string Id);

public class FooSet
{
    private readonly Dictionary<string, EntityEntry> _entries = new();
    private readonly Container _container = null!;
    private readonly Func<Foo, string?> _partitionKeySelector = null!;

    public FooSet(Container container, Func<Foo, string?> partitionKeySelector)
    {
        _container = container;
        _partitionKeySelector = partitionKeySelector;
    }

    public async ValueTask<Foo?> FindAsync(string id, string partitionKey, CancellationToken cancellationToken = default)
    {
        _entries.TryGetValue(key: id, out var entry);

        if (entry is null) // see EntityFinder.FindAsync
        {
            var response = await _container.ReadItemAsync<Foo>(id, new PartitionKey(partitionKey), cancellationToken: cancellationToken);

            entry = response.StatusCode switch
            {
                HttpStatusCode.OK => new EntityEntry { Entity = response.Resource, State = EntityState.Unchanged },
                HttpStatusCode.NotFound => new EntityEntry { Entity = null!, State = EntityState.Unchanged },
                _ => throw new NotImplementedException()
            };

            _entries.Add(key: id, value: entry);
        }

        return entry?.Entity;
    }

    public void Add(Foo entity)
    {
        _entries.TryAdd(key: entity.Id, value: new EntityEntry { Entity = entity, State = EntityState.Added });
    }

    public void Remove(Foo entity)
    {
        _entries.TryGetValue(key: entity.Id, out var entry);

        if (entry is not null)
            entry.State = EntityState.Removed;
    }

    public void Update(Foo entity)
    {
        _entries.TryGetValue(key: entity.Id, out var entry);

        if (entry is not null)
            entry.State = EntityState.Modified;
    }

    public void Attach(Foo entity)
    {
        _entries.TryAdd(key: entity.Id, value: new EntityEntry { Entity = entity, State = EntityState.Detached });
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (_entries.Count == 0)
            return;
        
        else if(_entries.Count == 1)
        {
            var entry = _entries.Values.First();

            await (entry.State switch
            {
                EntityState.Added => AddSingleAsync(entry, cancellationToken),
                EntityState.Modified => UpdateSingleAsync(entry, cancellationToken),
                EntityState.Removed => RemoveSingleAsync(entry, cancellationToken),
                _ => Task.CompletedTask
            });
        }

        else
        {
            foreach(var entries in _entries.GroupBy(kvp => kvp.Value.State, kvp => kvp.Value))
                await(entries.Key switch
                {
                    EntityState.Added => AddManyAsync(entries, cancellationToken),
                    EntityState.Modified =>UpdateManyAsync(entries, cancellationToken),
                    EntityState.Removed => RemoveManyAsync(entries, cancellationToken),
                    _ => throw new NotImplementedException()
                });
        }
    }

    private Task RemoveManyAsync(IEnumerable<EntityEntry> removedEntries, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private Task UpdateManyAsync(IEnumerable<EntityEntry> modifiedEntries, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private Task AddManyAsync(IEnumerable<EntityEntry> addedEntries, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private async Task AddSingleAsync(EntityEntry entry, CancellationToken cancellationToken)
    {
        string? partitionKeyValue = _partitionKeySelector(entry.Entity);
        PartitionKey partitionKey = partitionKeyValue is null ? PartitionKey.Null : new PartitionKey(partitionKeyValue);

        var response = await _container.CreateItemAsync(entry.Entity, partitionKey, cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.OK)
            entry.State = EntityState.Unchanged;
    }

    private async Task RemoveSingleAsync(EntityEntry entry, CancellationToken cancellationToken)
    {
        string? partitionKeyValue = _partitionKeySelector(entry.Entity);
        PartitionKey partitionKey = partitionKeyValue is null ? PartitionKey.Null : new PartitionKey(partitionKeyValue);

        var response = await _container.DeleteItemAsync<Foo>(id: entry.Entity.Id, partitionKey: partitionKey, cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.OK)
            entry.State = EntityState.Unchanged;
    }

    private async Task UpdateSingleAsync(EntityEntry entry, CancellationToken cancellationToken)
    {
        string? partitionKeyValue = _partitionKeySelector(entry.Entity);
        PartitionKey partitionKey = partitionKeyValue is null ? PartitionKey.Null : new PartitionKey(partitionKeyValue);

        var response = await _container.UpsertItemAsync(entry.Entity, partitionKey, cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.OK)
            entry.State = EntityState.Unchanged;
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
        public Foo Entity { get; init; } = null!;
        public EntityState State { get; set; } = EntityState.Unchanged;
    }
}