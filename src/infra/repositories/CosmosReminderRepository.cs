using Warrensoft.Reminders.Infra;
using WarrenSoft.Reminders.Domain;

namespace WarrenSoft.Reminders.Infra;

public sealed class CosmosReminderRepository : IRepository<Reminder>
{
    private readonly EntitySet<Reminder> _reminders;

    public CosmosReminderRepository(CosmosContext cosmosContext) =>
        _reminders = cosmosContext.Reminders.Entity<Reminder>(partitionKeySelector: reminder => reminder.Id);

    public void Add(Reminder reminder) =>
        _reminders.Add(reminder);

    public Task<Reminder?> GetAsync(string id, CancellationToken cancellationToken = default) =>
        _reminders.GetAsync(id, partitionKey : id, cancellationToken);

    public void Remove(string id)
    {
        throw new NotImplementedException();
    }
}
