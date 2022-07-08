namespace Migration.Toolkit.Common.Helpers;

public abstract record AlignResult<TA, TB, TKey, TResult>(TA? A, TB? B, TKey Key, TResult Result);

public record AlignResultMatchMapped<TA, TB, TKey, TResult>(TA A, TB B, TKey Key, TResult Result) : AlignResult<TA, TB, TKey, TResult>(A, B, Key, Result);
public record AlignResultMatchSame<TA, TB, TKey, TResult>(TA A, TB B, TKey Key, TResult Result) : AlignResult<TA, TB, TKey, TResult>(A, B, Key, Result);
public record AlignResultOnlyA<TA, TB, TKey, TResult>(TA A, TKey Key, TResult Result) : AlignResult<TA, TB, TKey, TResult>(A, default, Key, Result); 
public record AlignResultOnlyB<TA, TB, TKey, TResult>(TB B, TKey Key, TResult Result) : AlignResult<TA, TB, TKey, TResult>(default, B, Key, Result);
public record AlignFatalNoMatch<TA, TB, TKey, TResult>(TA A, TB B, TKey Key, string ErrorDescription) : AlignResult<TA, TB, TKey, TResult>(A, B, Key, default);



public abstract record SimpleAlignResult<TA, TB, TKey>(TA? A, TB? B, TKey Key);

public record SimpleAlignResultMatch<TA, TB, TKey>(TA A, TB B, TKey Key) : SimpleAlignResult<TA, TB, TKey>(A, B, Key);
public record SimpleAlignResultOnlyA<TA, TB, TKey>(TA A, TKey Key) : SimpleAlignResult<TA, TB, TKey>(A, default, Key); 
public record SimpleAlignResultOnlyB<TA, TB, TKey>(TB B, TKey Key) : SimpleAlignResult<TA, TB, TKey>(default, B, Key);
public record SimpleAlignFatalNoMatch<TA, TB, TKey>(TA A, TB B, TKey Key, string ErrorDescription) : SimpleAlignResult<TA, TB, TKey>(A, B, Key);