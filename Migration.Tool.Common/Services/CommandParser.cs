using Migration.Tool.Common.Abstractions;

using static Migration.Tool.Common.Helpers.ConsoleHelper;

namespace Migration.Tool.Common.Services;

public class CommandParser : ICommandParser
{
    public List<ICommand> Parse(Queue<string> args, ref bool bypassDependencyCheck, bool firstHaveToBeMigrate = true)
    {
        var commands = new List<ICommand>();
        while (args.TryDequeue(out string? arg))
        {
            if (arg.IsIn("help", "h"))
            {
                PrintCommandDescriptions();
                break;
            }

            if (arg == "migrate" && firstHaveToBeMigrate)
            {
                firstHaveToBeMigrate = false;
                continue;
            }

            if (arg == "--bypass-dependency-check")
            {
                bypassDependencyCheck = true;
                continue;
            }

            if (firstHaveToBeMigrate)
            {
                Console.WriteLine($@"First must be command, for example {Green("migrate")}");
                PrintCommandDescriptions();
                break;
            }

            // if (arg == $"--{MigrateContactGroupsCommand.Moniker}")
            // {
            //     commands.Add(new MigrateContactGroupsCommand());
            //     continue;
            // }

            if (arg == $"--{MigrateContactManagementCommand.Moniker}")
            {
                commands.Add(new MigrateContactManagementCommand());
                continue;
            }

            if (arg == $"--{MigrateDataProtectionCommand.Moniker}")
            {
                // RequireNumberParameter("--batchSize", out var batchSize);
                commands.Add(new MigrateDataProtectionCommand());
                continue;
            }

            if (arg == $"--{MigrateFormsCommand.Moniker}")
            {
                commands.Add(new MigrateFormsCommand());
                continue;
            }

            if (arg == $"--{MigrateMediaLibrariesCommand.Moniker}")
            {
                commands.Add(new MigrateMediaLibrariesCommand());
                continue;
            }

            if (arg == $"--{MigratePageTypesCommand.Moniker}")
            {
                commands.Add(new MigratePageTypesCommand());
                continue;
            }

            if (arg == $"--{MigratePagesCommand.Moniker}")
            {
                commands.Add(new MigratePagesCommand());
                continue;
            }

            if (arg == $"--{MigrateCategoriesCommand.Moniker}")
            {
                commands.Add(new MigrateCategoriesCommand());
                continue;
            }

            if (arg == $"--{MigrateSettingKeysCommand.Moniker}")
            {
                commands.Add(new MigrateSettingKeysCommand());
                continue;
            }

            if (arg == $"--{MigrateSitesCommand.Moniker}")
            {
                commands.Add(new MigrateSitesCommand());
                continue;
            }

            if (arg == $"--{MigrateUsersCommand.Moniker}")
            {
                commands.Add(new MigrateUsersCommand());
                continue;
            }

            if (arg == $"--{MigrateMembersCommand.Moniker}")
            {
                commands.Add(new MigrateMembersCommand());
            }

            if (arg == $"--{MigrateCustomModulesCommand.Moniker}")
            {
                commands.Add(new MigrateCustomModulesCommand());
                continue;
            }

            if (arg == $"--{MigrateCustomTablesCommand.Moniker}")
            {
                commands.Add(new MigrateCustomTablesCommand());
            }
        }

        return commands;
    }

    private void PrintCommandDescriptions()
    {
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
    }

    private void WriteCommandDesc(string desc, string commandMoniker) => Console.WriteLine($@"{Yellow(commandMoniker)}: {desc}");
}
