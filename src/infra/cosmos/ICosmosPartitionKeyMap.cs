namespace WarrenSoft.Reminders.Infra;

internal interface ICosmosPartitionKeyMap
{
    string GetPartitionKey(object entity);
}
