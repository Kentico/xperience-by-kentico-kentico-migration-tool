namespace Migration.Toolkit.Core.KX12.Handlers;

using CMS.Base;
using CMS.MediaLibrary;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.KX12.Contexts;
using Migration.Toolkit.Core.KX12.Mappers;
using Migration.Toolkit.KX12.Context;
using Migration.Toolkit.KX12.Models;
using Migration.Toolkit.KXP.Api;
using Migration.Toolkit.KXP.Api.Auxiliary;
using Migration.Toolkit.KXP.Context;

public class MigrateMediaLibrariesCommandHandler : IRequestHandler<MigrateMediaLibrariesCommand, CommandResult>, IDisposable
{
    private const string DIR_MEDIA = "media";
    private readonly ILogger<MigrateMediaLibrariesCommandHandler> _logger;
    private readonly IDbContextFactory<KxpContext> _kxpContextFactory;
    private readonly IDbContextFactory<KX12Context> _kx12ContextFactory;
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
        IDbContextFactory<KX12Context> kx12ContextFactory,
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
        _kx12ContextFactory = kx12ContextFactory;
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
        await using var kx12Context = await _kx12ContextFactory.CreateDbContextAsync(cancellationToken);

        var k12MediaLibraries = kx12Context.MediaLibraries
                .Include(ml => ml.LibrarySite)
                .OrderBy(t => t.LibraryId)
            ;

        var migratedMediaLibraries = new List<(MediaLibrary sourceLibrary, MediaLibraryInfo targetLibrary)>();
        foreach (var k12MediaLibrary in k12MediaLibraries)
        {
            _protocol.FetchedSource(k12MediaLibrary);

            if (k12MediaLibrary.LibraryGuid is not { } mediaLibraryGuid)
            {
                _protocol.Append(HandbookReferences
                    .InvalidSourceData<MediaLibrary>()
                    .WithId(nameof(MediaLibrary.LibraryId), k12MediaLibrary.LibraryId)
                    .WithMessage("Media library has missing MediaLibraryGUID")
                );
                continue;
            }

            var mediaLibraryInfo = _mediaFileFacade.GetMediaLibraryInfo(mediaLibraryGuid);

            _protocol.FetchedTarget(mediaLibraryInfo);

            var mapped = _mediaLibraryInfoMapper.Map(k12MediaLibrary, mediaLibraryInfo);
            _protocol.MappedTarget(mapped);

            if (mapped is { Success : true } result)
            {
                var (mfi, newInstance) = result;
                ArgumentNullException.ThrowIfNull(mfi, nameof(mfi));

                try
                {
                    _mediaFileFacade.SetMediaLibrary(mfi);

                    _protocol.Success(k12MediaLibrary, mfi, mapped);
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
                    k12MediaLibrary.LibraryId,
                    mfi.LibraryID
                );

                migratedMediaLibraries.Add((k12MediaLibrary, mfi));
            }
        }

        await RequireMigratedMediaFiles(migratedMediaLibraries, kx12Context, cancellationToken);

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
        KX12Context KX12Context, CancellationToken cancellationToken)
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

                var k12MediaFiles = KX12Context.MediaFiles
                    .Where(x => x.FileLibraryId == sourceMediaLibrary.LibraryId);

                foreach (var k12MediaFile in k12MediaFiles)
                {
                    _protocol.FetchedSource(k12MediaFile);

                    bool found = false;
                    IUploadedFile? uploadedFile = null;
                    if (loadMediaFileData)
                    {
                        (found, uploadedFile) = LoadMediaFileBinary(sourceMediaLibraryPath, k12MediaFile.FilePath, k12MediaFile.FileMimeType);
                        if (!found)
                        {
                            // TODO tk: 2022-07-07 report missing file (currently reported in mapper)
                        }
                    }

                    var librarySubfolder = Path.GetDirectoryName(k12MediaFile.FilePath);

                    var kxoMediaFile = _mediaFileFacade.GetMediaFile(k12MediaFile.FileGuid);

                    _protocol.FetchedTarget(kxoMediaFile);

                    var source = new MediaFileInfoMapperSource(k12MediaFile, targetMediaLibrary.LibraryID, found ? uploadedFile : null,
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

                            _protocol.Success(k12MediaFile, mf, mapped);
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
                            k12MediaFile.FileId,
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