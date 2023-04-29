using Microsoft.AspNetCore.Mvc;
using WarrenSoft.Reminders.Domain;

namespace WarrenSoft.Reminders.Http;

public sealed record AddReminderCommand(string ListId, string OwnerId, string Title, string Notes);

[ApiController]
public sealed class AddReminderCommandHandler : ControllerBase
{
    [HttpPost("api/reminders")]
    public async Task<IActionResult> Handle(
        [FromBody] AddReminderCommand command,
        [FromServices] IReminderListRepository reminderLists,
        [FromServices] IReminderRepository reminders,
        [FromServices] IEntityIdentityProvider ids,
        [FromServices] IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var reminderList = await reminderLists.FindAsync(id: command.ListId, cancellationToken);

        if (reminderList.OwnedBy(command.OwnerId) is false)
            return BadRequest();

        var reminder = reminderList.CreateReminder(id: ids.NextReminderId(), command.Title, command.Notes);

        reminders.Add(reminder);

        await unitOfWork.CommitAsync(cancellationToken);

        return Ok(reminder);
    }
}
