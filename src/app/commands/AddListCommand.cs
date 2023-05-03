using Microsoft.AspNetCore.Mvc;
using WarrenSoft.Reminders.Domain;

namespace WarrenSoft.Reminders.Http;

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
        [FromServices] IRepository<Plan> plans,
        [FromServices] IEntityIdentityProvider ids)
    {
        var plan = await plans.GetAsync(planId);
        if (plan is null)
            return new NotFoundResult();

        var reminderList = plan.CreateList(
            id: ids.NextReminderListId(),
            name: command.Name);

        return new OkObjectResult(reminderList.Id);
    }
}
