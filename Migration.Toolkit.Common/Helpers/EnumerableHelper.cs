namespace Migration.Toolkit.Common.Helpers;

public class EnumerableHelper
{
    public static SimpleAligner<TLeft, TRight, TKey> CreateAligner<TLeft, TRight, TKey>(IEnumerable<TLeft> eA, IEnumerable<TRight> eB, IEnumerable<TKey> eK,
        SimpleAligner<TLeft, TRight, TKey>.SelectKey<TLeft> selectKeyA, SimpleAligner<TLeft, TRight, TKey>.SelectKey<TRight> selectKeyB, bool disposeEnumerators)
        where TLeft : class where TRight : class => SimpleAligner<TLeft, TRight, TKey>.Create(eA.GetEnumerator(), eB.GetEnumerator(), eK.GetEnumerator(), selectKeyA, selectKeyB, disposeEnumerators);
    
    public static DeferrableItemEnumerableWrapper<TEntity> CreateDeferrableItemWrapper<TEntity>(IEnumerable<TEntity> innerEnumerable, int maxRecurrenceLimit = 5)
    {
        return new DeferrableItemEnumerableWrapper<TEntity>(innerEnumerable, maxRecurrenceLimit);
    }
}