namespace Migration.Toolkit.Core.Handlers;

using CMS.Base;
using CMS.MediaLibrary;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.Mappers;
using Migration.Toolkit.KX13.Context;
using Migration.Toolkit.KX13.Models;
using Migration.Toolkit.KXP.Api;
using Migration.Toolkit.KXP.Api.Auxiliary;
using Migration.Toolkit.KXP.Context;

public class MigrateMediaLibrariesCommandHandler : IRequestHandler<MigrateMediaLibrariesCommand, CommandResult>, IDisposable
{
    private const string DIR_MEDIA = "media";
    private readonly ILogger<MigrateMediaLibrariesCommandHandler> _logger;
    private readonly IDbContextFactory<KxpContext> _kxpContextFactory;
    private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;
    private readonly IEntityMapper<MediaLibrary, MediaLibraryInfo> _mediaLibraryInfoMapper;
    private readonly KxpMediaFileFacade _mediaFileFacade;
    private readonly IEntityMapper<MediaFileInfoMapperSource, MediaFileInfo> _mediaFileInfoMapper;
    private readonly ToolkitConfiguration _toolkitConfiguration;
    private readonly PrimaryKeyMappingContext _primaryKeyMappingContext;
    private readonly IProtocol _protocol;

    private KxpContext _kxpContext;

    public MigrateMediaLibrariesCommandHandler(
        ILogger<MigrateMediaLibrariesCommandHandler> logger,
        IDbContextFactory<KxpContext> kxpContextFactory,
        IDbContextFactory<KX13Context> kx13ContextFactory,
        IEntityMapper<MediaLibrary, MediaLibraryInfo> mediaLibraryInfoMapper,
        KxpMediaFileFacade mediaFileFacade,
        IEntityMapper<MediaFileInfoMapperSource, MediaFileInfo> mediaFileInfoMapper,
        ToolkitConfiguration toolkitConfiguration,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol
    )
    {
        _logger = logger;
        _kxpContextFactory = kxpContextFactory;
        _kx13ContextFactory = kx13ContextFactory;
        _mediaLibraryInfoMapper = mediaLibraryInfoMapper;
        _mediaFileFacade = mediaFileFacade;
        _mediaFileInfoMapper = mediaFileInfoMapper;
        _toolkitConfiguration = toolkitConfiguration;
        _primaryKeyMappingContext = primaryKeyMappingContext;
        _protocol = protocol;
        _kxpContext = kxpContextFactory.CreateDbContext();
    }

