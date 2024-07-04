namespace Migration.Toolkit.Common.Helpers;

using CMS.ContentEngine.Internal;
using CMS.Websites.Internal;

/// <summary>
/// Provides unique tree path.
/// </summary>
internal class UniqueTreePathProvider : UniqueStringValueProviderBase
{
    private readonly int websiteChannelId;
    private readonly ITreePathValidator treePathValidator;
    private readonly int maxTreePathSegmentLength;


    /// <summary>
    /// Creates a new instance of <see cref="UniqueTreePathProvider"/>.
    /// </summary>
    /// <param name="websiteChannelId">Website channel ID.</param>
    /// <param name="treePathValidator">Tree path validator.</param>
    /// <param name="maxTreePathLength">Maximum length of whole tree path. Default to <see cref="TreePathConstants.MAX_PATH_LENGTH"/>.</param>
    /// <param name="maxTreePathSegmentLength">Maximum length of a tree path segment without '/'. Defaults to <see cref="TreePathConstants.MAX_SLUG_LENGTH"/>.</param>
    public UniqueTreePathProvider(int websiteChannelId, ITreePathValidator treePathValidator, int maxTreePathLength = TreePathConstants.MAX_PATH_LENGTH,
        int maxTreePathSegmentLength = TreePathConstants.MAX_SLUG_LENGTH)
        : base(maxTreePathLength)
    {
        this.websiteChannelId = websiteChannelId;
        this.treePathValidator = treePathValidator;
        this.maxTreePathSegmentLength = maxTreePathSegmentLength;
    }


    ///<inheritdoc/>
    protected override Task<bool> IsValueUnique(string value)
    {
        return treePathValidator.IsUnique(websiteChannelId, value);
    }


    ///<inheritdoc/>
    protected override string AddRandomSuffix(string source, string randomSuffix)
    {
        var lastSegment = TreePathUtils.GetLastPathSegment(source);
        var lastSegmentWithSuffix = $"{lastSegment}{randomSuffix}";

        var modifiedPath = $"{source}{randomSuffix}";

        if (lastSegmentWithSuffix.Length > maxTreePathSegmentLength)
        {
            var pathWithoutLastSegment = TreePathUtils.RemoveLastPathSegment(source);

            var availableLastSegmentLength = maxTreePathSegmentLength - randomSuffix.Length;

            if (availableLastSegmentLength <= 0)
            {
                modifiedPath = $"{pathWithoutLastSegment}/{randomSuffix[1..maxTreePathSegmentLength]}";
            }
            else
            {
                var modifiedLastSegment = (lastSegment.Length > availableLastSegmentLength) ? lastSegment[..availableLastSegmentLength] : lastSegment;

                modifiedPath = $"{pathWithoutLastSegment}/{modifiedLastSegment}{randomSuffix}";
            }
        }

        return EnsureMaxTreePathLength(modifiedPath);
    }


    /// <summary>
    /// Ensures path with random suffix does not exceeds tree path length limit.
    /// </summary>
    /// <param name="path">Path.</param>
    private string EnsureMaxTreePathLength(string path)
    {
        return path.Length > MaxLength ? path[..MaxLength] : path;
    }
}