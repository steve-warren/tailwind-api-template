namespace WarrenSoft.Reminders.Infra;
public interface IContainerContext
{
    Task CommitAsync(CancellationToken cancellationToken = default);
}
