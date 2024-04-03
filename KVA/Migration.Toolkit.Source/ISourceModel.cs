namespace Migration.Toolkit.Source;

using System.Data;
using Migration.Toolkit.Common;

public interface ISourceModel<out T>
{
    static abstract bool IsAvailable(SemanticVersion version);
    static abstract string GetPrimaryKeyName(SemanticVersion version);
    static abstract string TableName { get; }
    static abstract string GuidColumnName { get; }
    static abstract T FromReader(IDataReader reader, SemanticVersion version);
}