using System.Collections.Immutable;

using CMS.Base;
using CMS.MediaLibrary;

using MediatR;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.KXP.Api;
using Migration.Toolkit.KXP.Api.Auxiliary;
using Migration.Toolkit.KXP.Context;
using Migration.Toolkit.KXP.Models;
using Migration.Toolkit.Source.Contexts;
using Migration.Toolkit.Source.Helpers;
using Migration.Toolkit.Source.Mappers;
using Migration.Toolkit.Source.Model;

namespace Migration.Toolkit.Source.Handlers;

public class MigrateMediaLibrariesCommandHandler(
    ILogger<MigrateMediaLibrariesCommandHandler> logger,
    IDbContextFactory<KxpContext> kxpContextFactory,
    ModelFacade modelFacade,
    KxpMediaFileFacade mediaFileFacade,
    IEntityMapper<MediaFileInfoMapperSource, MediaFileInfo> mediaFileInfoMapper,
    IEntityMapper<MediaLibraryInfoMapperSource, MediaLibraryInfo> mediaLibraryInfoMapper,
    ToolkitConfiguration toolkitConfiguration,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    IProtocol protocol)
    : IRequestHandler<MigrateMediaLibrariesCommand, CommandResult>, IDisposable
{
    private const string DirMedia = "media";

    private KxpContext kxpContext = kxpContextFactory.CreateDbContext();

    public void Dispose() => kxpContext.Dispose();

    public async Task<CommandResult> Handle(MigrateMediaLibrariesCommand request, CancellationToken cancellationToken)
    {
        var skippedMediaLibraries = new HashSet<Guid>();
        var unsuitableMediaLibraries =
            modelFacade.Select("""
                               SELECT LibraryName, STRING_AGG(CAST(LibraryGUID AS NVARCHAR(max)), '|') as [LibraryGUIDs]
                               FROM Media_Library
                               GROUP BY LibraryName
                               HAVING COUNT(*) > 1
                               """,
                (reader, _) => new { LibraryName = reader.Unbox<string>("LibraryName"), LibraryGuids = reader.Unbox<string?>("LibraryGUIDs")?.Split('|').Select(Guid.Parse).ToImmutableList() ?? [] });

        foreach (var mlg in unsuitableMediaLibraries)
        {
            logger.LogError(
                "Media libraries with LibraryGuid ({LibraryGuids}) have same LibraryName '{LibraryName}', due to removal of sites and media library globalization it is required to set unique LibraryName and LibraryFolder",
                string.Join(",", mlg.LibraryGuids), mlg.LibraryName);

            foreach (var libraryGuid in mlg.LibraryGuids)
            {
                skippedMediaLibraries.Add(libraryGuid);

                protocol.Append(HandbookReferences.NotCurrentlySupportedSkip()
                    .WithMessage($"Media library '{mlg.LibraryName}' with LibraryGuid '{libraryGuid}' doesn't satisfy unique LibraryName and LibraryFolder condition for migration")
                    .WithData(new { LibraryGuid = libraryGuid, mlg.LibraryName })
                );
            }
        }

        var ksMediaLibraries = modelFacade.SelectAll<IMediaLibrary>(" ORDER BY LibraryID");

        var migratedMediaLibraries = new List<(IMediaLibrary sourceLibrary, ICmsSite sourceSite, MediaLibraryInfo targetLibrary)>();
        foreach (var ksMediaLibrary in ksMediaLibraries)
        {
            if (ksMediaLibrary.LibraryGUID is { } libraryGuid && skippedMediaLibraries.Contains(libraryGuid))
            {
                continue;
            }

            protocol.FetchedSource(ksMediaLibrary);

            if (ksMediaLibrary.LibraryGUID is not { } mediaLibraryGuid)
            {
                protocol.Append(HandbookReferences
                    .InvalidSourceData<MediaLibrary>()
                    .WithId(nameof(MediaLibrary.LibraryId), ksMediaLibrary.LibraryID)
                    .WithMessage("Media library has missing MediaLibraryGUID")
                );
                continue;
            }

            var mediaLibraryInfo = mediaFileFacade.GetMediaLibraryInfo(mediaLibraryGuid);

            protocol.FetchedTarget(mediaLibraryInfo);

            if (modelFacade.SelectById<ICmsSite>(ksMediaLibrary.LibrarySiteID) is not { } ksSite)
            {
                protocol.Append(HandbookReferences
                    .InvalidSourceData<MediaLibrary>()
                    .WithId(nameof(MediaLibrary.LibraryId), ksMediaLibrary.LibraryID)
                    .WithMessage("Media library has missing site assigned")
                );
                logger.LogError("Missing site, SiteID=={SiteId}", ksMediaLibrary.LibrarySiteID);
                continue;
            }

            var mapped = mediaLibraryInfoMapper.Map(new MediaLibraryInfoMapperSource(ksMediaLibrary, ksSite), mediaLibraryInfo);
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
                    await kxpContext.DisposeAsync(); // reset context errors
                    kxpContext = await kxpContextFactory.CreateDbContextAsync(cancellationToken);

                    protocol.Append(HandbookReferences
                        .ErrorCreatingTargetInstance<MediaLibraryInfo>(ex)
                        .NeedsManualAction()
                        .WithIdentityPrint(mfi)
                    );
                    logger.LogEntitySetError(ex, newInstance, mfi);
                    continue;
                }

                primaryKeyMappingContext.SetMapping<MediaLibrary>(
                    r => r.LibraryId,
                    ksMediaLibrary.LibraryID,
                    mfi.LibraryID
                );

                migratedMediaLibraries.Add((ksMediaLibrary, ksSite, mfi));
            }
        }

        await RequireMigratedMediaFiles(migratedMediaLibraries, cancellationToken);

        return new GenericCommandResult();
    }

    private LoadMediaFileResult LoadMediaFileBinary(string? sourceMediaLibraryPath, string relativeFilePath, string contentType)
    {
        if (sourceMediaLibraryPath == null)
        {
            return new LoadMediaFileResult(false, null);
        }

        string filePath = Path.Combine(sourceMediaLibraryPath, relativeFilePath);
        if (File.Exists(filePath))
        {
            byte[] data = File.ReadAllBytes(filePath);
            var dummyFile = DummyUploadedFile.FromByteArray(data, contentType, data.LongLength, Path.GetFileName(filePath));
            return new LoadMediaFileResult(true, dummyFile);
        }

        return new LoadMediaFileResult(false, null);
    }

    private async Task RequireMigratedMediaFiles(List<(IMediaLibrary sourceLibrary, ICmsSite sourceSite, MediaLibraryInfo targetLibrary)> migratedMediaLibraries, CancellationToken cancellationToken)
    {
        var kxoDbContext = await kxpContextFactory.CreateDbContextAsync(cancellationToken);
        try
        {
            foreach (var (ksMediaLibrary, ksSite, targetMediaLibrary) in migratedMediaLibraries)
            {
                string? sourceMediaLibraryPath = null;
                bool loadMediaFileData = false;
                if (!toolkitConfiguration.MigrateOnlyMediaFileInfo.GetValueOrDefault(true) &&
                    !string.IsNullOrWhiteSpace(toolkitConfiguration.KxCmsDirPath))
                {
                    string? cmsMediaLibrariesFolder = KenticoHelper.GetSettingsKey(modelFacade, ksSite.SiteID, "CMSMediaLibrariesFolder");
                    if (cmsMediaLibrariesFolder != null)
                    {
                        if (Path.IsPathRooted(cmsMediaLibrariesFolder))
                        {
                            sourceMediaLibraryPath = Path.Combine(cmsMediaLibrariesFolder, ksSite.SiteName, ksMediaLibrary.LibraryFolder);
                            loadMediaFileData = true;
                        }
                        else
                        {
                            if (cmsMediaLibrariesFolder.StartsWith("~/"))
                            {
                                string cleared = $"{cmsMediaLibrariesFolder[2..]}".Replace("/", "\\");
                                sourceMediaLibraryPath = Path.Combine(toolkitConfiguration.KxCmsDirPath, cleared, ksSite.SiteName, ksMediaLibrary.LibraryFolder);
                                loadMediaFileData = true;
                            }
                            else
                            {
                                sourceMediaLibraryPath = Path.Combine(toolkitConfiguration.KxCmsDirPath, cmsMediaLibrariesFolder, ksSite.SiteName, ksMediaLibrary.LibraryFolder);
                                loadMediaFileData = true;
                            }
                        }
                    }
                    else
                    {
                        sourceMediaLibraryPath = Path.Combine(toolkitConfiguration.KxCmsDirPath, ksSite.SiteName, DirMedia, ksMediaLibrary.LibraryFolder);
                        loadMediaFileData = true;
                    }
                }

                var ksMediaFiles = modelFacade.SelectWhere<IMediaFile>("FileLibraryID = @FileLibraryId", new SqlParameter("FileLibraryId", ksMediaLibrary.LibraryID));

                foreach (var ksMediaFile in ksMediaFiles)
                {
                    protocol.FetchedSource(ksMediaFile);

                    bool found = false;
                    IUploadedFile? uploadedFile = null;
                    if (loadMediaFileData)
                    {
                        (found, uploadedFile) = LoadMediaFileBinary(sourceMediaLibraryPath, ksMediaFile.FilePath, ksMediaFile.FileMimeType);
                        if (!found)
                        {
                            // report missing file (currently reported in mapper)
                        }
                    }

                    string? librarySubfolder = Path.GetDirectoryName(ksMediaFile.FilePath);

                    var kxoMediaFile = mediaFileFacade.GetMediaFile(ksMediaFile.FileGUID);

                    protocol.FetchedTarget(kxoMediaFile);

                    var source = new MediaFileInfoMapperSource(ksMediaFile, targetMediaLibrary.LibraryID, found ? uploadedFile : null,
                        librarySubfolder, toolkitConfiguration.MigrateOnlyMediaFileInfo.GetValueOrDefault(false));
                    var mapped = mediaFileInfoMapper.Map(source, kxoMediaFile);
                    protocol.MappedTarget(mapped);

                    if (mapped is { Success: true } result)
                    {
                        (var mf, bool newInstance) = result;
                        ArgumentNullException.ThrowIfNull(mf, nameof(mf));

                        try
                        {
                            if (newInstance)
                            {
                                mediaFileFacade.EnsureMediaFilePathExistsInLibrary(mf, targetMediaLibrary.LibraryID);
                            }

                            mediaFileFacade.SetMediaFile(mf, newInstance);
                            await kxpContext.SaveChangesAsync(cancellationToken);

                            protocol.Success(ksMediaFile, mf, mapped);
                            logger.LogEntitySetAction(newInstance, mf);
                        }
                        catch (Exception ex)
                        {
                            await kxoDbContext.DisposeAsync(); // reset context errors
                            kxoDbContext = await kxpContextFactory.CreateDbContextAsync(cancellationToken);

                            protocol.Append(HandbookReferences
                                .ErrorCreatingTargetInstance<MediaLibraryInfo>(ex)
                                .NeedsManualAction()
                                .WithIdentityPrint(mf)
                            );
                            logger.LogEntitySetError(ex, newInstance, mf);
                            continue;
                        }

                        primaryKeyMappingContext.SetMapping<MediaFile>(
                            r => r.FileId,
                            ksMediaFile.FileID,
                            mf.FileID
                        );
                    }
                }
            }
        }
        finally
        {
            await kxoDbContext.DisposeAsync();
        }
    }

    private record LoadMediaFileResult(bool Found, IUploadedFile? File);
}
