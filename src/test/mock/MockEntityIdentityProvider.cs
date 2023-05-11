using Warrensoft.Reminders.Domain;

namespace mock;

public class MockEntityIdentityProvider : IEntityIdentityProvider
{
    public string NextAccountPlanId()
    {
        return "";
    }

    public string NextPlanId()
    {
        return "";
    }

    public string NextReminderId()
    {
        return "";
    }

    public string NextReminderListId()
    {
        return "";
    }
}
