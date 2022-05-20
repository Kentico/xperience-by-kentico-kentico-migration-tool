namespace Migration.Toolkit.Core.Abstractions;

public abstract record CommandResult();

public record GenericCommandResult(): CommandResult;
// public record GenericCommandResultFailure(Exception Exception): CommandResult;

public record MigratePageTypesResult(): CommandResult();
public record MigrateSettingsKeysResult(): CommandResult();