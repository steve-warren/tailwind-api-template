using Warrensoft.Reminders.Infra;
using WarrenSoft.Reminders.Domain;

namespace WarrenSoft.Reminders.Infra;

public sealed class CosmosReminderListRepository : IRepository<ReminderList>
{
    private readonly EntitySet<ReminderList> _lists;

    public CosmosReminderListRepository(CosmosContext cosmosContext) =>
        _lists = cosmosContext.Reminders.Entity<ReminderList>(partitionKeySelector: list => list.Id);

    public void Add(ReminderList list) =>
        _lists.Add(list);

    public Task<ReminderList?> GetAsync(string id, CancellationToken cancellationToken = default) =>
        _lists.GetAsync(id, partitionKey: id, cancellationToken);

    public void Remove(string id) =>
        _lists.Remove(id);
}
