using WarrenSoft.Reminders.Domain;

namespace mock;

public class MockReminderListRepository : IReminderListRepository
{
    private readonly List<ReminderList> _reminderLists = new();

    public MockReminderListRepository(IEnumerable<ReminderList>? reminderLists = null)
    {
        if (reminderLists is not null)
            _reminderLists.AddRange(reminderLists);
    }

    public void Add(ReminderList list) => _reminderLists.Add(list);

    public Task<ReminderList?> FindAsync(string id, CancellationToken cancellationToken = default) => Task.FromResult(_reminderLists.FirstOrDefault(list => list.Id == id));

    public void Remove(string id) => _reminderLists.RemoveAll(list => list.Id == id);
}
