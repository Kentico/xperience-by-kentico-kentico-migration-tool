using System.Collections.Frozen;
using Migration.Tool.Common;

namespace Migration.Tool.Source.Model;

public partial interface IMediaFile : ISourceGuidEntity
{
    (Guid EntityGuid, int? SiteId) ISourceGuidEntity.GetIdentity() => (FileGUID, FileSiteID);

    static FrozenDictionary<Guid, int[]> ISourceGuidEntity.Load(ModelFacade modelFacade) =>
        modelFacade
            .Select("SELECT FileGUID, FileSiteID FROM Media_File", (reader, _) => new { FileGUID = reader.Unbox<Guid>("FileGUID"), FileSiteID = reader.Unbox<int>("FileSiteID") })
            .ToLookup(x => x.FileGUID)
            .ToFrozenDictionary(
                x => x.Key,
                x => x.Select(z => z.FileSiteID).ToArray()
            );

    static Guid ISourceGuidEntity.NewGuidNs => new("A897335A-2FC0-4B9D-87BC-3BA95BD5B309");
}
