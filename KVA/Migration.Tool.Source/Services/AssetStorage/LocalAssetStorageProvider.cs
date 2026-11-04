namespace Migration.Tool.Source.Services.AssetStorage;

/// <summary>
/// Copies files on the local file system, rooted at the given directory.
/// </summary>
public class LocalAssetStorageProvider(string rootDirectory) : IAssetStorageProvider
{
    public Task<bool> ExistsAsync(string relativePath, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(File.Exists(GetFullPath(relativePath)));
    }

    public Task<bool> CopyToAsync(string sourceRelativePath, IAssetStorageProvider targetProvider, string targetRelativePath, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (targetProvider is not LocalAssetStorageProvider localTarget)
        {
            throw new InvalidOperationException($"{nameof(LocalAssetStorageProvider)} can only copy to another {nameof(LocalAssetStorageProvider)}.");
        }

        string sourceFullPath = GetFullPath(sourceRelativePath);
        if (!File.Exists(sourceFullPath))
        {
            return Task.FromResult(false);
        }

        string targetFullPath = localTarget.GetFullPath(targetRelativePath);
        string? directory = Path.GetDirectoryName(targetFullPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // copy to a temp file then rename atomically, so a re-run never mistakes a partial file for a complete one
        string tempFullPath = $"{targetFullPath}.partial";
        bool moved = false;
        try
        {
            File.Copy(sourceFullPath, tempFullPath, overwrite: true);
            cancellationToken.ThrowIfCancellationRequested();
            File.Move(tempFullPath, targetFullPath, overwrite: true);
            moved = true;
        }
        finally
        {
            if (!moved && File.Exists(tempFullPath))
            {
                File.Delete(tempFullPath);
            }
        }

        return Task.FromResult(true);
    }

    private string GetFullPath(string relativePath) => Path.Combine(rootDirectory, relativePath.Replace('/', Path.DirectorySeparatorChar));
}
