using System.Text.Json.Serialization;
using WarrenSoft.Reminders.Domain.Events;

namespace WarrenSoft.Reminders.Domain;

public class Plan : IAggregateRoot, IEntity, IEventEmitter
{
    [JsonIgnore]
    private readonly List<IDomainEvent> _domainEvents = new();

    public Plan(
        string id,
        string ownerId,
        string name,
        string description,
        DateTimeOffset startsOn,
        DateTimeOffset endsOn)
    {
        Id = id;
        OwnerId = ownerId;
        Name = name;
        Description = description;
        StartsOn = startsOn;
        EndsOn = endsOn;
    }

    public string Id { get; }
    public string OwnerId { get; }
    public string Name { get; }
    public string Description { get; }
    public DateTimeOffset StartsOn { get; }
    public DateTimeOffset EndsOn { get; }

    List<IDomainEvent> IEventEmitter.DomainEvents => _domainEvents;

    public ReminderList CreateList(string id, string name)
    {
        var list = new ReminderList(
            id: id,
            ownerId: OwnerId,
            name: name);

        return list;
    }
}
