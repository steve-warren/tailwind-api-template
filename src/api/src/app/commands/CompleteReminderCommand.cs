using Microsoft.AspNetCore.Mvc;
using WarrenSoft.Reminders.Domain;

namespace WarrenSoft.Reminders.Http;

public sealed record CompleteReminderCommand(string OwnerId, string ReminderId);

[ApiController]
public sealed class CompleteReminderCommandHandler : ControllerBase
{
    [HttpPost("api/reminders")]
    public async Task<IActionResult> Handle(
        [FromBody] CompleteReminderCommand command,
        [FromServices] IReminderRepository reminders,
        [FromServices] IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var reminder = await reminders.GetByIdAsync(id: command.ReminderId, cancellationToken);

        if (reminder is null)
            return BadRequest();

        if (reminder.OwnedBy(command.OwnerId) is false)
            return BadRequest();

        var active = Reminder.FromActive(reminder);
        var completed = active.Complete(DateTimeOffset.Now);

        Reminder.ReplaceState(reminder, completed);

        await unitOfWork.CommitAsync(cancellationToken);

        return Ok();
    }
}
