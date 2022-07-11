namespace Migration.Toolkit.Common.Helpers;

public class EnumerableHelper
{
    public static SimpleAligner<TA, TB, TKey> CreateAligner<TA, TB, TKey>(IEnumerable<TA> eA, IEnumerable<TB> eB, IEnumerable<TKey> eK,
        SimpleAligner<TA, TB, TKey>.SelectKey<TA> selectKeyA, SimpleAligner<TA, TB, TKey>.SelectKey<TB> selectKeyB, bool disposeEnumerators)
        where TA : class where TB : class => SimpleAligner<TA, TB, TKey>.Create(eA.GetEnumerator(), eB.GetEnumerator(), eK.GetEnumerator(), selectKeyA, selectKeyB, disposeEnumerators);
    
    public static DeferrableItemEnumerableWrapper<TEntity> CreateDeferrableItemWrapper<TEntity>(IEnumerable<TEntity> innerEnumerable, int maxRecurrenceLimit = 5)
    {
        return new DeferrableItemEnumerableWrapper<TEntity>(innerEnumerable, maxRecurrenceLimit);
    }
}