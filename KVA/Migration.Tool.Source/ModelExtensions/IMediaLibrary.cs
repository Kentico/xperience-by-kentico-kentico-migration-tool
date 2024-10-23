using System.Collections.Frozen;
using Migration.Tool.Common;
using Migration.Tool.Common.Helpers;

namespace Migration.Tool.Source.Model;

public partial interface IMediaLibrary : ISourceGuidEntity
{
    (Guid EntityGuid, int? SiteId) ISourceGuidEntity.GetIdentity() => (LibraryGUID ?? GuidHelper.CreateGuidFromLibraryAndSiteID(LibraryName, LibrarySiteID), LibrarySiteID);

    static FrozenDictionary<Guid, int[]> ISourceGuidEntity.Load(ModelFacade modelFacade) =>
        modelFacade
            .Select("SELECT LibraryGUID, LibrarySiteID, LibraryName FROM Media_Library", (reader, _) => new { LibraryGUID = reader.Unbox<Guid?>("LibraryGUID"), LibrarySiteID = reader.Unbox<int>("LibrarySiteID"), LibraryName = reader.Unbox<string>("LibraryName") })
            .ToLookup(x => x.LibraryGUID ?? GuidHelper.CreateGuidFromLibraryAndSiteID(x.LibraryName, x.LibrarySiteID))
            .ToFrozenDictionary(
                x => x.Key,
                x => x.Select(z => z.LibrarySiteID).ToArray()
            );

    static Guid ISourceGuidEntity.NewGuidNs => new("15EC1A66-2322-4931-BE8C-C22AA117CF8F");
}