    public async Task<CommandResult> Handle(MigrateMediaLibrariesCommand request, CancellationToken cancellationToken)
    {
        await using var kx13Context = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        var kx13MediaLibraries = kx13Context.MediaLibraries
                .Include(ml => ml.LibrarySite)
                .OrderBy(t => t.LibraryId)
            ;

        var migratedMediaLibraries = new List<(MediaLibrary sourceLibrary, MediaLibraryInfo targetLibrary)>();
        foreach (var kx13MediaLibrary in kx13MediaLibraries)
        {
            _protocol.FetchedSource(kx13MediaLibrary);

            if (kx13MediaLibrary.LibraryGuid is not { } mediaLibraryGuid)
            {
                _protocol.Append(HandbookReferences
                    .InvalidSourceData<MediaLibrary>()
                    .WithId(nameof(MediaLibrary.LibraryId), kx13MediaLibrary.LibraryId)
                    .WithMessage("Media library has missing MediaLibraryGUID")
                );
                continue;
            }

            var mediaLibraryInfo = _mediaFileFacade.GetMediaLibraryInfo(mediaLibraryGuid);

            _protocol.FetchedTarget(mediaLibraryInfo);

            var mapped = _mediaLibraryInfoMapper.Map(kx13MediaLibrary, mediaLibraryInfo);
            _protocol.MappedTarget(mapped);

            if (mapped is { Success : true } result)
            {
                var (mfi, newInstance) = result;
                ArgumentNullException.ThrowIfNull(mfi, nameof(mfi));

                try
                {
                    _mediaFileFacade.SetMediaLibrary(mfi);

                    _protocol.Success(kx13MediaLibrary, mfi, mapped);
                    _logger.LogEntitySetAction(newInstance, mfi);
                }
                catch (Exception ex)
                {
                    await _kxpContext.DisposeAsync(); // reset context errors
                    _kxpContext = await _kxpContextFactory.CreateDbContextAsync(cancellationToken);

                    _protocol.Append(HandbookReferences
                        .ErrorCreatingTargetInstance<MediaLibraryInfo>(ex)
                        .NeedsManualAction()
                        .WithIdentityPrint(mfi)
                    );
                    _logger.LogEntitySetError(ex, newInstance, mfi);
                    continue;
                }

                _primaryKeyMappingContext.SetMapping<MediaLibrary>(
                    r => r.LibraryId,
                    kx13MediaLibrary.LibraryId,
                    mfi.LibraryID
                );

                migratedMediaLibraries.Add((kx13MediaLibrary, mfi));
            }
        }

        await RequireMigratedMediaFiles(migratedMediaLibraries, kx13Context, cancellationToken);

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
        KX13Context kx13Context, CancellationToken cancellationToken)
    {
        var kxoDbContext = await _kxpContextFactory.CreateDbContextAsync(cancellationToken);
        try
        {
            foreach (var (sourceMediaLibrary, targetMediaLibrary) in migratedMediaLibraries)
            {
                string? sourceMediaLibraryPath = null;
                var loadMediaFileData = false;
                if (!_toolkitConfiguration.MigrateOnlyMediaFileInfo.GetValueOrDefault(true) &&
                    !string.IsNullOrWhiteSpace(_toolkitConfiguration.KxCmsDirPath))
                {
                    sourceMediaLibraryPath = Path.Combine(_toolkitConfiguration.KxCmsDirPath, sourceMediaLibrary.LibrarySite.SiteName, DIR_MEDIA, sourceMediaLibrary.LibraryFolder);
                    loadMediaFileData = true;
                }

                var kx13MediaFiles = kx13Context.MediaFiles
                    .Where(x => x.FileLibraryId == sourceMediaLibrary.LibraryId);

                foreach (var kx13MediaFile in kx13MediaFiles)
                {
                    _protocol.FetchedSource(kx13MediaFile);

                    bool found = false;
                    IUploadedFile? uploadedFile = null;
                    if (loadMediaFileData)
                    {
                        (found, uploadedFile) = LoadMediaFileBinary(sourceMediaLibraryPath, kx13MediaFile.FilePath, kx13MediaFile.FileMimeType);
                        if (!found)
                        {
                            // TODO tk: 2022-07-07 report missing file (currently reported in mapper)
                        }
                    }

                    var librarySubfolder = Path.GetDirectoryName(kx13MediaFile.FilePath);

                    var kxoMediaFile = _mediaFileFacade.GetMediaFile(kx13MediaFile.FileGuid);

                    _protocol.FetchedTarget(kxoMediaFile);

                    var source = new MediaFileInfoMapperSource(kx13MediaFile, targetMediaLibrary.LibraryID, found ? uploadedFile : null,
                        librarySubfolder, _toolkitConfiguration.MigrateOnlyMediaFileInfo.GetValueOrDefault(false));
                    var mapped = _mediaFileInfoMapper.Map(source, kxoMediaFile);
                    _protocol.MappedTarget(mapped);

                    if (mapped is { Success : true } result)
                    {
                        var (mf, newInstance) = result;
                        ArgumentNullException.ThrowIfNull(mf, nameof(mf));

                        try
                        {
                            if (newInstance)
                            {
                                _mediaFileFacade.EnsureMediaFilePathExistsInLibrary(mf, targetMediaLibrary.LibraryID);
                            }

                            _mediaFileFacade.SetMediaFile(mf, newInstance);
                            await _kxpContext.SaveChangesAsync(cancellationToken);

                            _protocol.Success(kx13MediaFile, mf, mapped);
                            _logger.LogEntitySetAction(newInstance, mf);
                        }
                        catch (Exception ex) // TODO tk: 2022-05-18 handle exceptions
                        {
                            await kxoDbContext.DisposeAsync(); // reset context errors
                            kxoDbContext = await _kxpContextFactory.CreateDbContextAsync(cancellationToken);

                            _protocol.Append(HandbookReferences
                                .ErrorCreatingTargetInstance<MediaLibraryInfo>(ex)
                                .NeedsManualAction()
                                .WithIdentityPrint(mf)
                            );
                            _logger.LogEntitySetError(ex, newInstance, mf);
                            continue;
                        }

                        _primaryKeyMappingContext.SetMapping<MediaFile>(
                            r => r.FileId,
                            kx13MediaFile.FileId,
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