using MediatR;

namespace Migration.Toolkit.Core;

using System.Reflection;
using Migration.Toolkit.Common.Abstractions;

public interface ICommand
{
    Type[] Dependencies { get; }

    int Rank => (int)(GetType().GetField("Rank", BindingFlags.Static | BindingFlags.Public)?.GetValue(null) ?? 999);
}

public interface ICultureReliantCommand
{
    string CultureCode { get; }
}

public record MigrateSitesCommand : IRequest<CommandResult>, ICommand
{
    public static readonly int Rank = 1;

    public static string Moniker => "sites";
    public static string MonikerFriendly => "Sites";

    public Type[] Dependencies => new Type[] { };
}

public record MigrateUsersCommand : IRequest<CommandResult>, ICommand
{
    public static readonly int Rank = 1 + MigrateSitesCommand.Rank + MigrateCustomModulesCommand.Rank;

    public static string Moniker => "users";
    public static string MonikerFriendly => "Users";

    public Type[] Dependencies => new[] { typeof(MigrateCustomModulesCommand) };
}

public record MigrateMembersCommand : IRequest<CommandResult>, ICommand
{
    public static readonly int Rank = 2 + MigrateSitesCommand.Rank + MigrateCustomModulesCommand.Rank;

    public static string Moniker => "members";
    public static string MonikerFriendly => "Members";

    public Type[] Dependencies => new[] { typeof(MigrateCustomModulesCommand) };
}

// public record MigrateContactGroupsCommand : IRequest<CommandResult>, ICommand
// {
//     public static string Moniker => "contact-groups";
//     public static string MonikerFriendly => "Contact groups";
//
//     public Type[] Dependencies => Type.EmptyTypes;
//  }

public record MigrateContactManagementCommand : IRequest<CommandResult>, ICommand
{
    public static readonly int Rank = 1 + MigrateUsersCommand.Rank + MigrateCustomModulesCommand.Rank;

    public static string Moniker => "contact-management";
    public static string MonikerFriendly => "Contact management";

    public Type[] Dependencies => new[] {  typeof(MigrateUsersCommand), typeof(MigrateCustomModulesCommand) };
}

public record MigrateDataProtectionCommand() : IRequest<CommandResult>, ICommand
{
    public static readonly int Rank = 5 + + MigrateSitesCommand.Rank + MigrateContactManagementCommand.Rank;

    public static string Moniker => "data-protection";
    public static string MonikerFriendly => "Data protection";

    public Type[] Dependencies => new[] { typeof(MigrateSitesCommand), typeof(MigrateContactManagementCommand) };
}

public record MigrateFormsCommand : IRequest<CommandResult>, ICommand
{
    public static readonly int Rank = 1 + MigrateSitesCommand.Rank + MigrateCustomModulesCommand.Rank + MigrateUsersCommand.Rank;

    public static string Moniker => "forms";
    public static string MonikerFriendly => "Forms";

    public Type[] Dependencies => new[] { typeof(MigrateSitesCommand), typeof(MigrateCustomModulesCommand), typeof(MigrateUsersCommand) };
}

public record MigrateMediaLibrariesCommand : IRequest<CommandResult>, ICommand
{
    public static readonly int Rank = 1 + MigrateSitesCommand.Rank + MigrateUsersCommand.Rank + MigrateCustomModulesCommand.Rank;

    public static string Moniker => "media-libraries";
    public static string MonikerFriendly => "Media libraries";

    public Type[] Dependencies => new[] { typeof(MigrateSitesCommand), typeof(MigrateUsersCommand), typeof(MigrateCustomModulesCommand) };
}

public record MigratePageTypesCommand : IRequest<CommandResult>, ICommand
{
    public static readonly int Rank = 1 + MigrateSitesCommand.Rank;

    public static string Moniker => "page-types";
    public static string MonikerFriendly => "Page types";

    public Type[] Dependencies => new[] { typeof(MigrateSitesCommand) };
}

public record MigratePagesCommand(): IRequest<CommandResult>, ICommand
{
    public static readonly int Rank = 1 + MigrateSitesCommand.Rank + MigrateUsersCommand.Rank + MigratePageTypesCommand.Rank;

    public static string Moniker => "pages";
    public static string MonikerFriendly => "Pages";

    public Type[] Dependencies => new[] { typeof(MigrateSitesCommand), typeof(MigrateUsersCommand), typeof(MigratePageTypesCommand) };
}

public record MigrateSettingKeysCommand : IRequest<CommandResult>, ICommand
{
    public static readonly int Rank = 1 + MigrateSitesCommand.Rank;

    public static string Moniker => "settings-keys";
    public static string MonikerFriendly => "Settings keys";

    public Type[] Dependencies => new[] { typeof(MigrateSitesCommand) };
}

public record MigrateAttachmentsCommand(string CultureCode) : IRequest<CommandResult>, ICommand, ICultureReliantCommand
{
    public static readonly int Rank = 1 + MigrateSitesCommand.Rank + MigrateCustomModulesCommand.Rank;

    public static string Moniker => "attachments";
    public static string MonikerFriendly => "Attachments";

    public Type[] Dependencies => new[] { typeof(MigrateSitesCommand), typeof(MigrateCustomModulesCommand) };
}

public record MigrateCustomModulesCommand(): IRequest<CommandResult>, ICommand
{
    public static readonly int Rank = 1 + MigrateSitesCommand.Rank;

    public static string Moniker => "custom-modules";
    public static string MonikerFriendly => "Custom modules";

    public Type[] Dependencies => new[] { typeof(MigrateSitesCommand) };
}