using System.Diagnostics.CodeAnalysis;

namespace WarrenSoft.Reminders.Domain;

public sealed class EntityIdentityEqualityComparer : IEqualityComparer<IEntity>
{
    public static readonly EntityIdentityEqualityComparer Instance = new();
    
    public bool Equals(IEntity? x, IEntity? y) =>
        string.Equals(x?.Id, y?.Id, StringComparison.Ordinal);

    public int GetHashCode([DisallowNull] IEntity obj) =>
        obj.Id.GetHashCode();
}