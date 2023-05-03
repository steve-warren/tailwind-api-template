using Warrensoft.Reminders.Infra;
using WarrenSoft.Reminders.Domain;

namespace WarrenSoft.Reminders.Infra;

public class CosmosPlanRepository : IRepository<Plan>
{
    private readonly EntitySet<Plan> _plans;
    public CosmosPlanRepository(CosmosContext cosmosContext) =>
        _plans = cosmosContext.Plans.Entity<Plan>(partitionKeySelector: list => list.Id);

    public void Add(Plan plan) =>
        _plans.Add(plan);

    public Task<Plan?> GetAsync(string id, CancellationToken cancellationToken = default) =>
        _plans.GetAsync(id, partitionKey: id, cancellationToken);

    public void Remove(string id)
    {
        throw new NotImplementedException();
    }
}
