using System.Text.Json.Serialization;
using Warrensoft.Reminders.Domain;

namespace Warrensoft.Reminders.Domain;

public class AccountPlan : IAggregateRoot, IEntity, IEventEmitter
{
    [JsonIgnore]
    private readonly List<IDomainEvent> _domainEvents = new();

    public AccountPlan(
        string id,
        string name,
        string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public string Id { get; }
    public string Name { get; }
    public string Description { get; }

    List<IDomainEvent> IEventEmitter.DomainEvents => _domainEvents;
}