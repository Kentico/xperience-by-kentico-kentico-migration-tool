using MediatR;
using Migration.Toolkit.Core.Abstractions;

namespace Migration.Toolkit.Core.Commands;

public record MigrateSettingKeysCommand() : IRequest<MigrateSettingsKeysResult>
{
    public static string Moniker => "setting-keys";
};