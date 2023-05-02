namespace WarrenSoft.Reminders.Domain;

public interface IEntitySet<TEntity> where TEntity : IEntity
{
    void Add(TEntity entity);
    Task<TEntity?> GetAsync(string id, string partitionKey, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void Remove(string id);
}