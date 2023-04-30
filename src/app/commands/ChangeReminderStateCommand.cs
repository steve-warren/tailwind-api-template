using Microsoft.AspNetCore.Mvc;
using WarrenSoft.Reminders.Domain;

namespace WarrenSoft.Reminders.Http;

public sealed record ChangeReminderStateCommand(string OwnerId, string ReminderId, string NewState);

[ApiController]
public sealed class ChangeReminderStateCommandHandler
{
    [HttpPut("api/reminders")]
    public async Task<IActionResult> Handle(
        [FromBody] ChangeReminderStateCommand command,
        [FromServices] IReminderRepository reminders,
        CancellationToken cancellationToken)
    {
        var reminder = await reminders.GetByIdAsync(command.ReminderId, cancellationToken);

        if (reminder is null)
            return new NotFoundResult();

        if (reminder.OwnedBy(command.OwnerId) is false)
            return new ForbidResult();

        var result = command.NewState switch
        {
            ActiveReminder.Active => ActivateReminder(reminder),
            CompletedReminder.Completed => CompleteReminder(reminder),
            _ => new BadRequestResult()
        };

        return result;
    }

    private static IActionResult CompleteReminder(Reminder reminder)
    {
        var activeReminder = Reminder.FromActive(reminder);

        if (activeReminder is null)
            return new BadRequestResult();

        var completedReminder = activeReminder.Complete(DateTimeOffset.Now);

        reminder.ChangeState(completedReminder);

        return new OkResult();
    }

    private static IActionResult ActivateReminder(Reminder reminder)
    {
        var activeReminder = Reminder.FromCompleted(reminder);

        if (activeReminder is null)
            return new BadRequestResult();

        var completedReminder = activeReminder.Activate();

        reminder.ChangeState(completedReminder);

        return new OkResult();
    }
}
