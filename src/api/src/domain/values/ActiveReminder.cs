namespace WarrenSoft.Reminders.Domain;

public sealed class ActiveReminder : IReminderState
{
    public const string Active = "Active";
    public string State => Active;
    public List<IDomainEvent> DomainEvents => new();

    public CompletedReminder Complete(DateTimeOffset completedOn) => new() { CompletedOn = completedOn };
}
