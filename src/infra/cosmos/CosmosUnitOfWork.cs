using Microsoft.Azure.Cosmos;
using WarrenSoft.Reminders.Domain;

namespace WarrenSoft.Reminders.Infra;

public sealed class CosmosContainer : IUnitOfWork
{
    private readonly HashSet<IEntity> _identityMap = new(EntityIdentityEqualityComparer.Instance);
    private readonly Dictionary<Type, ICosmosPartitionKeyMap> _partitionKeyMap = new();
    private readonly Container _container;
    
    public CosmosContainer(Container container)
    {
        _container = container;
    }

    internal Container Container => _container;

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (_identityMap.Count == 0)
        {
            return Task.CompletedTask;
        }

        var entity = _identityMap.First();

        var partitionKey = _partitionKeyMap[entity.GetType()].GetPartitionKey(entity);

        return _container.CreateItemAsync((object)entity, partitionKey, cancellationToken: cancellationToken);
    }

    public async Task<TEntity?> GetAsync<TEntity>(string id, string partitionKey, CancellationToken cancellationToken = default) where TEntity : IEntity
    {
        if (_identityMap.FirstOrDefault(e => e.Id == id) is TEntity entity)
            return entity;

        var response = await _container.ReadItemAsync<TEntity>(id, new PartitionKey(partitionKey), cancellationToken: cancellationToken);

        return response.Resource;
    }

    public void Register(IEntity entity) =>
        _identityMap.Add(entity);

    public EntitySet<TEntity> Entity<TEntity>() where TEntity : IEntity =>
        new(this);
    
    public EntitySet<TEntity> Entity<TEntity>(Func<TEntity, string> partitionKeySelector) where TEntity : IEntity
    {
        MapPartitionKey(partitionKeySelector);

        return Entity<TEntity>();
    }

    private void MapPartitionKey<TEntity>(Func<TEntity, string> partitionKeySelector) where TEntity : IEntity
    {
        if (!_partitionKeyMap.TryGetValue(typeof(TEntity), out var map))
            _partitionKeyMap.Add(typeof(TEntity), new CosmosPartitionKeyMap<TEntity>(partitionKeySelector));
    }
}
