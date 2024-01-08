namespace Migration.Toolkit.Common.Services;

using Migration.Toolkit.Common.Abstractions;

public interface ICommandParser
{
    List<ICommand> Parse(Queue<string> args, ref bool bypassDependencyCheck, bool firstHaveToBeMigrate = true);
}