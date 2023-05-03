using WarrenSoft.Reminders.Domain;

namespace WarrenSoft.Reminders.Domain;

public class EntitySet<TEntity> : IEntitySet<TEntity> where TEntity : IEntity
{
    private readonly IUnitOfWork _unitOfWork;

    public EntitySet(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public void Add(TEntity entity)
    {
        _unitOfWork.Register(entity);
    }

    public Task<TEntity?> GetAsync(string id, string partitionKey, CancellationToken cancellationToken = default) =>
        _unitOfWork.GetAsync<TEntity>(id, partitionKey, cancellationToken);

    public void Remove(string id)
    {
        throw new NotImplementedException();
    }

    public void Update(TEntity entity)
    {
        throw new NotImplementedException();
    }
}
