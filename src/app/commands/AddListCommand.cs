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
        CancellationToken cancellationToken)
    {
        var reminderList = new ReminderList(id: ids.NextReminderListId(), command.OwnerId, command.Name);

        reminderLists.Add(reminderList);

        return new OkObjectResult(reminderList.Id);
    }
}
