using WarrenSoft.Reminders.Domain;

namespace WarrenSoft.Reminders.Infra;

public sealed class CosmosReminderListRepository : IReminderListRepository
{
    private readonly CosmosEntitySet<ReminderList> _lists;

    public CosmosReminderListRepository(CosmosContainerContext context)
    {
        _lists = context.ReminderLists;
    }

    public void Add(ReminderList list) =>
        _lists.Add(list);

    public Task<ReminderList?> FindAsync(string id, CancellationToken cancellationToken = default) =>
        _lists.GetAsync(id, cancellationToken);

    public void Remove(string id) =>
        _lists.Remove(id);
}
