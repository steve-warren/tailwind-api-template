namespace Warrensoft.Reminders.Domain.Events;

public sealed record ReminderListCreatedEvent(ReminderList ReminderList) : IDomainEvent { }