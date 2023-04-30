using KsuidDotNet;
using WarrenSoft.Reminders.Domain;

namespace WarrenSoft.Reminders.Infra;

public sealed class KsuidEntityIdentityProvider : IEntityIdentityProvider
{
    public string NextReminderId() => Ksuid.NewKsuid("r_");
    public string NextReminderListId() => Ksuid.NewKsuid("rl_");
}
