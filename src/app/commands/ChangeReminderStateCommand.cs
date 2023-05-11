using Microsoft.AspNetCore.Mvc;
using Warrensoft.Reminders.Infra;
using Warrensoft.Reminders.Domain;

namespace Warrensoft.Reminders.Http;

public sealed record ChangeReminderStateCommand(string OwnerId, string ReminderId, string NewState);

[ApiController]
public sealed class ChangeReminderStateCommandHandler
{
    [HttpPut("api/reminders")]
    public async Task<IActionResult> Handle(
        [FromBody] ChangeReminderStateCommand command,
        [FromServices] CosmosContext context,
        CancellationToken cancellationToken)
    {
        var reminder = await context.Reminders.GetAsync(command.ReminderId, command.OwnerId, cancellationToken);

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
