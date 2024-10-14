namespace Migration.Tool.Core.KX13;

public class MappingFailureException : InvalidOperationException
{
    public MappingFailureException(string keyName, string reason) : base($"Key '{keyName}' mapping failed: {reason}")
    {
        KeyName = keyName;
        Reason = reason;
    }

    public string KeyName { get; }
    public string Reason { get; }
}
