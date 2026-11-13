using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace Migration.Tool.Source.Services.AssetStorage;

/// <summary>
/// Copies blobs within an Azure Blob Storage container using server-side ("copy from URL") transfers,
/// so blob content never passes through this process regardless of file size.
/// </summary>
public class AzureBlobAssetStorageProvider : IAssetStorageProvider
{
    private static readonly TimeSpan sourceSasLifetime = TimeSpan.FromHours(24);
    private readonly BlobContainerClient containerClient;
    private int containerEnsured;

    public AzureBlobAssetStorageProvider(string connectionString, string containerName) => containerClient = new BlobContainerClient(connectionString, containerName);

    public async Task<bool> ExistsAsync(string relativePath, CancellationToken cancellationToken)
    {
        var blobClient = containerClient.GetBlobClient(NormalizeBlobName(relativePath));

        // Blobs uploaded normally have no copy ID; interrupted copies are not complete targets.
        try
        {
            var properties = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken);
            return properties.Value.CopyId is null || properties.Value.CopyStatus == CopyStatus.Success;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return false;
        }
    }

    public async Task<bool> CopyToAsync(string sourceRelativePath, IAssetStorageProvider targetProvider, string targetRelativePath, CancellationToken cancellationToken)
    {
        if (targetProvider is not AzureBlobAssetStorageProvider azureTarget)
        {
            throw new InvalidOperationException($"{nameof(AzureBlobAssetStorageProvider)} can only copy to another {nameof(AzureBlobAssetStorageProvider)}.");
        }

        var sourceBlobClient = containerClient.GetBlobClient(NormalizeBlobName(sourceRelativePath));
        if (!await sourceBlobClient.ExistsAsync(cancellationToken))
        {
            return false;
        }

        if (!sourceBlobClient.CanGenerateSasUri)
        {
            throw new InvalidOperationException(
                "Unable to generate a read SAS URI for the source blob. Configure 'AssetFileTransfer:SourceAzureBlobStorage:ConnectionString' " +
                "with an account key (not a SAS-only connection string) so the target storage account can authenticate the server-side copy.");
        }

        await azureTarget.EnsureContainerAsync(cancellationToken);
        var targetBlobClient = azureTarget.containerClient.GetBlobClient(NormalizeBlobName(targetRelativePath));

        var sourceUri = sourceBlobClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.Add(sourceSasLifetime));
        try
        {
            var copyOperation = await targetBlobClient.StartCopyFromUriAsync(sourceUri, cancellationToken: cancellationToken);
            await copyOperation.WaitForCompletionAsync(cancellationToken);
        }
        catch (RequestFailedException ex) when (ex.ErrorCode == "PendingCopyOperation")
        {
            // a stale pending copy from an interrupted previous run is blocking this one - abort it and retry
            var properties = await targetBlobClient.GetPropertiesAsync(cancellationToken: cancellationToken);
            if (properties.Value.CopyId is { } staleCopyId)
            {
                await targetBlobClient.AbortCopyFromUriAsync(staleCopyId, cancellationToken: cancellationToken);
            }

            var copyOperation = await targetBlobClient.StartCopyFromUriAsync(sourceUri, cancellationToken: cancellationToken);
            await copyOperation.WaitForCompletionAsync(cancellationToken);
        }

        return true;
    }

    // CreateIfNotExists is idempotent, so the flag only avoids repeating the request once it has succeeded.
    private async Task EnsureContainerAsync(CancellationToken cancellationToken)
    {
        if (Volatile.Read(ref containerEnsured) == 1)
        {
            return;
        }

        await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        Volatile.Write(ref containerEnsured, 1);
    }

    private static string NormalizeBlobName(string relativePath) => relativePath.Replace('\\', '/').TrimStart('/');
}
