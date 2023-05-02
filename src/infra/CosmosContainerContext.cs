using Microsoft.Azure.Cosmos;
using WarrenSoft.Reminders.Domain;

namespace WarrenSoft.Reminders.Infra;

public sealed class CosmosContainerContext
{
    private readonly CosmosUnitOfWork _unitOfWork;

    public CosmosContainerContext(CosmosUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        ReminderLists = new CosmosEntitySet<ReminderList>(_unitOfWork);
        
        _unitOfWork.MapPartitionKey<ReminderList>(list => list.Id);
    }

    public CosmosEntitySet<ReminderList> ReminderLists { get; private set; } = null!;

    public Task CommitAsync(CancellationToken cancellationToken = default) =>
        _unitOfWork.CommitAsync(cancellationToken);
}