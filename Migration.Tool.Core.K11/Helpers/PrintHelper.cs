namespace Migration.Tool.Core.K11.Helpers;

public static class PrintHelper
{
    public static string PrintDictionary(Dictionary<string, object?> dictionary) =>
        string.Join(", ", dictionary.Select(x => $"{x.Key}:{x.Value ?? "<null>"}"));
}
