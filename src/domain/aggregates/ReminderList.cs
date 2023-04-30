using WarrenSoft.Reminders.Domain.Events;

namespace WarrenSoft.Reminders.Domain;

public class ReminderList : IAggregateRoot, IEntity, IEventEmitter
{
    private readonly IEventEmitter _eventEmitter;

    public ReminderList(string id, string ownerId, string name)
    {
        Id = id;
        OwnerId = ownerId;
        Name = name;
        _eventEmitter = this;
    }

    public string Id { get; }
    public string OwnerId { get; }
    public string Name { get; }
    List<IDomainEvent> IEventEmitter.DomainEvents => new();

    public Reminder CreateReminder(string id, string title, string notes, DateTimeOffset? dueOn, ReminderPriority? priority)
    {
        var reminder = new Reminder(id: id, ownerId: OwnerId, listId: Id, title, notes, dueOn, priority);
        
        _eventEmitter.AddEvent(new ReminderCreatedEvent(reminder));
        
        return reminder;
    }

    public bool OwnedBy(string ownerId) => OwnerId == ownerId;
}
