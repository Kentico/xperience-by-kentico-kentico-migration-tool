using Migration.Tool.Common;
using Migration.Tool.Common.Model;

namespace Migration.Tool.Source.Services.AssetStorage;

public static class AssetStorageProviderFactory
{
    public static IAssetStorageProvider CreateSourceProvider(ToolConfiguration toolConfiguration)
    {
        var transferConfig = toolConfiguration.AssetFileTransfer;
        var storageType = transferConfig?.StorageType ?? AssetStorageType.Local;
        return storageType switch
        {
            AssetStorageType.AzureBlobStorage => CreateAzureProvider(transferConfig?.SourceAzureBlobStorage, "SourceAzureBlobStorage"),
            AssetStorageType.Local => CreateLocalProvider(toolConfiguration.KxCmsDirPath, nameof(ToolConfiguration.KxCmsDirPath)),
            _ => throw new InvalidOperationException($"Unsupported media asset storage type '{storageType}'.")
        };
    }

    public static IAssetStorageProvider CreateTargetProvider(ToolConfiguration toolConfiguration)
    {
        var transferConfig = toolConfiguration.AssetFileTransfer;
        var storageType = transferConfig?.StorageType ?? AssetStorageType.Local;
        return storageType switch
        {
            AssetStorageType.AzureBlobStorage => CreateAzureProvider(transferConfig?.TargetAzureBlobStorage, "TargetAzureBlobStorage"),
            AssetStorageType.Local => CreateLocalProvider(toolConfiguration.XbyKDirPath ?? toolConfiguration.XbKDirPath, nameof(ToolConfiguration.XbyKDirPath)),
            _ => throw new InvalidOperationException($"Unsupported media asset storage type '{storageType}'.")
        };
    }

    private static IAssetStorageProvider CreateLocalProvider(string? rootDirectory, string configKeyName)
    {
        if (string.IsNullOrWhiteSpace(rootDirectory))
        {
            throw new InvalidOperationException($"'Settings:{configKeyName}' must be set to use local storage for the media asset transfer command.");
        }

        return new LocalAssetStorageProvider(rootDirectory);
    }

    private static IAssetStorageProvider CreateAzureProvider(AzureBlobStorageConfiguration? config, string configSectionName)
    {
        if (config is null || string.IsNullOrWhiteSpace(config.ConnectionString))
        {
            throw new InvalidOperationException($"'Settings:AssetFileTransfer:{configSectionName}:ConnectionString' must be set to use Azure Blob Storage for the media asset transfer command.");
        }
        if (string.IsNullOrWhiteSpace(config.ContainerName))
        {
            throw new InvalidOperationException($"'Settings:AssetFileTransfer:{configSectionName}:ContainerName' must be set to use Azure Blob Storage for the media asset transfer command.");
        }

        return new AzureBlobAssetStorageProvider(config.ConnectionString, config.ContainerName);
    }
}
