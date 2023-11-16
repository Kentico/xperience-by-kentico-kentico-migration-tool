using CMS.Helpers;
using CMS.MediaLibrary;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.MigrationProtocol;

namespace Migration.Toolkit.Core.Mappers;

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

        if (!target.LibraryName.StartsWith($"{source.LibrarySite.SiteName}_", StringComparison.InvariantCultureIgnoreCase))
        {
            target.LibraryName = $"{source.LibrarySite.SiteName}_{source.LibraryName}";
        }

        target.LibraryDisplayName = source.LibraryDisplayName;
        target.LibraryDescription = source.LibraryDescription;

        if (!target.LibraryFolder.StartsWith($"{source.LibrarySite.SiteName}_", StringComparison.InvariantCultureIgnoreCase))
        {
            target.LibraryFolder = $"{source.LibrarySite.SiteName}_{source.LibraryFolder}";
        }

        target.LibraryLastModified = mappingHelper.Require(source.LibraryLastModified, nameof(source.LibraryLastModified));
        // TODO tomas.krch: 2023-10-30 verify is needed
        // target.LibraryUseDirectPathForContent = source.LibraryUseDirectPathForContent ?? true;

        // TODOV27 tomas.krch: 2023-09-05: library obsolete properties
        // target.LibraryTeaserPath = source.LibraryTeaserPath;
        // target.LibraryTeaserGUID = source.LibraryTeaserGuid ?? Guid.Empty;
        //
        // if (mappingHelper.TranslateRequiredId<KX13.Models.CmsSite>(c => c.SiteId, source.LibrarySiteId, out var siteId))
        // {
        //     target.LibrarySiteID = siteId;
        // }

        // OBSOLETE
        // var libraryAccess = mappingHelper.Require(source.LibraryAccess, nameof(source.LibraryAccess));
        // target.FileCreate = SecurityHelper.GetSecurityAccessEnum(libraryAccess, 1);
        // target.FileDelete = SecurityHelper.GetSecurityAccessEnum(libraryAccess, 2);
        // target.FileModify = SecurityHelper.GetSecurityAccessEnum(libraryAccess, 3);
        // target.FolderCreate = SecurityHelper.GetSecurityAccessEnum(libraryAccess, 4);
        // target.FolderDelete = SecurityHelper.GetSecurityAccessEnum(libraryAccess, 5);
        // target.FolderModify = SecurityHelper.GetSecurityAccessEnum(libraryAccess, 6);
        // target.Access = SecurityHelper.GetSecurityAccessEnum(libraryAccess, 7);

        return target;
    }
}