namespace Migration.Tool.Source.Services.AssetStorage;

/// <summary>
/// Abstracts fast, same-storage-kind file transfers (local file copy, or server-side Azure Blob copy) identified
/// by a storage-relative path (using '/' separators). Mixed local/blob transfers are not supported.
/// </summary>
public interface IAssetStorageProvider
{
    Task<bool> ExistsAsync(string relativePath, CancellationToken cancellationToken);

    /// <summary>
    /// Copies a file/blob directly to <paramref name="targetProvider"/>, which must be the same concrete
    /// provider type, without routing bytes through this process.
    /// </summary>
    /// <returns>false when the source file/blob does not exist.</returns>
    Task<bool> CopyToAsync(string sourceRelativePath, IAssetStorageProvider targetProvider, string targetRelativePath, CancellationToken cancellationToken);
}
