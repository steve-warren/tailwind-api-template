using Microsoft.Azure.Cosmos;
using Warrensoft.Reminders.Domain;
using WarrenSoft.Reminders.Domain;

namespace Warrensoft.Reminders.Infra;

public class CosmosContext : IUnitOfWork
{
    private readonly CosmosClient _client;
    private readonly Lazy<CosmosContainerSet<Reminder>> _reminders;
    private readonly Lazy<CosmosContainerSet<Plan>> _plans;
    private readonly Lazy<CosmosContainerSet<AccountPlan>> _accountPlans;
    private readonly Lazy<CosmosContainerSet<ReminderList>> _reminderLists;
    private readonly List<IUnitOfWork> _containerUnitsOfWork = new();

    public CosmosContext(CosmosClient client, string databaseName)
    {
        _client = client;
        DatabaseName  = databaseName;

        _reminders = CreateContainerSet<Reminder>("reminders", r => r.ListId);
        _plans = CreateContainerSet<Plan>("plans", p => p.Id);
        _accountPlans = CreateContainerSet<AccountPlan>("accountPlans", ap => ap.Id);
        _reminderLists = CreateContainerSet<ReminderList>("reminderLists", rl => rl.OwnerId);
    }

    public string DatabaseName { get; }
    public CosmosContainerSet<Reminder> Reminders => _reminders.Value;
    public CosmosContainerSet<Plan> Plans => _plans.Value;
    public CosmosContainerSet<AccountPlan> AccountPlans => _accountPlans.Value;
    public CosmosContainerSet<ReminderList> ReminderLists => _reminderLists.Value;

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach(var unitOfWork in _containerUnitsOfWork)
            await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private Lazy<CosmosContainerSet<TDocument>> CreateContainerSet<TDocument>(string containerName, Func<TDocument,string?> partitionKeySelector) where TDocument : class, IAggregateRoot
    {
        var container = _client.GetContainer(DatabaseName, containerName);
        var list = _containerUnitsOfWork;

        return new Lazy<CosmosContainerSet<TDocument>>(() =>
        {
            var set = new CosmosContainerSet<TDocument>(container, partitionKeySelector);

            list.Add(set);

            return set;
        });
    }
}