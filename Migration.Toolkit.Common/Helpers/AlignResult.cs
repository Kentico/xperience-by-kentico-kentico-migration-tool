namespace Migration.Toolkit.Common.Helpers;

public abstract record SimpleAlignResult<TLeft, TRight, TKey>(TLeft? A, TRight? B, TKey? Key);

public record SimpleAlignResultMatch<TLeft, TRight, TKey>(TLeft A, TRight B, TKey Key) : SimpleAlignResult<TLeft, TRight, TKey>(A, B, Key);
public record SimpleAlignResultOnlyA<TLeft, TRight, TKey>(TLeft A, TKey Key) : SimpleAlignResult<TLeft, TRight, TKey>(A, default, Key); 
public record SimpleAlignResultOnlyB<TLeft, TRight, TKey>(TRight B, TKey Key) : SimpleAlignResult<TLeft, TRight, TKey>(default, B, Key);
public record SimpleAlignFatalNoMatch<TLeft, TRight, TKey>(TLeft A, TRight B, TKey Key, string ErrorDescription) : SimpleAlignResult<TLeft, TRight, TKey>(A, B, Key);
public record AlignDefault<TLeft, TRight, TKey>(): SimpleAlignResult<TLeft?, TRight?, TKey?>(default, default, default);