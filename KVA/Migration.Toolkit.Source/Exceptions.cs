namespace Migration.Toolkit.Source;

public class MappingFailureException(string keyName, string reason) : InvalidOperationException($"Key '{keyName}' mapping failed: {reason}")
{
    public string KeyName { get; } = keyName;
    public string Reason { get; } = reason;
}
