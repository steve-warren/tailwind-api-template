namespace WarrenSoft.Reminders.Domain;

public interface IUnitOfWork
{
    Task CommitAsync(CancellationToken cancellationToken = default);
}
