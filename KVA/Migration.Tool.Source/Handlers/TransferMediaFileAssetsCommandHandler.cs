using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.Helpers;
using Migration.Tool.Common.Model;
using Migration.Tool.Source.Auxiliary;
using Migration.Tool.Source.Helpers;
using Migration.Tool.Source.Model;
using Migration.Tool.Source.Services;
using Migration.Tool.Source.Services.AssetStorage;

namespace Migration.Tool.Source.Handlers;

/// <summary>
/// Copies media file bytes from their source location to the file/blob structure expected by content item assets.
/// Used after <see cref="MigrateMediaLibrariesCommand"/> when <see cref="ToolConfiguration.MigrateOnlyMediaFileInfo"/> was set.
/// </summary>
public class TransferMediaFileAssetsCommandHandler(
    ILogger<TransferMediaFileAssetsCommandHandler> logger,
    ModelFacade modelFacade,
    ToolConfiguration toolConfiguration,
    EntityIdentityFacade entityIdentityFacade,
    IAssetFacade assetFacade
    ) : IRequestHandler<TransferMediaFileAssetsCommand, CommandResult>
{
    private static readonly TimeSpan progressLogInterval = TimeSpan.FromSeconds(10);

    private const int AzureParallelism = 8;

    public async Task<CommandResult> Handle(TransferMediaFileAssetsCommand request, CancellationToken cancellationToken)
    {
        if (toolConfiguration.MigrateMediaToMediaLibrary)
        {
            throw new InvalidOperationException(
                $"'{nameof(TransferMediaFileAssetsCommand)}' only supports migrating media files as content item assets. " +
                $"Set '{nameof(ToolConfiguration.MigrateMediaToMediaLibrary)}' to false, or migrate media file bytes " +
                $"inline (set '{nameof(ToolConfiguration.MigrateOnlyMediaFileInfo)}' to false) instead of using this command.");
        }

        var transferConfig = toolConfiguration.AssetFileTransfer ?? new AssetFileTransferConfiguration();

        var sourceProvider = AssetStorageProviderFactory.CreateSourceProvider(toolConfiguration);
        var targetProvider = AssetStorageProviderFactory.CreateTargetProvider(toolConfiguration);

        string contentLanguageName = assetFacade.DefaultContentLanguage;
        if (AssetFacade.LegacyMediaFileAssetField.Guid is not Guid fieldGuid)
        {
            throw new InvalidOperationException("LegacyMediaFileAssetField.Guid is not set.");
        }

        var ksMediaFiles = modelFacade.SelectAll<IMediaFile>(" ORDER BY FileLibraryID").ToList();
        var ksSites = ksMediaFiles
            .Select(file => file.FileSiteID)
            .Distinct()
            .ToDictionary(id => id, id => modelFacade.SelectById<ICmsSite>(id));
        var ksMediaLibraries = ksMediaFiles
            .Select(file => file.FileLibraryID)
            .Distinct()
            .ToDictionary(id => id, id => modelFacade.SelectById<IMediaLibrary>(id));
        var sourceLibraryPaths = ksMediaFiles
            .Select(file => (file.FileSiteID, file.FileLibraryID))
            .Distinct()
            .Where(ids => ksSites[ids.FileSiteID] is not null && ksMediaLibraries[ids.FileLibraryID] is not null)
            .ToDictionary(
                ids => ids,
                ids => GetSourceLibraryRelativePath(transferConfig.StorageType, ksSites[ids.FileSiteID]!, ksMediaLibraries[ids.FileLibraryID]!));
        var targetPaths = ksMediaFiles.ToDictionary(
            file => file.FileID,
            file =>
            {
                var (_, translatedMediaGuid) = entityIdentityFacade.Translate(file);
                var assetGuid = GuidHelper.CreateAssetGuid(translatedMediaGuid, contentLanguageName);
                return ContentItemAssetPathHelper.GetRelativePath(translatedMediaGuid, fieldGuid, assetGuid, file.FileExtension);
            });
        var duplicateTargetPath = targetPaths.Values
            .GroupBy(path => path, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault(group => group.Count() > 1)?.Key;
        if (duplicateTargetPath is not null)
        {
            throw new InvalidOperationException($"Multiple media files resolve to target path '{duplicateTargetPath}'. Resolve duplicate media file GUIDs before transferring assets.");
        }

        int totalFiles = ksMediaFiles.Count;
        logger.LogInformation("Starting media asset transfer for {Total} media files", totalFiles);

        int transferred = 0, skippedExisting = 0, missingSource = 0, failed = 0, processed = 0;
        var stopwatch = Stopwatch.StartNew();
        var lastProgressLog = TimeSpan.Zero;
        var progressLock = new object();
        // also log every N files, so fast runs with many files still show interim progress
        int progressLogFileInterval = Math.Max(10, totalFiles / 20);

        async ValueTask ProcessFileAsync(IMediaFile ksMediaFile, CancellationToken ct)
        {
            try
            {
                if (ksSites[ksMediaFile.FileSiteID] is null)
                {
                    logger.LogError("Media file '{File}' site not found", ksMediaFile.FileGUID);
                    Interlocked.Increment(ref failed);
                    return;
                }

                if (ksMediaLibraries[ksMediaFile.FileLibraryID] is null)
                {
                    logger.LogError("Media file '{File}' library not found", ksMediaFile.FileGUID);
                    Interlocked.Increment(ref failed);
                    return;
                }

                string sourceRelativePath = Path.Combine(
                    sourceLibraryPaths[(ksMediaFile.FileSiteID, ksMediaFile.FileLibraryID)],
                    ksMediaFile.FilePath).Replace('\\', '/');
                if (transferConfig.StorageType == AssetStorageType.AzureBlobStorage)
                {
                    sourceRelativePath = sourceRelativePath.ToLowerInvariant();
                }

                string targetRelativePath = targetPaths[ksMediaFile.FileID];

                if (!transferConfig.ForceOverwrite && await targetProvider.ExistsAsync(targetRelativePath, ct))
                {
                    logger.LogInformation("Media file '{File}' already exists at target '{TargetPath}', skipping", ksMediaFile.FileGUID, targetRelativePath);
                    Interlocked.Increment(ref skippedExisting);
                    return;
                }

                bool copied = await sourceProvider.CopyToAsync(sourceRelativePath, targetProvider, targetRelativePath, ct);
                if (!copied)
                {
                    logger.LogWarning("Source file for media file '{File}' not found at '{SourcePath}'", ksMediaFile.FileGUID, sourceRelativePath);
                    Interlocked.Increment(ref missingSource);
                    return;
                }

                logger.LogInformation("Transferred media file '{File}' to '{TargetPath}'", ksMediaFile.FileGUID, targetRelativePath);
                Interlocked.Increment(ref transferred);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to transfer media file '{File}'", ksMediaFile.FileGUID);
                Interlocked.Increment(ref failed);
            }
            finally
            {
                lock (progressLock)
                {
                    processed++;
                    if (processed == totalFiles || processed % progressLogFileInterval == 0 || stopwatch.Elapsed - lastProgressLog >= progressLogInterval)
                    {
                        lastProgressLog = stopwatch.Elapsed;
                        var averagePerFile = stopwatch.Elapsed / processed;
                        var remaining = averagePerFile * (totalFiles - processed);
                        logger.LogInformation(
                            "Media asset transfer progress: {Processed}/{Total} ({Percentage:F1}%), elapsed {Elapsed}, estimated remaining {Remaining}",
                            processed, totalFiles, processed * 100.0 / totalFiles,
                            TimeSpanFormatHelper.Round(stopwatch.Elapsed), TimeSpanFormatHelper.Round(remaining));
                    }
                }
            }
        }

        if (transferConfig.StorageType == AssetStorageType.AzureBlobStorage)
        {
            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = AzureParallelism, CancellationToken = cancellationToken };
            await Parallel.ForEachAsync(ksMediaFiles, parallelOptions, ProcessFileAsync);
        }
        else
        {
            foreach (var ksMediaFile in ksMediaFiles)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await ProcessFileAsync(ksMediaFile, cancellationToken);
            }
        }

        logger.LogInformation(
            "Media asset transfer finished. Transferred: {Transferred}, skipped (already existing at target): {Skipped}, missing source file: {Missing}, failed: {Failed}",
            transferred, skippedExisting, missingSource, failed);

        return new GenericCommandResult();
    }

    private string GetSourceLibraryRelativePath(AssetStorageType sourceStorageType, ICmsSite site, IMediaLibrary library)
    {
        if (string.IsNullOrWhiteSpace(toolConfiguration.KxCmsDirPath))
        {
            throw new InvalidOperationException($"'Settings:{nameof(ToolConfiguration.KxCmsDirPath)}' must be set to determine the relative path of source media files.");
        }

        string absoluteLibraryPath = AssetFacade.GetMediaLibraryAbsolutePathUnconditional(toolConfiguration.KxCmsDirPath, site, library, modelFacade);
        string relativePath = Path.GetRelativePath(toolConfiguration.KxCmsDirPath, absoluteLibraryPath).Replace('\\', '/');

        // Kentico's Azure Storage provider uses this same relative path as the blob name, lower-cased (verified against a live container).
        return sourceStorageType == AssetStorageType.AzureBlobStorage ? relativePath.ToLowerInvariant() : relativePath;
    }
}

static file class TimeSpanFormatHelper
{
    public static TimeSpan Round(TimeSpan value) => TimeSpan.FromSeconds(Math.Round(value.TotalSeconds));
}
