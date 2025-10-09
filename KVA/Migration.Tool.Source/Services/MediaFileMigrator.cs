using System.Collections.Immutable;
using CMS.Base;
using CMS.MediaLibrary;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.KXP.Api;
using Migration.Tool.KXP.Api.Auxiliary;
using Migration.Tool.Source.Auxiliary;
using Migration.Tool.Source.Contexts;
using Migration.Tool.Source.Handlers;
using Migration.Tool.Source.Mappers;
using Migration.Tool.Source.Model;

namespace Migration.Tool.Source.Services;

public class MediaFileMigrator(
    ILogger<MigrateMediaLibrariesCommandHandler> logger,
    ModelFacade modelFacade,
    KxpMediaFileFacade mediaFileFacade,
#pragma warning disable CS0618 // Type or member is obsolete
    IEntityMapper<MediaFileInfoMapperSource, MediaFileInfo> mediaFileInfoMapper,
    IEntityMapper<MediaLibraryInfoMapperSource, MediaLibraryInfo> mediaLibraryInfoMapper,
#pragma warning restore CS0618 // Type or member is obsolete
    ToolConfiguration toolConfiguration,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    EntityIdentityFacade entityIdentityFacade,
    IProtocol protocol
    ) : IMediaFileMigrator
{
    public Task<CommandResult> Handle(MigrateMediaLibrariesCommand request, CancellationToken cancellationToken)
    {
        var ksMediaLibraries = modelFacade.SelectAll<IMediaLibrary>(" ORDER BY LibraryID");

        var nonUniqueLibraryNames = modelFacade.Select("""
                                                       SELECT LibraryName FROM Media_Library GROUP BY LibraryName HAVING COUNT(*) > 1
                                                       """, (reader, version) => reader.Unbox<string>("LibraryName"))
            .ToImmutableHashSet(StringComparer.InvariantCultureIgnoreCase);

#pragma warning disable CS0618 // Type or member is obsolete
        var migratedMediaLibraries = new List<(IMediaLibrary sourceLibrary, ICmsSite sourceSite, MediaLibraryInfo targetLibrary)>();
#pragma warning restore CS0618 // Type or member is obsolete
        foreach (var ksMediaLibrary in ksMediaLibraries)
        {
            (bool isFixed, var libraryGuid) = entityIdentityFacade.Translate(ksMediaLibrary);
            if (isFixed)
            {
                logger.LogWarning("MediaLibrary {Library} has non-unique guid, new guid {Guid} was required", new { ksMediaLibrary.LibraryGUID, ksMediaLibrary.LibraryName, ksMediaLibrary.LibrarySiteID }, libraryGuid);
            }

            protocol.FetchedSource(ksMediaLibrary);

            var mediaLibraryInfo = mediaFileFacade.GetMediaLibraryInfo(libraryGuid);

            protocol.FetchedTarget(mediaLibraryInfo);

            if (modelFacade.SelectById<ICmsSite>(ksMediaLibrary.LibrarySiteID) is not { } ksSite)
            {
                protocol.Append(HandbookReferences
#pragma warning disable CS0618 // Type or member is obsolete
                    .InvalidSourceData<MediaLibraryInfo>()
                    .WithId(nameof(MediaLibraryInfo.LibraryID), ksMediaLibrary.LibraryID)
#pragma warning restore CS0618 // Type or member is obsolete
                    .WithMessage("Media library has missing site assigned")
                );
                logger.LogError("Missing site, SiteID=={SiteId}", ksMediaLibrary.LibrarySiteID);
                continue;
            }

            string safeLibraryName = nonUniqueLibraryNames.Contains(ksMediaLibrary.LibraryName)
                ? $"{ksSite.SiteName}_{ksMediaLibrary.LibraryName}"
                : ksMediaLibrary.LibraryName;

            var mapped = mediaLibraryInfoMapper.Map(new MediaLibraryInfoMapperSource(ksMediaLibrary, ksSite, libraryGuid, safeLibraryName), mediaLibraryInfo);
            protocol.MappedTarget(mapped);

            if (mapped is { Success: true } result)
            {
                (var mfi, bool newInstance) = result;
                ArgumentNullException.ThrowIfNull(mfi, nameof(mfi));

                try
                {
                    mediaFileFacade.SetMediaLibrary(mfi);

                    protocol.Success(ksMediaLibrary, mfi, mapped);
                    logger.LogEntitySetAction(newInstance, mfi);
                }
                catch (Exception ex)
                {
                    protocol.Append(HandbookReferences
#pragma warning disable CS0618 // Type or member is obsolete
                        .ErrorCreatingTargetInstance<MediaLibraryInfo>(ex)
#pragma warning restore CS0618 // Type or member is obsolete
                        .NeedsManualAction()
                        .WithIdentityPrint(mfi)
                    );
                    logger.LogEntitySetError(ex, newInstance, mfi);
                    continue;
                }

#pragma warning disable CS0618 // Type or member is obsolete
                primaryKeyMappingContext.SetMapping<MediaLibraryInfo>(
#pragma warning restore CS0618 // Type or member is obsolete
                    r => r.LibraryID,
                    ksMediaLibrary.LibraryID,
                    mfi.LibraryID
                );

                migratedMediaLibraries.Add((ksMediaLibrary, ksSite, mfi));
            }
        }

        RequireMigratedMediaFiles(migratedMediaLibraries, cancellationToken);

        return Task.FromResult((CommandResult)new GenericCommandResult());
    }

    private LoadMediaFileResult LoadMediaFileBinary(string? sourceMediaLibraryPath, string relativeFilePath, string contentType)
    {
        if (sourceMediaLibraryPath == null)
        {
            return new LoadMediaFileResult(false, null, "<missing library path>");
        }

        string filePath = Path.Combine(sourceMediaLibraryPath, relativeFilePath);
        if (File.Exists(filePath))
        {
            byte[] data = File.ReadAllBytes(filePath);
            var dummyFile = DummyUploadedFile.FromByteArray(data, contentType, data.LongLength, Path.GetFileName(filePath));
            return new LoadMediaFileResult(true, dummyFile, filePath);
        }

        return new LoadMediaFileResult(false, null, filePath);
    }

#pragma warning disable CS0618 // Type or member is obsolete
    private void RequireMigratedMediaFiles(List<(IMediaLibrary sourceLibrary, ICmsSite sourceSite, MediaLibraryInfo targetLibrary)> migratedMediaLibraries, CancellationToken cancellationToken)
#pragma warning restore CS0618 // Type or member is obsolete
    {
        foreach (var (ksMediaLibrary, ksSite, targetMediaLibrary) in migratedMediaLibraries)
        {
            string? sourceMediaLibraryPath = AssetFacade.GetMediaLibraryAbsolutePath(toolConfiguration, ksSite, ksMediaLibrary, modelFacade);
            bool loadMediaFileData = !toolConfiguration.MigrateOnlyMediaFileInfo.GetValueOrDefault(false) && !string.IsNullOrWhiteSpace(sourceMediaLibraryPath);
            var ksMediaFiles = modelFacade.SelectWhere<IMediaFile>("FileLibraryID = @FileLibraryId", new SqlParameter("FileLibraryId", ksMediaLibrary.LibraryID));

            foreach (var ksMediaFile in ksMediaFiles)
            {
                protocol.FetchedSource(ksMediaFile);

                bool found = false;
                IUploadedFile? uploadedFile = null;
                string? fullMediaPath = "<uninitialized>";
                if (loadMediaFileData)
                {
                    (found, uploadedFile, fullMediaPath) = LoadMediaFileBinary(sourceMediaLibraryPath, ksMediaFile.FilePath, ksMediaFile.FileMimeType);
                    if (!found)
                    {
                        // report missing file (currently reported in mapper)
                    }
                }

                string? librarySubfolder = Path.GetDirectoryName(ksMediaFile.FilePath);

                (bool isFixed, var safeMediaFileGuid) = entityIdentityFacade.Translate(ksMediaFile);
                if (isFixed)
                {
                    logger.LogWarning("MediaFile {File} has non-unique guid, new guid {Guid} was required", new { ksMediaFile.FileGUID, ksMediaFile.FileName, ksMediaFile.FileSiteID }, safeMediaFileGuid);
                }

                var kxoMediaFile = mediaFileFacade.GetMediaFile(safeMediaFileGuid);

                protocol.FetchedTarget(kxoMediaFile);

                var source = new MediaFileInfoMapperSource(fullMediaPath, ksMediaFile, targetMediaLibrary.LibraryID, found ? uploadedFile : null,
                    librarySubfolder, toolConfiguration.MigrateOnlyMediaFileInfo.GetValueOrDefault(false), safeMediaFileGuid);
                var mapped = mediaFileInfoMapper.Map(source, kxoMediaFile);
                protocol.MappedTarget(mapped);

                if (mapped is { Success: true } result)
                {
                    (var mf, bool newInstance) = result;
                    ArgumentNullException.ThrowIfNull(mf, nameof(mf));

                    try
                    {
                        mediaFileFacade.EnsureMediaFilePathExistsInLibrary(mf, targetMediaLibrary.LibraryID);
                        mediaFileFacade.SetMediaFile(mf, newInstance);

                        protocol.Success(ksMediaFile, mf, mapped);
                        logger.LogEntitySetAction(newInstance, mf);
                    }
                    catch (Exception ex)
                    {
                        protocol.Append(HandbookReferences
#pragma warning disable CS0618 // Type or member is obsolete
                            .ErrorCreatingTargetInstance<MediaLibraryInfo>(ex)
#pragma warning restore CS0618 // Type or member is obsolete
                            .NeedsManualAction()
                            .WithIdentityPrint(mf)
                        );
                        logger.LogEntitySetError(ex, newInstance, mf);
                        continue;
                    }

#pragma warning disable CS0618 // Type or member is obsolete
                    primaryKeyMappingContext.SetMapping<MediaFileInfo>(
#pragma warning restore CS0618 // Type or member is obsolete
                        r => r.FileID,
                        ksMediaFile.FileID,
                        mf.FileID
                    );
                }
            }
        }
    }

    private record LoadMediaFileResult(bool Found, IUploadedFile? File, string? SearchedPath);
}
