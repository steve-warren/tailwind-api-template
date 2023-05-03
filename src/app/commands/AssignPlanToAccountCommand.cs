using Microsoft.AspNetCore.Mvc;
using Warrensoft.Reminders.Domain;
using WarrenSoft.Reminders.Domain;

namespace Warrensoft.Reminders.App;

public sealed record AssignPlanToAccountCommand(
    string AccountId,
    string PlanId);

[ApiController]
public sealed class AssignPlanToAccountCommandHandler
{
    [HttpPost("api/user/plans")]
    public async Task<IActionResult> HandleAsync(
        [FromBody] AssignPlanToAccountCommand command,
        [FromServices] IRepository<AccountPlan> accountPlans,
        [FromServices] IRepository<Plan> plans,
        [FromServices] IEntityIdentityProvider ids)
    {
        throw new NotImplementedException();
    }
}