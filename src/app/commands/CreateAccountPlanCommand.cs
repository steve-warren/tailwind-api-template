using Microsoft.AspNetCore.Mvc;
using Warrensoft.Reminders.Domain;
using Warrensoft.Reminders.Domain;

namespace Warrensoft.Reminders.App;

public sealed record CreateAccountPlanCommand(
    string Name,
    string Description);

[ApiController]
public sealed class CreateAccountPlanCommandHandler
{
    [HttpPost("api/plans")]
    public IActionResult HandleAsync(
        [FromBody] CreateAccountPlanCommand command,
        [FromServices] IRepository<AccountPlan> accountPlans,
        [FromServices] IEntityIdentityProvider ids)
    {
        var plan = new AccountPlan(
            id: ids.NextAccountPlanId(),
            name: command.Name,
            description: command.Description);

        accountPlans.Add(plan);

        return new OkObjectResult(plan);
    }
}