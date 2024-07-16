using System.Reflection;

namespace Migration.Toolkit.Common.Abstractions;

public interface ICommand
{
    Type[] Dependencies { get; }

    int Rank => (int)(GetType().GetField("Rank", BindingFlags.Static | BindingFlags.Public)?.GetValue(null) ?? 999);

    static virtual string Moniker { get; } = "";
    static virtual string MonikerFriendly { get; } = "";
}

public interface ICultureReliantCommand
{
    string CultureCode { get; }
}
