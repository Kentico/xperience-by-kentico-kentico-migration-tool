
using CMS.ContentEngine.Internal;
using CMS.Core;
using CMS.Websites.Internal;

namespace Migration.Toolkit.Common.Helpers;
public class TreePathConvertor(int webSiteChannel)
{
    private static readonly IDictionary<int, TreePathConvertor> SiteToConverter = new Dictionary<int, TreePathConvertor>();

    public static TreePathConvertor GetSiteConverter(int webSiteId)
    {
        if (SiteToConverter.TryGetValue(webSiteId, out var converter))
        {
            return converter;
        }

        converter = new TreePathConvertor(webSiteId);
        SiteToConverter.Add(webSiteId, converter);
        return converter;
    }

    public static readonly StringComparer TreePathComparer = StringComparer.InvariantCultureIgnoreCase;

    private readonly IDictionary<string, string> _nodeAliasPathToTreePath = new Dictionary<string, string>(TreePathComparer);

    public string GetConvertedOrUnchangedAssumingChannel(string nodeAliasPath) => _nodeAliasPathToTreePath.TryGetValue(nodeAliasPath, out string? converted)
            ? converted
            : nodeAliasPath;

    public record TreePathConversionResult(bool AnythingChanged, string Result);
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
                _nodeAliasPathToTreePath.Add(nodeAliasPath, nodeAliasPath);
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
                _nodeAliasPathToTreePath.Add(nodeAliasPath, uniqueTreePath);
                break;
            }
        }

        return result;
    }
}
