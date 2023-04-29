using Microsoft.AspNetCore.Mvc;
using WarrenSoft.Reminders.Domain;

namespace WarrenSoft.Reminders.Http;

public sealed record AddReminderCommand(string ListId, string OwnerId, string Title, string Notes);

[ApiController]
public sealed class AddReminderCommandHandler
{
    [HttpPost("api/reminders")]
    public async Task<IActionResult> Handle(
        [FromBody] AddReminderCommand command,
        [FromServices] IReminderListRepository reminderLists,
        [FromServices] IReminderRepository reminders,
        [FromServices] IEntityIdentityProvider ids,
        CancellationToken cancellationToken)
    {
        var reminderList = await reminderLists.FindAsync(id: command.ListId, cancellationToken);

        if (reminderList.OwnedBy(command.OwnerId) is false)
            return new BadRequestResult();

        var reminder = reminderList.CreateReminder(id: ids.NextReminderId(), command.Title, command.Notes);

        reminders.Add(reminder);

        return new OkObjectResult(reminder);
    }
}
