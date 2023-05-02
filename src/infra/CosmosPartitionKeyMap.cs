using WarrenSoft.Reminders.Domain;

namespace WarrenSoft.Reminders.Infra;

public interface ICosmosPartitionKeyMap
{
    string GetPartitionKey(object entity);
}

public sealed class CosmosPartitionKeyMap<TEntity> : ICosmosPartitionKeyMap where TEntity : IEntity
{
    private readonly Dictionary<Type, Func<TEntity, string>> _map = new();

    public void MapEntity(Func<TEntity, string> partitionKeySelector)
    {
        _map.Add(typeof(TEntity), partitionKeySelector);
    }

    public string GetPartitionKey(object entity)
    {
        if (_map.TryGetValue(entity.GetType(), out var partitionKeySelector))
        {
            return partitionKeySelector((TEntity)entity);
        }

        throw new InvalidOperationException($"No partition key mapping found for {entity.GetType()}");
    }
}
