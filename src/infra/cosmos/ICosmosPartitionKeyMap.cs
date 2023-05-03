using Microsoft.Azure.Cosmos;

namespace WarrenSoft.Reminders.Infra;

internal interface ICosmosPartitionKeyMap
{
    PartitionKey GetPartitionKey(object entity);
}
