namespace Migration.Tool.Source.Helpers;

/// <summary>
/// Builds the storage-relative path (using '/' separators) of a content item asset file, matching Xperience by
/// Kentico's own <c>ContentItemAssetPathProvider</c> algorithm:
/// <c>{commonRootDirectoryName}/contentitems/{first 2 chars of content item GUID}/{content item GUID}/{field GUID}/{asset identifier GUID}{extension}</c>.
/// </summary>
public static class ContentItemAssetPathHelper
{
    private const string ContentItemsRoot = "contentitems";

    /// <summary>
    /// Default value of Xperience by Kentico's <c>AssetOptions.CommonRootDirectoryName</c>.
    /// </summary>
    public const string DefaultCommonRootDirectoryName = "assets";

    public static string GetRelativePath(Guid contentItemGuid, Guid fieldGuid, Guid assetIdentifier, string extension, string commonRootDirectoryName = DefaultCommonRootDirectoryName)
    {
        string contentItemGuidString = contentItemGuid.ToString();
        string normalizedExtension = extension.StartsWith('.') ? extension : $".{extension}";

        var parts = new List<string>();
        if (!string.IsNullOrEmpty(commonRootDirectoryName))
        {
            parts.Add(commonRootDirectoryName);
        }

        parts.Add(ContentItemsRoot);
        parts.Add(contentItemGuidString[..2]);
        parts.Add(contentItemGuidString);
        parts.Add(fieldGuid.ToString());
        parts.Add($"{assetIdentifier}{normalizedExtension}");

        return string.Join('/', parts);
    }
}
