using System.Collections.Frozen;
using System.Data;

using Migration.Tool.Common;

namespace Migration.Tool.Source;

public interface ISourceModel<out T>
{
    static abstract string TableName { get; }
    static abstract string GuidColumnName { get; }
    static abstract bool IsAvailable(SemanticVersion version);
    static abstract string GetPrimaryKeyName(SemanticVersion version);
    static abstract T FromReader(IDataReader reader, SemanticVersion version);
}

public interface ISourceGuidEntity
{
    (Guid EntityGuid, int? SiteId) GetIdentity();
    static abstract FrozenDictionary<Guid, int[]> Load(ModelFacade modelFacade);
    static abstract Guid NewGuidNs { get; }
}
