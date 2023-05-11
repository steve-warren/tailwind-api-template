namespace Warrensoft.Reminders.Domain;

public interface IRepository<TEntity> where TEntity : IEntity
{
    ValueTask<TEntity?> GetAsync(string id, string partitionKey, CancellationToken cancellationToken = default);
    void Add(TEntity entity);
    void Remove(TEntity id);
    void Update(TEntity entity);

    IQueryable<TEntity> AsQueryable();
    IAsyncEnumerable<TEntity> QueryAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> query);
}