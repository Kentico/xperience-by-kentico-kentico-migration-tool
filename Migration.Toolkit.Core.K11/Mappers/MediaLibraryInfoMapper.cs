namespace Migration.Toolkit.Core.K11.Mappers;

using CMS.MediaLibrary;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.K11.Contexts;
using Migration.Toolkit.K11.Models;

public class MediaLibraryInfoMapper(ILogger<MediaLibraryInfoMapper> logger,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol)
    : EntityMapperBase<MediaLibrary, MediaLibraryInfo>(logger, primaryKeyMappingContext, protocol)
{
    protected override MediaLibraryInfo? CreateNewInstance(MediaLibrary source, MappingHelper mappingHelper, AddFailure addFailure) =>
        MediaLibraryInfo.New();

    protected override MediaLibraryInfo MapInternal(MediaLibrary source, MediaLibraryInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
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