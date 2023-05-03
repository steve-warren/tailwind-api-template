using WarrenSoft.Reminders.Domain;

namespace WarrenSoft.Reminders.Infra;

public sealed class CosmosReminderRepository : IReminderRepository
{
    private readonly EntitySet<Reminder> _reminders;

    public CosmosReminderRepository(CosmosUnitOfWork unitOfWork) =>
        _reminders = unitOfWork.Set<Reminder>(reminder => reminder.Id);

    public void Add(Reminder reminder) =>
        _reminders.Add(reminder);

    public Task<Reminder?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
        _reminders.GetAsync(id, partitionKey : id, cancellationToken);

    public void Remove(string id)
    {
        throw new NotImplementedException();
    }
}
