using System.Runtime.CompilerServices;

namespace Migration.Tool.Common.Helpers;

public class DeferrableItemEnumerableWrapper<T>(IEnumerable<T> innerEnumerable, int maxRecurrenceLimit = 5) : IDisposable
{
    private readonly Queue<DeferrableItem> deferredItems = new();
    private readonly IEnumerator<T> innerEnumerator = innerEnumerable.GetEnumerator();

    public void Dispose() => innerEnumerator.Dispose();

    public bool GetNext(out DeferrableItem item)
    {
        if (innerEnumerator.MoveNext())
        {
            item = new DeferrableItem(0, innerEnumerator.Current);
            return true;
        }

        if (deferredItems.TryDequeue(out var deferred))
        {
            item = deferred with { Recurrence = deferred.Recurrence + 1 };
            return true;
        }

        Unsafe.SkipInit(out item);
        return false;
    }

    public bool TryDeferItem(DeferrableItem item)
    {
        if (item.Recurrence < maxRecurrenceLimit)
        {
            deferredItems.Enqueue(item);
            return true;
        }

        return false;
    }

    public record DeferrableItem(int Recurrence, T Item);
}
