namespace Migration.Toolkit.Source.Mappers;

using CMS.MediaLibrary;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Source.Contexts;
using Migration.Toolkit.Source.Model;

public record MediaLibraryInfoMapperSource(IMediaLibrary MediaLibrary, ICmsSite Site);

public class MediaLibraryInfoMapper(ILogger<MediaLibraryInfoMapper> logger, PrimaryKeyMappingContext primaryKeyMappingContext, IProtocol protocol)
    : EntityMapperBase<MediaLibraryInfoMapperSource, MediaLibraryInfo>(logger, primaryKeyMappingContext, protocol)
{
    protected override MediaLibraryInfo? CreateNewInstance(MediaLibraryInfoMapperSource source, MappingHelper mappingHelper, AddFailure addFailure) =>
        MediaLibraryInfo.New();

    protected override MediaLibraryInfo MapInternal(MediaLibraryInfoMapperSource s, MediaLibraryInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        var (ksLibrary, ksSite) = s;

        // Sets the library properties
        target.LibraryDisplayName = ksLibrary.LibraryDisplayName;
        target.LibraryName = ksLibrary.LibraryName;
        target.LibraryDescription = ksLibrary.LibraryDescription;
        target.LibraryFolder = ksLibrary.LibraryFolder;
        target.LibraryGUID = mappingHelper.Require(ksLibrary.LibraryGUID, nameof(ksLibrary.LibraryGUID));
        target.LibraryDisplayName = ksLibrary.LibraryDisplayName;
        target.LibraryDescription = ksLibrary.LibraryDescription;

        if (!target.LibraryFolder.StartsWith($"{ksSite.SiteName}_", StringComparison.InvariantCultureIgnoreCase))
        {
            target.LibraryFolder = $"{ksSite.SiteName}_{ksLibrary.LibraryFolder}";
        }

        target.LibraryLastModified = mappingHelper.Require(ksLibrary.LibraryLastModified, nameof(ksLibrary.LibraryLastModified));

        return target;
    }
}