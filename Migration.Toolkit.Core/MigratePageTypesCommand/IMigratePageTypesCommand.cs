using Migration.Toolkit.Core.Abstractions;

namespace Migration.Toolkit.Core.MigratePageTypesCommand;

public interface IMigratePageTypesCommand: ICommand
{
    public static string Moniker => "page-types";
}