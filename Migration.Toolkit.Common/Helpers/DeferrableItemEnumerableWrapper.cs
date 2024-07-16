using System.Runtime.CompilerServices;

namespace Migration.Toolkit.Common.Helpers;

public class DeferrableItemEnumerableWrapper<T>(IEnumerable<T> innerEnumerable, int maxRecurrenceLimit = 5) : IDisposable
{
    private readonly IEnumerator<T> _innerEnumerator = innerEnumerable.GetEnumerator();

    public record DeferrableItem(int Recurrence, T Item);

    private readonly Queue<DeferrableItem> _deferredItems = new();

    public bool GetNext(out DeferrableItem item)
    {
        if (_innerEnumerator.MoveNext())
        {
            item = new DeferrableItem(0, _innerEnumerator.Current);
            return true;
        }

        if (_deferredItems.TryDequeue(out var deferred))
        {
            item = deferred with
            {
                Recurrence = deferred.Recurrence + 1
            };
            return true;
        }

        Unsafe.SkipInit(out item);
        return false;
    }

    public bool TryDeferItem(DeferrableItem item)
    {
        if (item.Recurrence < maxRecurrenceLimit)
        {
            _deferredItems.Enqueue(item);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Dispose() => _innerEnumerator.Dispose();
}
