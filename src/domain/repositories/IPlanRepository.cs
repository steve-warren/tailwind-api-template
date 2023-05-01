namespace WarrenSoft.Reminders.Domain;

public interface IPlanRepository
{
    Task<Plan?> GetByIdAsync(string ownerId, CancellationToken cancellationToken = default);
    void Add(Plan plan);
    void Remove(string id);
}