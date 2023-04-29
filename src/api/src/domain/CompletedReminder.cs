namespace WarrenSoft.Reminders.Domain;

public sealed record CompletedReminder : IReminderState
{
    public const string Completed = "Completed";
    public DateTimeOffset CompletedOn { get; init; }
    public string State => Completed;
    public List<IDomainEvent> DomainEvents => new();

    public ActiveReminder Activate() => new();
}
