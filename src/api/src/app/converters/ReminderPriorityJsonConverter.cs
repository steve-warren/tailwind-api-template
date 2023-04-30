using System.Text.Json;
using System.Text.Json.Serialization;
using WarrenSoft.Reminders.Domain;

namespace WarrenSoft.Reminders.App.TypeConverters;

public sealed class ReminderPriorityJsonConverter : JsonConverter<ReminderPriority>
{
    public override ReminderPriority Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => ReminderPriority.From(reader.GetString());

    public override void Write(Utf8JsonWriter writer, ReminderPriority value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.Value);
}