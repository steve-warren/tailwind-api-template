using Microsoft.Azure.Cosmos;
using WarrenSoft.Reminders.Domain;

namespace WarrenSoft.Reminders.Infra;

public sealed class CosmosUnitOfWork : IUnitOfWork
{
    private readonly HashSet<IEntity> _entities = new(EntityIdentityEqualityComparer.Instance);
    private readonly Container _container;
    private readonly Dictionary<Type, ICosmosPartitionKeyMap> _partitionKeyMap = new();
    
    public CosmosUnitOfWork(Container container)
    {
        _container = container;
    }

    internal Container Container => _container;

    internal void MapPartitionKey<TEntity>(Func<TEntity, string> partitionKeySelector) where TEntity : IEntity
    {
        // todo - check for existing map
        var map = new CosmosPartitionKeyMap<TEntity>();

        map.MapEntity(partitionKeySelector);

        _partitionKeyMap.Add(typeof(TEntity), map);
    }

    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_entities.Count == 0)
        {
            return Task.CompletedTask;
        }

        var entity = _entities.First();

        var partitionKey = new PartitionKey(_partitionKeyMap[entity.GetType()].GetPartitionKey(entity));

        return _container.CreateItemAsync((object)entity, partitionKey, cancellationToken: cancellationToken);
    }

    public async Task<TEntity?> GetAsync<TEntity>(string id, string partitionKey, CancellationToken cancellationToken = default) where TEntity : IEntity
    {
        if (_entities.FirstOrDefault(e => e.Id == id) is TEntity entity)
            return entity;

        var response = await _container.ReadItemAsync<TEntity>(id, new PartitionKey(partitionKey), cancellationToken: cancellationToken);

        return response.Resource;
    }

    public void Register(IEntity entity) =>
        _entities.Add(entity);
}
