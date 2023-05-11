namespace Warrensoft.Reminders.Domain;

public sealed record ReminderPriority
{
    public static readonly ReminderPriority None = new(nameof(None));
    public static readonly ReminderPriority Low = new(nameof(Low));
    public static readonly ReminderPriority Medium = new(nameof(Medium));
    public static readonly ReminderPriority High = new(nameof(High));

    public ReminderPriority() => Value = nameof(None);
    private ReminderPriority(string value) => Value = value;
    public string Value { get; }

    public static ReminderPriority From(string? value)
        => value switch {
            nameof(None) => None,
            nameof(Low) => Low,
            nameof(Medium) => Medium,
            nameof(High) => High,
            _ => None
        };

    public static implicit operator string(ReminderPriority priority) => priority.Value;
    public static implicit operator ReminderPriority(string value) => From(value);
}