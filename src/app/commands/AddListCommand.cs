using Microsoft.AspNetCore.Mvc;
using WarrenSoft.Reminders.Domain;

namespace WarrenSoft.Reminders.Http;

public sealed record AddListCommand(string OwnerId, string Name, string ColorId, string EmojiId);

[ApiController]
public sealed class AddListCommandHandler
{
    [HttpPost("api/lists")]
    public async Task<IActionResult> Handle(
        [FromBody] AddListCommand command,
        [FromServices] IReminderListRepository reminderLists,
        [FromServices] IEntityIdentityProvider ids,
        [FromServices] IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var plan = new Plan(
        id: "pl_123",
        ownerId: command.OwnerId,
        name: "My Plan",
        description: "My plan description",
        startsOn: DateTimeOffset.UtcNow,
        endsOn: DateTimeOffset.UtcNow.AddDays(7));

        var reminderList = plan.CreateList(
            id: ids.NextReminderListId(),
            name: command.Name);

        reminderLists.Add(reminderList);

        await unitOfWork.CommitAsync(cancellationToken);

        return new OkObjectResult(reminderList.Id);
    }
}
