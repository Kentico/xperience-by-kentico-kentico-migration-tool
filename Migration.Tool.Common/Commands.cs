using MediatR;

using Migration.Tool.Common.Abstractions;

namespace Migration.Tool.Common;

public record MigrateSitesCommand : IRequest<CommandResult>, ICommand
{
    public static readonly int Rank = 1;

    public static string Moniker => "sites";
    public static string MonikerFriendly => "Sites";

    public Type[] Dependencies => [];
}

public record MigrateUsersCommand : IRequest<CommandResult>, ICommand
{
    public static readonly int Rank = 1 + MigrateSitesCommand.Rank + MigrateCustomModulesCommand.Rank;

    public static string Moniker => "users";
    public static string MonikerFriendly => "Users";

    public Type[] Dependencies => [typeof(MigrateCustomModulesCommand)];
}

public record MigrateMembersCommand : IRequest<CommandResult>, ICommand
{
    public static readonly int Rank = 2 + MigrateSitesCommand.Rank + MigrateCustomModulesCommand.Rank;

    public static string Moniker => "members";
    public static string MonikerFriendly => "Members";

    public Type[] Dependencies => [typeof(MigrateCustomModulesCommand)];
}

public record MigrateContactManagementCommand : IRequest<CommandResult>, ICommand
{
    public static readonly int Rank = 1 + MigrateUsersCommand.Rank + MigrateCustomModulesCommand.Rank;

    public static string Moniker => "contact-management";
    public static string MonikerFriendly => "Contact management";

    public Type[] Dependencies => [typeof(MigrateUsersCommand), typeof(MigrateCustomModulesCommand)];
}

public record MigrateDataProtectionCommand : IRequest<CommandResult>, ICommand
{
    public static readonly int Rank = 5 + +MigrateSitesCommand.Rank + MigrateContactManagementCommand.Rank;

    public static string Moniker => "data-protection";
    public static string MonikerFriendly => "Data protection";

    public Type[] Dependencies => [typeof(MigrateSitesCommand), typeof(MigrateContactManagementCommand)];
}

public record MigrateFormsCommand : IRequest<CommandResult>, ICommand
{
    public static readonly int Rank = 1 + MigrateSitesCommand.Rank + MigrateCustomModulesCommand.Rank + MigrateUsersCommand.Rank;

    public static string Moniker => "forms";
    public static string MonikerFriendly => "Forms";

    public Type[] Dependencies => [typeof(MigrateSitesCommand), typeof(MigrateCustomModulesCommand), typeof(MigrateUsersCommand)];
}

public record MigrateMediaLibrariesCommand : IRequest<CommandResult>, ICommand
{
    public static readonly int Rank = 1 + MigrateSitesCommand.Rank + MigrateUsersCommand.Rank + MigrateCustomModulesCommand.Rank;

    public static string Moniker => "media-libraries";
    public static string MonikerFriendly => "Media libraries";

    public Type[] Dependencies => [typeof(MigrateSitesCommand), typeof(MigrateUsersCommand), typeof(MigrateCustomModulesCommand)];
}

public record MigratePageTypesCommand : IRequest<CommandResult>, ICommand
{
    public static readonly int Rank = 1 + MigrateSitesCommand.Rank;

    public static string Moniker => "page-types";
    public static string MonikerFriendly => "Page types";

    public Type[] Dependencies => [typeof(MigrateSitesCommand)];
}

public record MigratePagesCommand : IRequest<CommandResult>, ICommand
{
    public static readonly int Rank = 1 + MigrateSitesCommand.Rank + MigrateUsersCommand.Rank + MigratePageTypesCommand.Rank;

    public static string Moniker => "pages";
    public static string MonikerFriendly => "Pages";

    public Type[] Dependencies => [typeof(MigrateSitesCommand), typeof(MigrateUsersCommand), typeof(MigratePageTypesCommand)];
}

public record MigrateCategoriesCommand : IRequest<CommandResult>, ICommand
{
    public static readonly int Rank = 1 + MigrateSitesCommand.Rank + MigrateUsersCommand.Rank + MigratePageTypesCommand.Rank + MigratePagesCommand.Rank;

    public static string Moniker => "categories";
    public static string MonikerFriendly => "Categories";

    public Type[] Dependencies => [typeof(MigrateSitesCommand), typeof(MigrateUsersCommand), typeof(MigratePageTypesCommand), typeof(MigratePagesCommand)];
}

public record MigrateSettingKeysCommand : IRequest<CommandResult>, ICommand
{
    public static readonly int Rank = 1 + MigrateSitesCommand.Rank;

    public static string Moniker => "settings-keys";
    public static string MonikerFriendly => "Settings keys";

    public Type[] Dependencies => [typeof(MigrateSitesCommand)];
}

public record MigrateAttachmentsCommand(string CultureCode) : IRequest<CommandResult>, ICommand, ICultureReliantCommand
{
    public static readonly int Rank = 1 + MigrateSitesCommand.Rank + MigrateCustomModulesCommand.Rank;

    public static string Moniker => "attachments";
    public static string MonikerFriendly => "Attachments";

    public Type[] Dependencies => [typeof(MigrateSitesCommand), typeof(MigrateCustomModulesCommand)];
}

public record MigrateCustomModulesCommand : IRequest<CommandResult>, ICommand
{
    public static readonly int Rank = 1 + MigrateSitesCommand.Rank;

    public static string Moniker => "custom-modules";
    public static string MonikerFriendly => "Custom modules";

    public Type[] Dependencies => [typeof(MigrateSitesCommand)];
}

public record MigrateCustomTablesCommand : IRequest<CommandResult>, ICommand
{
    public static readonly int Rank = 1 + MigrateSitesCommand.Rank;

    public static string Moniker => "custom-tables";
    public static string MonikerFriendly => "Custom tables";

    public Type[] Dependencies => [];
}
