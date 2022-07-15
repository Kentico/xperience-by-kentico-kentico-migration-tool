namespace Migration.Toolkit.Core.Abstractions;

public abstract record CommandResult();

public record GenericCommandResult(): CommandResult;
public record CommandFailureResult(): CommandResult;