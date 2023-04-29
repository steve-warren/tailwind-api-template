using WarrenSoft.Reminders.Domain;

namespace WarrenSoft.Reminders.Infra;

public sealed class InMemoryReminderListRepository : IReminderListRepository
{
    private readonly List<ReminderList> _lists = new();

    public void Add(ReminderList list) => _lists.Add(list);

    public Task<ReminderList?> FindAsync(string id, CancellationToken cancellationToken = default) => Task.FromResult(_lists.FirstOrDefault(x => x.Id == id));

    public void Remove(string id) => _lists.RemoveAll(x => x.Id == id);
}
