namespace WarrenSoft.Reminders.Domain;

public interface IReminderListRepository
{
    Task<ReminderList?> FindAsync(string id, CancellationToken cancellationToken = default);
    void Add(ReminderList list);
    void Replace(ReminderList list);
    void Remove(string id);
}