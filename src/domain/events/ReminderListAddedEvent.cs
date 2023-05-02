namespace WarrenSoft.Reminders.Domain.Events;

public sealed record ReminderListAddedEvent(ReminderList ReminderList) : IDomainEvent { }