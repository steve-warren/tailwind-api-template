using WarrenSoft.Reminders.Domain;

namespace WarrenSoft.Reminders.Infra;

public sealed class CosmosContainerContext : IContainerContext
{
    private readonly CosmosUnitOfWork _unitOfWork;

    public CosmosContainerContext(CosmosUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        ReminderLists = new CosmosEntitySet<ReminderList>(unitOfWork);
        Reminders = new CosmosEntitySet<Reminder>(unitOfWork);

        _unitOfWork.MapPartitionKey<ReminderList>(list => list.Id);
        _unitOfWork.MapPartitionKey<Reminder>(reminder => reminder.Id);
    }

    public CosmosEntitySet<ReminderList> ReminderLists { get; private set; } = null!;
    public CosmosEntitySet<Reminder> Reminders { get; private set; } = null!;

    public Task CommitAsync(CancellationToken cancellationToken = default) =>
        _unitOfWork.CommitAsync(cancellationToken);
}