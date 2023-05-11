using System.Text.Json.Serialization;

namespace Warrensoft.Reminders.Domain;

public interface IEntity
{
    [JsonPropertyName("id")]
    string Id { get; }
}
