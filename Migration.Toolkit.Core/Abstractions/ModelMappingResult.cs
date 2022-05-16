namespace Migration.Toolkit.Core.Abstractions;

public abstract record ModelMappingResult<TResult>(TResult? Item, bool Success, string Message, bool NewInstance);

public record ModelMappingSuccess<TResult>(TResult? Result, bool NewInstance) : ModelMappingResult<TResult>(Result, true, null, NewInstance);

public record ModelMappingFailed<TResult>(string Message) : ModelMappingResult<TResult>(default, false, Message, false);
public record ModelMappingFailedKeyMismatch<TResult>() : ModelMappingResult<TResult>(default, false, $"Entity Guid mismatch, cannot map entity {typeof(TResult).FullName}", false);
public record ModelMappingFailedSourceNotDefined<TResult>() : ModelMappingResult<TResult>(default, false, $"Source entity is not defined for target {typeof(TResult).FullName}", false);