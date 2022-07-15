namespace Migration.Toolkit.Common.Helpers;

public abstract record SimpleAlignResult<TA, TB, TKey>(TA? A, TB? B, TKey Key);

public record SimpleAlignResultMatch<TA, TB, TKey>(TA A, TB B, TKey Key) : SimpleAlignResult<TA, TB, TKey>(A, B, Key);
public record SimpleAlignResultOnlyA<TA, TB, TKey>(TA A, TKey Key) : SimpleAlignResult<TA, TB, TKey>(A, default, Key); 
public record SimpleAlignResultOnlyB<TA, TB, TKey>(TB B, TKey Key) : SimpleAlignResult<TA, TB, TKey>(default, B, Key);
public record SimpleAlignFatalNoMatch<TA, TB, TKey>(TA A, TB B, TKey Key, string ErrorDescription) : SimpleAlignResult<TA, TB, TKey>(A, B, Key);