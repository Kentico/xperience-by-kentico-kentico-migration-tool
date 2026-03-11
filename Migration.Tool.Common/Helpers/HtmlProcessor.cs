using HtmlAgilityPack;

namespace Migration.Tool.Common.Helpers;

public class HtmlProcessor
{
    private readonly MediaLinkService mediaLinkService;
    private readonly HtmlDocument document;
    private readonly string html;

    public HtmlProcessor(string html, MediaLinkService mediaLinkService)
    {
        this.mediaLinkService = mediaLinkService;

        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        this.html = html;
        document = doc;
    }

    public async Task<string> ProcessHtml(int currentSiteId, Func<MatchMediaLinkResult, string, Task<string>> mediaLinkTransformer)
    {
        bool anythingChanged = false;

        anythingChanged |= await ProcessNodes("//img[@src]", "src", "image", currentSiteId, mediaLinkTransformer);
        anythingChanged |= await ProcessNodes("//a[@href]", "href", "hyperlink", currentSiteId, mediaLinkTransformer);

        return anythingChanged
            ? document.DocumentNode.OuterHtml
            : html;
    }

    private async Task<bool> ProcessNodes(
        string xpath,
        string attributeName,
        string linkType,
        int currentSiteId,
        Func<MatchMediaLinkResult, string, Task<string>> mediaLinkTransformer)
    {
        bool changed = false;

        foreach (var node in document.DocumentNode.SelectNodes(xpath) ?? Enumerable.Empty<HtmlNode>())
        {
            if (node?.Attributes[attributeName].Value is { } src)
            {
                var matchedLink = mediaLinkService.MatchMediaLink(src, currentSiteId);
                var newValue = await TransformMediaLink(matchedLink, src, linkType, mediaLinkTransformer);

                if (newValue != src)
                {
                    node.Attributes[attributeName].Value = newValue;
                    changed = true;
                }
            }
        }

        return changed;
    }

    private static async Task<string> TransformMediaLink(
        MatchMediaLinkResult matchedLink,
        string originalValue,
        string linkType,
        Func<MatchMediaLinkResult, string, Task<string>> mediaLinkTransformer) =>
        matchedLink switch
        {
            { Success: true, MediaKind: MediaKind.MediaFile, LinkKind: MediaLinkKind.Guid } => await mediaLinkTransformer(matchedLink, originalValue),
            { Success: true, MediaKind: MediaKind.Attachment, LinkKind: MediaLinkKind.Guid } => await mediaLinkTransformer(matchedLink, originalValue),
            { Success: true, MediaKind: MediaKind.MediaFile, LinkKind: MediaLinkKind.DirectMediaPath } => await mediaLinkTransformer(matchedLink, originalValue),
            { Success: true, MediaKind: MediaKind.Attachment, LinkKind: MediaLinkKind.DirectMediaPath } => throw new InvalidOperationException($"Invalid {linkType} link encountered: {matchedLink}"),
            _ => originalValue
        };
}
