using Migration.Toolkit.Common.Abstractions;

namespace Migration.Toolkit.Common.Services;

public interface ICommandParser
{
    List<ICommand> Parse(Queue<string> args, ref bool bypassDependencyCheck, bool firstHaveToBeMigrate = true);
}
