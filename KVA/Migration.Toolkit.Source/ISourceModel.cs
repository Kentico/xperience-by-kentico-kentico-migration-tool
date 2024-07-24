using System.Data;

using Migration.Toolkit.Common;

namespace Migration.Toolkit.Source;

public interface ISourceModel<out T>
{
    static abstract string TableName { get; }
    static abstract string GuidColumnName { get; }
    static abstract bool IsAvailable(SemanticVersion version);
    static abstract string GetPrimaryKeyName(SemanticVersion version);
    static abstract T FromReader(IDataReader reader, SemanticVersion version);
}
