namespace Migration.Tool.Common.Model;

/// <summary>
/// Type of storage used to read source media files from or write target content item asset files to.
/// </summary>
public enum AssetStorageType
{
    Local,
    AzureBlobStorage
}

/// <summary>
/// Configuration for the "transfer-media-assets" command, which copies legacy media library files from
/// their source location to the file/blob structure expected by Xperience by Kentico content item assets.
/// Source and target must use the same storage type - mixed local/Azure Blob transfers are not supported,
/// as fast copying (local file copy, server-side blob copy) requires both sides to be the same kind of storage.
/// </summary>
public class AssetFileTransferConfiguration
{
    /// <summary>
    /// Storage type used for both source and target. Defaults to <see cref="AssetStorageType.Local"/>
    /// (<see cref="ToolConfiguration.KxCmsDirPath"/> / <see cref="ToolConfiguration.XbyKDirPath"/>).
    /// </summary>
    public AssetStorageType StorageType { get; set; } = AssetStorageType.Local;

    /// <summary>
    /// Required when <see cref="StorageType"/> is <see cref="AssetStorageType.AzureBlobStorage"/>. Identifies the source storage account/container.
    /// </summary>
    public AzureBlobStorageConfiguration? SourceAzureBlobStorage { get; set; }

    /// <summary>
    /// Required when <see cref="StorageType"/> is <see cref="AssetStorageType.AzureBlobStorage"/>. Identifies the target storage account/container.
    /// </summary>
    public AzureBlobStorageConfiguration? TargetAzureBlobStorage { get; set; }

    /// <summary>
    /// If true, existing target files/blobs are overwritten. Defaults to false (existing targets are skipped).
    /// </summary>
    public bool ForceOverwrite { get; set; } = false;
}

public class AzureBlobStorageConfiguration
{
    public string ConnectionString { get; set; } = "";

    /// <summary>
    /// Defaults to "cmsstorage", which matches Kentico's default Azure Storage container name (<c>ContainerInfoProvider.CMS_STORAGE</c>).
    /// </summary>
    public string ContainerName { get; set; } = "cmsstorage";
}
