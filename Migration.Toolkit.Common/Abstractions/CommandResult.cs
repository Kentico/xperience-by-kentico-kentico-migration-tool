namespace Migration.Toolkit.Common.Abstractions;

public abstract record CommandResult();

public record GenericCommandResult(): CommandResult;
public record CommandFailureResult(): CommandResult;
public record CommandCheckFailedResult(bool CanContinue): CommandResult;