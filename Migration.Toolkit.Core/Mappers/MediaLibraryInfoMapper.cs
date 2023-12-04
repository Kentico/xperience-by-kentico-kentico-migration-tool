using CMS.Helpers;
using CMS.MediaLibrary;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Contexts;

namespace Migration.Toolkit.Core.Mappers;

using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;

public class MediaLibraryInfoMapper : EntityMapperBase<KX13.Models.MediaLibrary, MediaLibraryInfo>
{
    public MediaLibraryInfoMapper(
        ILogger<MediaLibraryInfoMapper> logger,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol
    ) : base(logger, primaryKeyMappingContext, protocol)
    {
    }

    protected override MediaLibraryInfo? CreateNewInstance(KX13.Models.MediaLibrary source, MappingHelper mappingHelper, AddFailure addFailure) =>
        MediaLibraryInfo.New();

    protected override MediaLibraryInfo MapInternal(KX13.Models.MediaLibrary source, MediaLibraryInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        // Sets the library properties
        target.LibraryDisplayName = source.LibraryDisplayName;
        target.LibraryName = source.LibraryName;
        target.LibraryDescription = source.LibraryDescription;
        target.LibraryFolder = source.LibraryFolder;
        target.LibraryGUID = mappingHelper.Require(source.LibraryGuid, nameof(source.LibraryGuid));
        target.LibraryDisplayName = source.LibraryDisplayName;
        target.LibraryDescription = source.LibraryDescription;

        if (!target.LibraryFolder.StartsWith($"{source.LibrarySite.SiteName}_", StringComparison.InvariantCultureIgnoreCase))
        {
            target.LibraryFolder = $"{source.LibrarySite.SiteName}_{source.LibraryFolder}";
        }

        target.LibraryLastModified = mappingHelper.Require(source.LibraryLastModified, nameof(source.LibraryLastModified));

        return target;
    }
}