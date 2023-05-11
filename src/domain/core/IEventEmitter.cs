using System.Text.Json.Serialization;

namespace Warrensoft.Reminders.Domain;

public interface IEventEmitter
{
    [JsonIgnore]
    public List<IDomainEvent> DomainEvents { get; }
    public void ClearEvents() => DomainEvents.Clear();
}