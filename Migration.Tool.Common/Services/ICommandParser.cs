using Migration.Tool.Common.Abstractions;

namespace Migration.Tool.Common.Services;

public interface ICommandParser
{
    List<ICommand> Parse(Queue<string> args, ref bool bypassDependencyCheck, bool firstHaveToBeMigrate = true);
}
