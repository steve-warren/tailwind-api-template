namespace WarrenSoft.Reminders.Domain;

public interface IReminderRepository
{
    Task<Reminder?> GetAsync(string id, CancellationToken cancellationToken = default);
    void Add(Reminder list);
    void Remove(string id);
}
