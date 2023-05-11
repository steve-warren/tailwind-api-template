namespace Warrensoft.Reminders.Domain.Events;

public sealed record ReminderCreatedEvent(Reminder Reminder, string? Id = null, DateTimeOffset? OccurredOn = null) : IDomainEvent { }
