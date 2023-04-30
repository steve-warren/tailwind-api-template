namespace WarrenSoft.Reminders.Domain;

public class Reminder : IAggregateRoot, IEntity, IEventEmitter
{
    // store the state as an object so JSON serializer serializes the actual type and not the interface members.
    private object _state = new();

    private Reminder() { }

    public Reminder(string id, string ownerId, string listId, string title, string notes, DateTimeOffset? dueOn, ReminderPriority? priority) => 
        (Id, OwnerId, ListId, Title, Notes, State, DueOn, Priority, _state) =
        (id, ownerId, listId, title, notes, ActiveReminder.Active, dueOn, priority ?? ReminderPriority.None, new());

    public string Id { get; private set; } = "";
    public string OwnerId { get; private set; } = "";
    public string ListId { get; private set; } = "";
    public string Title { get; private set; } = "";
    public string Notes { get; private set; } = "";
    public string State { get; private set; } = "";
    public DateTimeOffset? DueOn { get; private set; }
    public ReminderPriority Priority { get; private set; } = null!;
    List<IDomainEvent> IEventEmitter.DomainEvents => new();

    public void ChangeTitle(string title)
    {
        Title = title;
    }

    internal void ChangeState(IReminderState state)
    {
        _state = state;
        State = state.State;
    }

    /// <summary>
    /// Checks whether the reminder is owned by the specified owner id.
    /// </summary>
    /// <param name="ownerId"></param>
    /// <returns></returns>
    public bool OwnedBy(string ownerId) => OwnerId == ownerId;

    /// <summary>
    /// Factory method which attempts to create an <see cref="ActiveReminder"/> state object from a <see cref="Reminder"/> object in the active state.
    /// </summary>
    /// <param name="reminder"></param>
    /// <returns></returns>
    public static ActiveReminder? FromActive(Reminder reminder) => reminder._state as ActiveReminder;

    /// <summary>
    /// Factory method which attempts to create a <see cref="CompletedReminder"/> state object from a <see cref="Reminder"/> object in the completed state.
    /// </summary>
    /// <param name="reminder"></param>
    /// <returns></returns>
    public static CompletedReminder? FromCompleted(Reminder reminder) => reminder._state as CompletedReminder;
}
