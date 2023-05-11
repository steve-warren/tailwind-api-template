using Microsoft.AspNetCore.Mvc;
using Warrensoft.Reminders.Infra;
using Warrensoft.Reminders.Domain;

namespace Warrensoft.Reminders.Http;

public sealed record AddListCommand(
    string OwnerId,
    string Name,
    string ColorId,
    string EmojiId);

[ApiController]
public sealed class AddListCommandHandler
{
    [HttpPost("api/user/plans/{planId}/lists")]
    public async Task<IActionResult> HandleAsync(
        [FromBody] AddListCommand command,
        [FromRoute] string planId,
        [FromServices] IPlanRepository plans,
        [FromServices] IEntityIdentityProvider ids,
        CancellationToken cancellationToken)
    {
        var plan = await plans.GetAsync(id: planId, partitionKey: command.OwnerId, cancellationToken);
        if (plan is null)
            return new NotFoundResult();

        var reminderList = plan.CreateList(
            id: ids.NextReminderListId(),
            name: command.Name);

        plans.Update(plan);

        return new OkObjectResult(reminderList.Id);
    }
}
