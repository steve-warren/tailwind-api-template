using System.Text.Json.Serialization;

namespace WarrenSoft.Reminders.Domain;

public interface IEntity
{
    [JsonPropertyName("id")]
    string Id { get; }
}
