namespace Migration.Toolkit.Common.Helpers;

using System.Collections;

public class SimpleAligner<TLeft, TRight, TKey> : IEnumerator<SimpleAlignResult<TLeft?, TRight?, TKey>> where TLeft : class where TRight : class
{
        public delegate TKey? SelectKey<in T>(T? current);

        private readonly SelectKey<TLeft> _selectKeyA;
        private readonly SelectKey<TRight> _selectKeyB;
        private readonly bool _disposeEnumerators;
        private readonly IEnumerator<TLeft> _eA;
        private readonly IEnumerator<TRight> _eB;
        private readonly IEnumerator<TKey> _eK;
        
        public int Ordinal { get; private set; }

        private SimpleAligner(
            IEnumerator<TLeft> eA,
            IEnumerator<TRight> eB,
            IEnumerator<TKey> eK,
            SelectKey<TLeft> selectKeyA,
            SelectKey<TRight> selectKeyB,
            bool disposeEnumerators
        )
        {
            _selectKeyA = selectKeyA;
            _selectKeyB = selectKeyB;
            _disposeEnumerators = disposeEnumerators;
            _eA = eA;
            _eB = eB;
            _eK = eK;
            Current = new AlignDefault<TLeft?, TRight?, TKey>();
        }

        public static SimpleAligner<TLeft, TRight, TKey> Create(
            IEnumerator<TLeft> eA, 
            IEnumerator<TRight> eB, 
            IEnumerator<TKey> eK,
            SelectKey<TLeft> selectKeyA,
            SelectKey<TRight> selectKeyB,
            bool disposeEnumerators
            )
        {
            return new SimpleAligner<TLeft, TRight, TKey>(eA, eB, eK, selectKeyA, selectKeyB, disposeEnumerators);
        }

        private bool _hasA;
        private bool _hasB;
        private bool _hasK = true;

        private bool _firstMove = true;

        public bool MoveNext()
        {
            if (_firstMove)
            {
                _hasA = _eA.MoveNext();
                _hasB = _eB.MoveNext();
                _firstMove = false;
            }

            _hasK = _eK.MoveNext();
            if (!_hasK) return false;

            do
            {
                var keyA = _selectKeyA(_eA.Current);
                var keyB = _selectKeyB(_eB.Current);

                var matchA = _eA.Current != default && Equals(keyA, _eK.Current);
                var matchB = _eB.Current != default && Equals(keyB, _eK.Current);
                if (matchA && matchB)
                {
                    Current = new SimpleAlignResultMatch<TLeft?, TRight?, TKey>(_eA.Current, _eB.Current, _eK.Current);
                    
                    _hasA = _hasA && _eA.MoveNext();
                    _hasB = _hasB && _eB.MoveNext();
                    Ordinal++;
                    return _hasK;
                }

                if (matchA)
                {
                    Current = new SimpleAlignResultOnlyA<TLeft?, TRight?, TKey>(_eA.Current, _eK.Current);
                    
                    _hasA = _hasA && _eA.MoveNext();
                    Ordinal++;
                    return _hasK;
                }

                if (matchB)
                {
                    Current = new SimpleAlignResultOnlyB<TLeft?, TRight?, TKey>(_eB.Current, _eK.Current);
                    
                    _hasB = _hasB && _eB.MoveNext();
                    Ordinal++;
                    return _hasK;
                }

                if (!matchA && !matchB)
                {
                    Current = new SimpleAlignFatalNoMatch<TLeft?, TRight?, TKey>(_eA.Current, _eB.Current, _eK.Current, "AB.NOMATCH: possibly error / wrongly sorted, selected source enumerators.");
                    
                    Ordinal++;
                    return _hasK;
                }
            } while (_hasA || _hasB);

            return false;
        }

        public void Reset()
        {
            _eA.Reset();
            _eB.Reset();
            _eK.Reset();
        }

        public SimpleAlignResult<TLeft?, TRight?, TKey> Current { get; private set; }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            if (!_disposeEnumerators) return;
            _eA.Dispose();
            _eB.Dispose();
            _eK.Dispose();
        }
    }