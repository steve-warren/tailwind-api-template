using Microsoft.Azure.Cosmos;
using WarrenSoft.Reminders.Domain;

namespace WarrenSoft.Reminders.Infra;

public class CosmosEntitySet<TEntity> : IEntitySet<TEntity> where TEntity : IEntity
{
    private readonly CosmosUnitOfWork _unitOfWork;

    public CosmosEntitySet(CosmosUnitOfWork unitOfWork)
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
