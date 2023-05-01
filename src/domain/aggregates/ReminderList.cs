using System.Text.Json.Serialization;
using WarrenSoft.Reminders.Domain.Events;

namespace WarrenSoft.Reminders.Domain;

public class ReminderList : IAggregateRoot, IEntity, IEventEmitter
{
    [JsonIgnore]
    private readonly List<IDomainEvent> _domainEvents = new();

    public ReminderList(string id, string ownerId, string name)
    {
        Id = id;
        OwnerId = ownerId;
        Name = name;
    }

    public string Id { get; }
    public string OwnerId { get; }
    public string Name { get; }
    List<IDomainEvent> IEventEmitter.DomainEvents => _domainEvents;

    public Reminder CreateReminder(string id, string title, string notes, DateTimeOffset? dueOn, ReminderPriority? priority)
    {
        var reminder = new Reminder(id: id, ownerId: OwnerId, listId: Id, title, notes, dueOn, priority);
        
        _domainEvents.Add(new ReminderCreatedEvent(reminder));

        return reminder;
    }

    public bool OwnedBy(string ownerId) => OwnerId == ownerId;
}
