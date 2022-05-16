using Migration.Toolkit.Core.Abstractions;

namespace Migration.Toolkit.Core.MigrateSettingKeysCommand;

public interface IMigrateSettingKeysCommand: ICommand
{
    public static string Moniker => "setting-keys";
}