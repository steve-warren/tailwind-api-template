using WarrenSoft.Reminders.Domain;

namespace mock;

public class MockEntityIdentityProvider : IEntityIdentityProvider
{
    public string NextReminderId()
    {
        return "";
    }

    public string NextReminderListId()
    {
        return "";
    }
}
