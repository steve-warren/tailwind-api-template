namespace WarrenSoft.Reminders.Domain;

public interface IUnitOfWork
{
    void Register(IEntity entity);
    Task<TEntity?> GetAsync<TEntity>(string id, string partitionKey, CancellationToken cancellationToken = default) where TEntity : IEntity;
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    EntitySet<TEntity> Entity<TEntity>() where TEntity : IEntity;
}
