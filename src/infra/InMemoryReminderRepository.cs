using WarrenSoft.Reminders.Domain;

namespace WarrenSoft.Reminders.Infra;

public sealed class InMemoryReminderRepository : IReminderRepository
{
    private readonly List<Reminder> _reminders = new();

    public void Add(Reminder list) => _reminders.Add(list);

    public Task<Reminder?> GetByIdAsync(string id, CancellationToken cancellationToken = default) => Task.FromResult(_reminders.FirstOrDefault(x => x.Id == id));

    public void Remove(string id) => _reminders.RemoveAll(x => x.Id == id);
}
