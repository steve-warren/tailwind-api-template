namespace WarrenSoft.Reminders.Domain;

public interface IEntityContext
{
    Task CommitAsync(CancellationToken cancellationToken = default);
}
