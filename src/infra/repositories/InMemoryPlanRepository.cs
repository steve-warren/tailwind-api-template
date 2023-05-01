using WarrenSoft.Reminders.Domain;

namespace WarrenSoft.Reminders.Infra;

public class InMemoryPlanRepository : IPlanRepository
{
    public void Add(Plan plan)
    {
        throw new NotImplementedException();
    }

    public Task<Plan?> GetByIdAsync(string ownerId, CancellationToken cancellationToken = default)
    {
            return new Plan(
            id: "pl_123",
            ownerId: ownerId,
            name: "My Plan",
            description: "My plan description",
            startsOn: DateTimeOffset.UtcNow,
            endsOn: DateTimeOffset.UtcNow.AddDays(7))
    }

    public void Remove(string id)
    {
        throw new NotImplementedException();
    }
}
