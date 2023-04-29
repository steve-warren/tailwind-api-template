namespace WarrenSoft.Reminders.Domain;

public interface IReminderRepository
{
    string NextIdentity();
    Task<Reminder?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    void Add(Reminder list);
    void Replace(Reminder list);
    void Remove(string id);
}