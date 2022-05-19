using MediatR;
using Migration.Toolkit.Core.Abstractions;

namespace Migration.Toolkit.Core;

public record MigrateContactGroupsCommand(bool Dry): IRequest<GenericCommandResult>
{
    public static string Moniker => "contact-groups";
    public static string MonikerFriendly => "Contact groups";
}

public record MigrateContactManagementCommand(bool Dry): IRequest<GenericCommandResult>
{
    public static string Moniker => "contact-management";
    public static string MonikerFriendly => "Contact management";
}

public record MigrateDataProtectionCommand(bool Dry): IRequest<GenericCommandResult>
{
    public static string Moniker => "data-protection";
    public static string MonikerFriendly => "Data protection";
}

public record MigrateFormsCommand(bool Dry) : IRequest<GenericCommandResult>
{
    public static string Moniker => "forms";
    public static string MonikerFriendly => "Forms";
}

public record MigrateMediaLibrariesCommand(bool Dry): IRequest<GenericCommandResult>
{
    public static string Moniker => "media-libraries";
    public static string MonikerFriendly => "Media libraries";
}

public record MigratePageTypesCommand(bool Dry): IRequest<MigratePageTypesResult>
{
    public static string Moniker => "page-types";
    public static string MonikerFriendly => "Page types";
}

public record MigratePagesCommand(bool Dry, string CultureCode): IRequest<GenericCommandResult>
{
    public static string Moniker => "pages";
    public static string MonikerFriendly => "Pages";
}

public record MigrateSettingKeysCommand(bool Dry) : IRequest<MigrateSettingsKeysResult>
{
    public static string Moniker => "settings-keys";
    public static string MonikerFriendly => "Settings keys";
}

public record MigrateSitesCommand(bool Dry): IRequest<GenericCommandResult>
{
    public static string Moniker => "sites";
    public static string MonikerFriendly => "Sites";
}

public record MigrateUsersCommand(bool Dry): IRequest<GenericCommandResult>
{
    public static string Moniker => "users";
    public static string MonikerFriendly => "Users";
}

public record MigrateWebFarmsCommand(bool Dry): IRequest<GenericCommandResult>
{
    public static string Moniker => "web-farms";
    public static string MonikerFriendly => "Web farms";
}