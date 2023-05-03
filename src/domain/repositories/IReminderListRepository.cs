namespace WarrenSoft.Reminders.Domain;

public interface IReminderListRepository
{
    Task<ReminderList?> GetAsync(string id, CancellationToken cancellationToken = default);
    void Add(ReminderList list);
    void Remove(string id);
}