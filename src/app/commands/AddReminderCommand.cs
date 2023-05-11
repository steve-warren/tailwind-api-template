using Microsoft.AspNetCore.Mvc;
using Warrensoft.Reminders.Infra;
using Warrensoft.Reminders.Domain;

namespace Warrensoft.Reminders.Http;

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
    public async Task<IActionResult> HandleAsync(
        [FromBody] AddReminderCommand command,
        [FromServices] CosmosContext context,
        [FromServices] IEntityIdentityProvider ids,
        CancellationToken cancellationToken)
    {
        var reminderList = await context.ReminderLists.GetAsync(id: command.ListId, partitionKey: command.ListId, cancellationToken);

        if (reminderList is null)
            return new BadRequestResult();

        var reminder = reminderList.CreateReminder(id: ids.NextReminderId(), command.Title, command.Notes, command.DueOn, command.Priority);

        context.Reminders.Add(reminder);

        return new OkObjectResult(reminder);
    }
}
