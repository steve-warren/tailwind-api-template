namespace WarrenSoft.Reminders.Domain;

public interface IDomainEvent
{
    string? Id { get; }
    DateTimeOffset? OccurredOn { get; }
}
