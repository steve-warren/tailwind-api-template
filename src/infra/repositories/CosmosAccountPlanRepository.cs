using Warrensoft.Reminders.Domain;
using WarrenSoft.Reminders.Domain;

namespace Warrensoft.Reminders.Infra;

public sealed class CosmosAccountPlanRepository : IRepository<AccountPlan>
{
    private readonly EntitySet<AccountPlan> _plans;

    public CosmosAccountPlanRepository(CosmosContext cosmosContext) =>
        _plans = cosmosContext.AccountPlans.Entity<AccountPlan>(partitionKeySelector: accountPlan => accountPlan.Id);

    public void Add(AccountPlan entity) =>
        _plans.Add(entity);

    public Task<AccountPlan?> GetAsync(string id, CancellationToken cancellationToken = default) =>
        _plans.GetAsync(id, partitionKey: id, cancellationToken);

    public void Remove(string id)
    {
        throw new NotImplementedException();
    }
}
