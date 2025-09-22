using Migration.Tool.Common.Abstractions;

using static Migration.Tool.Common.Helpers.ConsoleHelper;

namespace Migration.Tool.Common.Services;

public class CommandParser : ICommandParser
{
    public (MasterCommand? masterCommand, List<ICommand> commands) Parse(Queue<string> args, ref bool bypassDependencyCheck)
    {
        MasterCommand? command = args.Count >= 1
            ? args.Dequeue() switch
            {
                "migrate" => MasterCommand.Migrate,
                "patch" => MasterCommand.Patch,
                _ => null
            }
            : null;

        if (command is null)
        {
            Console.WriteLine($@"No known command passed");
            PrintCommandDescriptions();
            return (null, []);
        }

        var subcommands = new List<ICommand>();
        while (args.TryDequeue(out string? arg))
        {
            if (arg.IsIn("help", "h"))
            {
                PrintCommandDescriptions();
                break;
            }

            if (command == MasterCommand.Patch)
            {
                Console.WriteLine($@"Patch command does not support any subcommands, but '{arg}' was issued. Continuing as with no subcommands.");
                break;
            }

            if (arg == "--bypass-dependency-check")
            {
                bypassDependencyCheck = true;
                continue;
            }

            if (arg == $"--{MigrateContactManagementCommand.Moniker}")
            {
                subcommands.Add(new MigrateContactManagementCommand());
                continue;
            }

            if (arg == $"--{MigrateDataProtectionCommand.Moniker}")
            {
                // RequireNumberParameter("--batchSize", out var batchSize);
                subcommands.Add(new MigrateDataProtectionCommand());
                continue;
            }

            if (arg == $"--{MigrateFormsCommand.Moniker}")
            {
                subcommands.Add(new MigrateFormsCommand());
                continue;
            }

            if (arg == $"--{MigrateMediaLibrariesCommand.Moniker}")
            {
                subcommands.Add(new MigrateMediaLibrariesCommand());
                continue;
            }

            if (arg == $"--{MigratePageTypesCommand.Moniker}")
            {
                subcommands.Add(new MigratePageTypesCommand());
                continue;
            }

            if (arg == $"--{MigratePagesCommand.Moniker}")
            {
                subcommands.Add(new MigratePagesCommand());
                continue;
            }

            if (arg == $"--{MigrateCategoriesCommand.Moniker}")
            {
                subcommands.Add(new MigrateCategoriesCommand());
                continue;
            }

            if (arg == $"--{MigrateSettingKeysCommand.Moniker}")
            {
                subcommands.Add(new MigrateSettingKeysCommand());
                continue;
            }

            if (arg == $"--{MigrateSitesCommand.Moniker}")
            {
                subcommands.Add(new MigrateSitesCommand());
                continue;
            }

            if (arg == $"--{MigrateUsersCommand.Moniker}")
            {
                subcommands.Add(new MigrateUsersCommand());
                continue;
            }

            if (arg == $"--{MigrateMembersCommand.Moniker}")
            {
                subcommands.Add(new MigrateMembersCommand());
                continue;
            }

            if (arg == $"--{MigrateCustomModulesCommand.Moniker}")
            {
                subcommands.Add(new MigrateCustomModulesCommand());
                continue;
            }

            if (arg == $"--{MigrateCustomTablesCommand.Moniker}")
            {
                subcommands.Add(new MigrateCustomTablesCommand());
                continue;
            }

            if (arg == $"--{MigrateContentTypeRestrictionsCommand.Moniker}")
            {
                subcommands.Add(new MigrateContentTypeRestrictionsCommand());
                continue;
            }

            throw new InvalidOperationException($"Unknown command '{arg}'");
        }

        return (command, subcommands);
    }

    private void PrintCommandDescriptions()
    {
        Console.WriteLine($"Command {Green("migrate")}: Migrates data from legacy Kentico CMS project to XbyK project");
        Console.WriteLine("Subcommands:");
        WriteCommandDesc($"starts migration of {Green(MigratePageTypesCommand.MonikerFriendly)}", $"migrate --{MigratePageTypesCommand.Moniker}");
        WriteCommandDesc($"starts migration of {Green(MigratePagesCommand.MonikerFriendly)}", $"migrate --{MigratePagesCommand.Moniker}");
        WriteCommandDesc($"starts migration of {Green(MigrateCategoriesCommand.MonikerFriendly)}", $"migrate --{MigrateCategoriesCommand.Moniker}");
        WriteCommandDesc($"starts migration of {Green(MigrateSettingKeysCommand.MonikerFriendly)}", $"migrate --{MigrateSettingKeysCommand.Moniker}");
        WriteCommandDesc($"starts migration of {Green(MigrateContactManagementCommand.MonikerFriendly)}", $"migrate --{MigrateContactManagementCommand.Moniker}");
        WriteCommandDesc($"starts migration of {Green(MigrateDataProtectionCommand.MonikerFriendly)}", $"migrate --{MigrateDataProtectionCommand.Moniker}");
        WriteCommandDesc($"starts migration of {Green(MigrateFormsCommand.MonikerFriendly)}", $"migrate --{MigrateFormsCommand.Moniker}");
        WriteCommandDesc($"starts migration of {Green(MigrateMediaLibrariesCommand.MonikerFriendly)}", $"migrate --{MigrateMediaLibrariesCommand.Moniker}");
        WriteCommandDesc($"starts migration of {Green(MigrateSitesCommand.MonikerFriendly)}", $"migrate --{MigrateSitesCommand.Moniker}");
        WriteCommandDesc($"starts migration of {Green(MigrateUsersCommand.MonikerFriendly)}", $"migrate --{MigrateUsersCommand.Moniker}");
        WriteCommandDesc($"starts migration of {Green(MigrateMembersCommand.MonikerFriendly)}", $"migrate --{MigrateMembersCommand.Moniker}");
        WriteCommandDesc($"starts migration of {Green(MigrateAttachmentsCommand.MonikerFriendly)}", $"migrate --{MigrateAttachmentsCommand.Moniker}");
        WriteCommandDesc($"starts migration of {Green(MigrateCustomModulesCommand.MonikerFriendly)}", $"migrate --{MigrateCustomModulesCommand.Moniker}");
        Console.WriteLine();
        Console.WriteLine($"Command {Green("patch")}: Applies migration patches to XbyK database. Patches are also applied at each run of {Green("migrate")}. Use this command to run patches without migration. " +
            $"Migration patches fix data problems caused by bugs in previous versions of Migration Tool. This command is idempotent - i.e. tolerant to multiple runs.");
    }

    private void WriteCommandDesc(string desc, string commandMoniker) => Console.WriteLine($@"{Yellow(commandMoniker)}: {desc}");
}
