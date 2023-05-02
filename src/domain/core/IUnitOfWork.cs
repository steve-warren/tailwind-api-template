namespace WarrenSoft.Reminders.Domain;

public interface IUnitOfWork
{
    void Register(IEntity entity);
    Task<TEntity?> GetAsync<TEntity>(string id, CancellationToken cancellationToken = default) where TEntity : IEntity;
    Task CommitAsync(CancellationToken cancellationToken = default);
}
