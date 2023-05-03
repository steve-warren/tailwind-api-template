using Microsoft.Azure.Cosmos;
using WarrenSoft.Reminders.Domain;

namespace WarrenSoft.Reminders.Infra;

internal sealed class CosmosPartitionKeyMap<TEntity> : ICosmosPartitionKeyMap where TEntity : IEntity
{
    private readonly Func<TEntity, string> _partitionKeySelector;

    public CosmosPartitionKeyMap(Func<TEntity, string> partitionKeySelector) =>
        _partitionKeySelector = partitionKeySelector;

    public PartitionKey GetPartitionKey(object entity) =>
        new(_partitionKeySelector((TEntity)entity));
}
