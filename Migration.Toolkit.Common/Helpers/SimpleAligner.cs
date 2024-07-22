using System.Collections;

namespace Migration.Toolkit.Common.Helpers;

public class SimpleAligner<TLeft, TRight, TKey> : IEnumerator<SimpleAlignResult<TLeft?, TRight?, TKey>> where TLeft : class where TRight : class
{
    public delegate TKey? SelectKey<in T>(T? current);

    private readonly bool disposeEnumerators;
    private readonly IEnumerator<TLeft> eA;
    private readonly IEnumerator<TRight> eB;
    private readonly IEnumerator<TKey> eK;

    private readonly SelectKey<TLeft> selectKeyA;
    private readonly SelectKey<TRight> selectKeyB;

    private bool firstMove = true;

    private bool hasA;
    private bool hasB;
    private bool hasK = true;

    private SimpleAligner(
        IEnumerator<TLeft> eA,
        IEnumerator<TRight> eB,
        IEnumerator<TKey> eK,
        SelectKey<TLeft> selectKeyA,
        SelectKey<TRight> selectKeyB,
        bool disposeEnumerators
    )
    {
        this.selectKeyA = selectKeyA;
        this.selectKeyB = selectKeyB;
        this.disposeEnumerators = disposeEnumerators;
        this.eA = eA;
        this.eB = eB;
        this.eK = eK;
        Current = new AlignDefault<TLeft?, TRight?, TKey>()!;
    }

    public int Ordinal { get; private set; }

    public bool MoveNext()
    {
        if (firstMove)
        {
            hasA = eA.MoveNext();
            hasB = eB.MoveNext();
            firstMove = false;
        }

        hasK = eK.MoveNext();
        if (!hasK)
        {
            return false;
        }

        do
        {
            var keyA = selectKeyA(eA.Current);
            var keyB = selectKeyB(eB.Current);

            bool matchA = eA.Current != default && Equals(keyA, eK.Current);
            bool matchB = eB.Current != default && Equals(keyB, eK.Current);
            if (matchA && matchB)
            {
                Current = new SimpleAlignResultMatch<TLeft?, TRight?, TKey>(eA.Current, eB.Current, eK.Current);

                hasA = hasA && eA.MoveNext();
                hasB = hasB && eB.MoveNext();
                Ordinal++;
                return hasK;
            }

            if (matchA)
            {
                Current = new SimpleAlignResultOnlyA<TLeft?, TRight?, TKey>(eA.Current, eK.Current);

                hasA = hasA && eA.MoveNext();
                Ordinal++;
                return hasK;
            }

            if (matchB)
            {
                Current = new SimpleAlignResultOnlyB<TLeft?, TRight?, TKey>(eB.Current, eK.Current);

                hasB = hasB && eB.MoveNext();
                Ordinal++;
                return hasK;
            }

            if (!matchA && !matchB)
            {
                Current = new SimpleAlignFatalNoMatch<TLeft?, TRight?, TKey>(eA.Current, eB.Current, eK.Current, "AB.NOMATCH: possibly error / wrongly sorted, selected source enumerators.");

                Ordinal++;
                return hasK;
            }
        } while (hasA || hasB);

        return false;
    }

    public void Reset()
    {
        eA.Reset();
        eB.Reset();
        eK.Reset();
    }

    public SimpleAlignResult<TLeft?, TRight?, TKey> Current { get; private set; }

    object IEnumerator.Current => Current;

    public void Dispose()
    {
        if (!disposeEnumerators)
        {
            return;
        }

        eA.Dispose();
        eB.Dispose();
        eK.Dispose();
    }

    public static SimpleAligner<TLeft, TRight, TKey> Create(
        IEnumerator<TLeft> eA,
        IEnumerator<TRight> eB,
        IEnumerator<TKey> eK,
        SelectKey<TLeft> selectKeyA,
        SelectKey<TRight> selectKeyB,
        bool disposeEnumerators
    ) => new(eA, eB, eK, selectKeyA, selectKeyB, disposeEnumerators);
}
