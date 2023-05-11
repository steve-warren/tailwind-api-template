using Microsoft.Azure.Cosmos.Linq;

namespace Warrensoft.Reminders.Infra;

public static class CosmosExtensions
{
    public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> enumerable, CancellationToken cancellationToken = default)
    {
        var list = new List<T>();

        await foreach(var item in enumerable.WithCancellation(cancellationToken))
            list.Add(item);

        return list;
    }
}