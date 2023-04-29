namespace WarrenSoft.Reminders.Domain;

public interface IEntityIdentityProvider
{
    /// <summary>
    /// Generates a new reminder id.
    /// </summary>
    /// <returns>A string representing a new reminder id.</returns>
    string NextReminderId();

    /// <summary>
    /// Generates a new reminder list id.
    /// </summary>
    /// <returns>A string representing a new reminder id.</returns>
    string NextReminderListId();
}