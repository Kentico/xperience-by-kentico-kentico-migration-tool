namespace Migration.Toolkit.Core.Abstractions;

public abstract record CommandResult();

public record MigratePageTypesResult(): CommandResult();
public record MigrateSettingsKeysResult(): CommandResult();