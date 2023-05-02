using WarrenSoft.Reminders.Domain;

namespace WarrenSoft.Reminders.Infra;

public class InMemoryPlanRepository : IPlanRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public InMemoryPlanRepository(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public void Add(Plan plan)
    {
        _unitOfWork.Register(plan);
    }

    public Task<Plan?> GetByIdAsync(string ownerId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Remove(string id)
    {
        throw new NotImplementedException();
    }
}
