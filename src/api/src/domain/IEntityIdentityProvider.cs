namespace WarrenSoft.Reminders.Domain;

public interface IEntityIdentityProvider
{
    string NextReminderId();
    string NextReminderListId();
}