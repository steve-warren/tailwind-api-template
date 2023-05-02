using WarrenSoft.Reminders.Domain;

namespace WarrenSoft.Reminders.Infra;

public sealed class CosmosReminderRepository : IReminderRepository
{
    private readonly CosmosEntitySet<Reminder> _reminders;

    public CosmosReminderRepository(CosmosContainerContext context)
    {
        _reminders = context.Reminders;
    }

    public void Add(Reminder reminder) =>
        _reminders.Add(reminder);

    public Task<Reminder?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
        _reminders.GetAsync(id, partitionKey : id, cancellationToken);

    public void Remove(string id)
    {
        throw new NotImplementedException();
    }
}
