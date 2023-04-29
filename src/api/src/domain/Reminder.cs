namespace WarrenSoft.Reminders.Domain;

public class Reminder : IAggregateRoot, IEntity, IEventEmitter
{
    private object _state;

    private Reminder()
    {
        Id = null!;
        OwnerId = null!;
        ListId = null!;
        Title = null!;
        Notes = null!;
        State = null!;
        _state = null!;
    }

    public Reminder(string id, string ownerId, string listId, string title, string notes)
    {
        Id = id;
        OwnerId = ownerId;
        ListId = listId;
        Title = title;
        Notes = notes;

        var state = new ActiveReminder();
        State = state.State;
        _state = state;
    }

    public string Id { get; private set; }
    public string OwnerId { get; private set; }
    public string ListId { get; private set; }
    public string Title { get; private set; }
    public string Notes { get; private set; }
    public string State { get; private set; }
    List<IDomainEvent> IEventEmitter.DomainEvents => new();

    public void ChangeTitle(string title)
    {
        Title = title;
    }

    public void ChangeState(IReminderState state)
    {
        _state = state;
        State = state.State;
    }

    public bool OwnedBy(string ownerId) => OwnerId == ownerId;

    public static ActiveReminder? FromActive(Reminder reminder) => reminder._state as ActiveReminder;
    public static CompletedReminder? FromCompleted(Reminder reminder) => reminder._state as CompletedReminder;
}
