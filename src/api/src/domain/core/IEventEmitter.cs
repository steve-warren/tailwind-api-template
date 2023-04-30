using System.Text.Json.Serialization;

namespace WarrenSoft.Reminders.Domain;

public interface IEventEmitter
{
    [JsonIgnore]
    public List<IDomainEvent> DomainEvents { get; }
    public void AddEvent(IDomainEvent domainEvent) => DomainEvents.Add(domainEvent);
    public void ClearEvents() => DomainEvents.Clear();
}