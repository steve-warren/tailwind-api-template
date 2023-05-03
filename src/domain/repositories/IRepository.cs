namespace WarrenSoft.Reminders.Domain;

public interface IRepository<TEntity> where TEntity : IEntity
{
    Task<TEntity?> GetAsync(string id, CancellationToken cancellationToken = default);
    void Add(TEntity entity);
    void Remove(string id);
}