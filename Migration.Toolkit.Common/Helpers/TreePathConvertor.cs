using CMS.ContentEngine.Internal;
using CMS.Core;
using CMS.Websites.Internal;

namespace Migration.Toolkit.Common.Helpers;

public class TreePathConvertor(int webSiteChannel)
{
    private static readonly IDictionary<int, TreePathConvertor> siteToConverter = new Dictionary<int, TreePathConvertor>();

    public static readonly StringComparer TreePathComparer = StringComparer.InvariantCultureIgnoreCase;

    private readonly IDictionary<string, string> nodeAliasPathToTreePath = new Dictionary<string, string>(TreePathComparer);

    public static TreePathConvertor GetSiteConverter(int webSiteId)
    {
        if (siteToConverter.TryGetValue(webSiteId, out var converter))
        {
            return converter;
        }

        converter = new TreePathConvertor(webSiteId);
        siteToConverter.Add(webSiteId, converter);
        return converter;
    }

    public string GetConvertedOrUnchangedAssumingChannel(string nodeAliasPath) => nodeAliasPathToTreePath.TryGetValue(nodeAliasPath, out string? converted)
        ? converted
        : nodeAliasPath;

    public async Task<TreePathConversionResult> ConvertAndEnsureUniqueness(string nodeAliasPath)
    {
        TreePathConversionResult result;
        switch (nodeAliasPath)
        {
            case null:
            {
                result = new TreePathConversionResult(false, null);
                break;
            }
            case "/":
            case "#":
            {
                result = new TreePathConversionResult(false, nodeAliasPath);
                nodeAliasPathToTreePath.Add(nodeAliasPath, nodeAliasPath);
                break;
            }
            default:
            {
                var napSpl = nodeAliasPath.Split('/').Select(nodeAlias => TreePathUtils.NormalizePathSegment(nodeAlias));
                string normalized = string.Join("/", napSpl);
                var treePathValidator = Service.Resolve<ITreePathValidator>();
                string uniqueTreePath = await new UniqueTreePathProvider(webSiteChannel, treePathValidator).GetUniqueValue(normalized);
                bool anythingChanged = !TreePathComparer.Equals(nodeAliasPath, uniqueTreePath);
                result = new TreePathConversionResult(anythingChanged, uniqueTreePath);
                nodeAliasPathToTreePath.Add(nodeAliasPath, uniqueTreePath);
                break;
            }
        }

        return result;
    }

    public record TreePathConversionResult(bool AnythingChanged, string Result);
}
