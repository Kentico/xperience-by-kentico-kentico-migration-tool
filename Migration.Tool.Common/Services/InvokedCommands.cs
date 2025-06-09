using Migration.Tool.Common.Abstractions;

namespace Migration.Tool.Common;
public class InvokedCommands
{
    public List<ICommand> Commands { get; } = [];
}
