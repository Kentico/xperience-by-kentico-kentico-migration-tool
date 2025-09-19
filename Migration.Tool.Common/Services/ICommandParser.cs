using Migration.Tool.Common.Abstractions;

namespace Migration.Tool.Common.Services;

public interface ICommandParser
{
    (MasterCommand? masterCommand, List<ICommand> commands) Parse(Queue<string> args, ref bool bypassDependencyCheck);
}
