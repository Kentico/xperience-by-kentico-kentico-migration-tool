using HtmlAgilityPack;

namespace Migration.Toolkit.Common.Helpers;

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


    public IEnumerable<MatchMediaLinkResult> GetImages(int currentSiteId)
    {
        foreach (var imgNode in document.DocumentNode.SelectNodes("//img[@src]"))
        {
            if (imgNode?.Attributes["src"].Value is {} src)
            {
                yield return mediaLinkService.MatchMediaLink(src, currentSiteId);
            }
        }
    }

    public string ProcessHtml(int currentSiteId, Func<MatchMediaLinkResult, string, string> mediaLinkTransformer)
    {
        bool anythingChanged = false;
        foreach (var imgNode in document.DocumentNode.SelectNodes("//img[@src]") ?? Enumerable.Empty<HtmlNode>())
        {
            if (imgNode?.Attributes["src"].Value is {} src)
            {
                var matchedLink = mediaLinkService.MatchMediaLink(src, currentSiteId);

                imgNode.Attributes["src"].Value = matchedLink switch
                {
                    { Success: true, MediaKind: MediaKind.MediaFile, LinkKind: MediaLinkKind.Guid } => mediaLinkTransformer(matchedLink, src),
                    { Success: true, MediaKind: MediaKind.Attachment, LinkKind: MediaLinkKind.Guid } => mediaLinkTransformer(matchedLink, src),
                    { Success: true, MediaKind: MediaKind.MediaFile, LinkKind: MediaLinkKind.DirectMediaPath } => mediaLinkTransformer(matchedLink, src),
                    { Success: true, MediaKind: MediaKind.Attachment, LinkKind: MediaLinkKind.DirectMediaPath } => throw new InvalidOperationException($"Invalid image link encountered: {matchedLink}"),
                    _ => imgNode.Attributes["src"].Value
                };
                
                anythingChanged = true;
            }
        }

        return anythingChanged
            ? document.DocumentNode.OuterHtml
            : html;
    }
}
