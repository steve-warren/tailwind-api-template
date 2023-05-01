namespace WarrenSoft.Reminders.Domain.Events;

public sealed record ReminderListAddedEvent(ReminderList ReminderList, string? Id = null, DateTimeOffset? OccurredOn = null) : IDomainEvent { }