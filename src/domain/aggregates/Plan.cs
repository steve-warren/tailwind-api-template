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
        int numberOfLists)
    {
        Id = id;
        PartitionKey = id;
        OwnerId = ownerId;
        Name = name;
        Description = description;
        NumberOfLists = numberOfLists;
    }

    public string Id { get; }
    public string OwnerId { get; }
    public string Name { get; }
    public string Description { get; }
    public int NumberOfLists { get; private set; }
    public string PartitionKey { get; }

    List<IDomainEvent> IEventEmitter.DomainEvents => _domainEvents;

    public ReminderList CreateList(string id, string name)
    {
        var list = new ReminderList(
            id: id,
            ownerId: OwnerId,
            name: name);

        ++NumberOfLists;

        _domainEvents.Add(new ReminderListCreatedEvent(list));

        return list;
    }
}
