using KsuidDotNet;
using Warrensoft.Reminders.Domain;

namespace Warrensoft.Reminders.Infra;

public sealed class KsuidEntityIdentityProvider : IEntityIdentityProvider
{
    public string NextAccountPlanId() => Ksuid.NewKsuid("ap_");
    public string NextPlanId() => Ksuid.NewKsuid("p_");
    public string NextReminderId() => Ksuid.NewKsuid("r_");
    public string NextReminderListId() => Ksuid.NewKsuid("rl_");
}
