using WarrenSoft.Reminders.Domain;

namespace WarrenSoft.Reminders.Infra;

public sealed class CosmosReminderListRepository : IReminderListRepository
{
    private readonly EntitySet<ReminderList> _lists;

    public CosmosReminderListRepository(CosmosUnitOfWork unitOfWork) =>
        _lists = unitOfWork.Set<ReminderList>(list => list.Id);

    public void Add(ReminderList list) =>
        _lists.Add(list);

    public Task<ReminderList?> FindAsync(string id, CancellationToken cancellationToken = default) =>
        _lists.GetAsync(id, partitionKey: id, cancellationToken);

    public void Remove(string id) =>
        _lists.Remove(id);
}
