using Microsoft.AspNetCore.Mvc;
using WarrenSoft.Reminders.Domain;

namespace WarrenSoft.Reminders.Http;

public sealed record AddListCommand(string OwnerId, string Name, string ColorId, string EmojiId);

[ApiController]
public sealed class AddListCommandHandler : ControllerBase
{
    [HttpPost("api/reminders")]
    public async Task<IActionResult> Handle(
        [FromBody] AddListCommand command,
        [FromServices] IReminderListRepository reminderLists,
        [FromServices] IEntityIdentityProvider ids,
        [FromServices] IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var reminderList = new ReminderList(id: ids.NextReminderListId(), command.OwnerId, command.Name);

        reminderLists.Add(reminderList);

        await unitOfWork.CommitAsync(cancellationToken);

        return Ok();
    }
}
