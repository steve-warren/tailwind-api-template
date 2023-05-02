using Microsoft.AspNetCore.Mvc;
using WarrenSoft.Reminders.Domain;

namespace WarrenSoft.Reminders.Http;

public sealed record AddReminderCommand(
    string ListId, 
    string OwnerId, 
    string Title, 
    string Notes,
    ReminderPriority Priority, 
    DateTimeOffset? DueOn);

[ApiController]
public sealed class AddReminderCommandHandler
{
    [HttpPost("api/reminders")]
    public async Task<IActionResult> Handle(
        [FromBody] AddReminderCommand command,
        [FromServices] IReminderListRepository reminderLists,
        [FromServices] IReminderRepository reminders,
        [FromServices] IUnitOfWork unitOfWork,
        [FromServices] IEntityIdentityProvider ids,
        CancellationToken cancellationToken)
    {
        var reminderList = await reminderLists.FindAsync(id: command.ListId, cancellationToken);

        if (reminderList is null)
            return new BadRequestResult();

        // if (reminderList.OwnedBy(command.OwnerId) is false)
        //     return new BadRequestResult();

        var reminder = reminderList.CreateReminder(id: ids.NextReminderId(), command.Title, command.Notes, command.DueOn, command.Priority);

        reminders.Add(reminder);

        await unitOfWork.CommitAsync(cancellationToken);

        return new OkObjectResult(reminder);
    }
}
