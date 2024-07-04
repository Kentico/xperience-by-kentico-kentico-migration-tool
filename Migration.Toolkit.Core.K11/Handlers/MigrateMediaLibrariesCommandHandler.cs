namespace Migration.Toolkit.Core.K11.Handlers;

using CMS.Base;
using CMS.MediaLibrary;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.K11.Contexts;
using Migration.Toolkit.Core.K11.Mappers;
using Migration.Toolkit.K11;
using Migration.Toolkit.K11.Models;
using Migration.Toolkit.KXP.Api;
using Migration.Toolkit.KXP.Api.Auxiliary;
using Migration.Toolkit.KXP.Context;

public class MigrateMediaLibrariesCommandHandler(ILogger<MigrateMediaLibrariesCommandHandler> logger,
        IDbContextFactory<KxpContext> kxpContextFactory,
        IDbContextFactory<K11Context> k11ContextFactory,
        IEntityMapper<MediaLibrary, MediaLibraryInfo> mediaLibraryInfoMapper,
        KxpMediaFileFacade mediaFileFacade,
        IEntityMapper<MediaFileInfoMapperSource, MediaFileInfo> mediaFileInfoMapper,
        ToolkitConfiguration toolkitConfiguration,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol)
    : IRequestHandler<MigrateMediaLibrariesCommand, CommandResult>, IDisposable
{
    private const string DIR_MEDIA = "media";

    private KxpContext _kxpContext = kxpContextFactory.CreateDbContext();

    public async Task<CommandResult> Handle(MigrateMediaLibrariesCommand request, CancellationToken cancellationToken)
    {
        await using var k11Context = await k11ContextFactory.CreateDbContextAsync(cancellationToken);

        var skippedMediaLibraries = new HashSet<Guid>();
        var unsuitableMediaLibraries = k11Context.MediaLibraries
            .Include(ml => ml.LibrarySite)
            .GroupBy(l => l.LibraryName);

        foreach (var mlg in unsuitableMediaLibraries)
        {
            if (mlg.Count() <= 1) continue;

            logger.LogError("""
                            Media libraries with LibraryGuid ({LibraryGuids}) have same LibraryName '{LibraryName}', due to removal of sites and media library globalization it is required to set unique LibraryName and LibraryFolder
                            """, string.Join(",", mlg.Select(l => l.LibraryGuid)), mlg.Key);

            foreach (var ml in mlg)
            {
                if (ml.LibraryGuid is { } libraryGuid)
                {
                    skippedMediaLibraries.Add(libraryGuid);
                }

                protocol.Append(HandbookReferences.NotCurrentlySupportedSkip()
                    .WithMessage($"Media library '{ml.LibraryName}' with LibraryGuid '{ml.LibraryGuid}' doesn't satisfy unique LibraryName and LibraryFolder condition for migration")
                    .WithIdentityPrint(ml)
                    .WithData(new { ml.LibraryGuid, ml.LibraryName, ml.LibrarySiteId, ml.LibraryFolder })
                );
            }
        }

        var k11MediaLibraries = k11Context.MediaLibraries
                .Include(ml => ml.LibrarySite)
                .OrderBy(t => t.LibraryId)
            ;

        var migratedMediaLibraries = new List<(MediaLibrary sourceLibrary, MediaLibraryInfo targetLibrary)>();
        foreach (var k11MediaLibrary in k11MediaLibraries)
        {
            if (k11MediaLibrary.LibraryGuid is { } libraryGuid && skippedMediaLibraries.Contains(libraryGuid))
            {
                continue;
            }

            protocol.FetchedSource(k11MediaLibrary);

            if (k11MediaLibrary.LibraryGuid is not { } mediaLibraryGuid)
            {
                protocol.Append(HandbookReferences
                    .InvalidSourceData<MediaLibrary>()
                    .WithId(nameof(MediaLibrary.LibraryId), k11MediaLibrary.LibraryId)
                    .WithMessage("Media library has missing MediaLibraryGUID")
                );
                continue;
            }

            var mediaLibraryInfo = mediaFileFacade.GetMediaLibraryInfo(mediaLibraryGuid);

            protocol.FetchedTarget(mediaLibraryInfo);

            var mapped = mediaLibraryInfoMapper.Map(k11MediaLibrary, mediaLibraryInfo);
            protocol.MappedTarget(mapped);

            if (mapped is { Success: true } result)
            {
                var (mfi, newInstance) = result;
                ArgumentNullException.ThrowIfNull(mfi, nameof(mfi));

                try
                {
                    mediaFileFacade.SetMediaLibrary(mfi);

                    protocol.Success(k11MediaLibrary, mfi, mapped);
                    logger.LogEntitySetAction(newInstance, mfi);
                }
                catch (Exception ex)
                {
                    await _kxpContext.DisposeAsync(); // reset context errors
                    _kxpContext = await kxpContextFactory.CreateDbContextAsync(cancellationToken);

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
                    k11MediaLibrary.LibraryId,
                    mfi.LibraryID
                );

                migratedMediaLibraries.Add((k11MediaLibrary, mfi));
            }
        }

        await RequireMigratedMediaFiles(migratedMediaLibraries, k11Context, cancellationToken);

        return new GenericCommandResult();
    }

    private record LoadMediaFileResult(bool Found, IUploadedFile? File);
    private LoadMediaFileResult LoadMediaFileBinary(string? sourceMediaLibraryPath, string relativeFilePath, string contentType)
    {
        if (sourceMediaLibraryPath == null)
        {
            return new LoadMediaFileResult(false, null);
        }

        var filePath = Path.Combine(sourceMediaLibraryPath, relativeFilePath);
        if (File.Exists(filePath))
        {
            var data = File.ReadAllBytes(filePath);
            var dummyFile = DummyUploadedFile.FromByteArray(data, contentType, data.LongLength, Path.GetFileName(filePath));
            return new LoadMediaFileResult(true, dummyFile);
        }

        return new LoadMediaFileResult(false, null);
    }

    private async Task RequireMigratedMediaFiles(
        List<(MediaLibrary sourceLibrary, MediaLibraryInfo targetLibrary)> migratedMediaLibraries,
        K11Context k11Context, CancellationToken cancellationToken)
    {
        var kxoDbContext = await kxpContextFactory.CreateDbContextAsync(cancellationToken);
        try
        {
            foreach (var (sourceMediaLibrary, targetMediaLibrary) in migratedMediaLibraries)
            {
                string? sourceMediaLibraryPath = null;
                var loadMediaFileData = false;
                if (!toolkitConfiguration.MigrateOnlyMediaFileInfo.GetValueOrDefault(true) &&
                    !string.IsNullOrWhiteSpace(toolkitConfiguration.KxCmsDirPath))
                {
                    sourceMediaLibraryPath = Path.Combine(toolkitConfiguration.KxCmsDirPath, sourceMediaLibrary.LibrarySite.SiteName, DIR_MEDIA, sourceMediaLibrary.LibraryFolder);
                    loadMediaFileData = true;
                }

                var k11MediaFiles = k11Context.MediaFiles
                    .Where(x => x.FileLibraryId == sourceMediaLibrary.LibraryId);

                foreach (var k11MediaFile in k11MediaFiles)
                {
                    protocol.FetchedSource(k11MediaFile);

                    bool found = false;
                    IUploadedFile? uploadedFile = null;
                    if (loadMediaFileData)
                    {
                        (found, uploadedFile) = LoadMediaFileBinary(sourceMediaLibraryPath, k11MediaFile.FilePath, k11MediaFile.FileMimeType);
                        if (!found)
                        {
                            // TODO tk: 2022-07-07 report missing file (currently reported in mapper)
                        }
                    }

                    var librarySubfolder = Path.GetDirectoryName(k11MediaFile.FilePath);

                    var kxoMediaFile = mediaFileFacade.GetMediaFile(k11MediaFile.FileGuid);

                    protocol.FetchedTarget(kxoMediaFile);

                    var source = new MediaFileInfoMapperSource(k11MediaFile, targetMediaLibrary.LibraryID, found ? uploadedFile : null,
                        librarySubfolder, toolkitConfiguration.MigrateOnlyMediaFileInfo.GetValueOrDefault(false));
                    var mapped = mediaFileInfoMapper.Map(source, kxoMediaFile);
                    protocol.MappedTarget(mapped);

                    if (mapped is { Success: true } result)
                    {
                        var (mf, newInstance) = result;
                        ArgumentNullException.ThrowIfNull(mf, nameof(mf));

                        try
                        {
                            if (newInstance)
                            {
                                mediaFileFacade.EnsureMediaFilePathExistsInLibrary(mf, targetMediaLibrary.LibraryID);
                            }

                            mediaFileFacade.SetMediaFile(mf, newInstance);
                            await _kxpContext.SaveChangesAsync(cancellationToken);

                            protocol.Success(k11MediaFile, mf, mapped);
                            logger.LogEntitySetAction(newInstance, mf);
                        }
                        catch (Exception ex) // TODO tk: 2022-05-18 handle exceptions
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
                            k11MediaFile.FileId,
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

    public void Dispose()
    {
        _kxpContext.Dispose();
    }
}