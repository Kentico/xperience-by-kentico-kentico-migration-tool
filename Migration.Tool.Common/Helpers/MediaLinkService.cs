using System.Collections.Immutable;

namespace Migration.Tool.Common.Helpers;

public enum MediaKind
{
    None,
    Attachment,
    MediaFile
}

public enum MediaLinkKind
{
    None,
    Path,
    DirectMediaPath,
    Guid
}

public class MediaLinkService(
    ImmutableList<(int siteId, string siteName, string siteLiveSiteUrl)> sites,
    ImmutableList<(int? siteId, string? value)> cmsMediaLibrariesFolder,
    ImmutableList<(int? siteId, string? value)> cmsUseMediaLibrariesSiteFolder,
    Dictionary<int, HashSet<string>> siteLibraryNames
    )
{
    public MatchMediaLinkResult MatchMediaLink(string? linkStr, int currentSiteId)
    {
        if (string.IsNullOrEmpty(linkStr))
        {
            return MatchMediaLinkResult.None;
        }

        string link = linkStr.TrimStart(['~']);

        Guid? mediaId = null;
        var mediaLinkKind = MediaLinkKind.None;
        var mediaKind = MediaKind.None;
        var mediaPathResult = new List<string>();
        bool copyPath = false;
        bool inspectNext = false;

        string path = "";
        bool isAbsolute = false;
        Uri? uri = null;
        if (Uri.IsWellFormedUriString(link, UriKind.Absolute))
        {
            uri = new Uri(link, UriKind.Absolute);
            path = uri.LocalPath;
            isAbsolute = true;
        }

        var mockDomain = new Uri("http://mock.local/", UriKind.Absolute);
        if (Uri.IsWellFormedUriString(link, UriKind.Relative))
        {
            try
            {
                uri = new Uri(mockDomain, new Uri(link, UriKind.Relative));
                path = uri.LocalPath;
                isAbsolute = false;
            }
            catch
            {
                path = link;
            }
        }

        int inspectionIndex = 0;

        // assuming that link is from current site
        int? linkSiteId = currentSiteId;
        if (uri != null)
        {
            // in this case link may be from foreign domain, but still in same instance on different site
            // https://localhost:42157/Kentico13_2024_DG_CustomMedia/media/CustomDirLibWithDirContent/unnamed.jpg
            if (isAbsolute)
            {
                foreach ((int siteId, string? _, string? siteLiveSiteUrl) in sites)
                {
                    var siteUri = new Uri(siteLiveSiteUrl);
                    if (siteUri.IsBaseOf(uri))
                    {
                        linkSiteId = siteId;
                        break;
                    }
                }
            }
        }

        string[] spl = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var site = sites.FirstOrDefault(s => s.siteId == linkSiteId);

        // match site sub path
        if (site != default && uri is not null)
        {
            if (Uri.IsWellFormedUriString(site.siteLiveSiteUrl, UriKind.Absolute))
            {
                var siteLiveSiteUri = new Uri(site.siteLiveSiteUrl, UriKind.Absolute);
                int subPathLength = siteLiveSiteUri.LocalPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Length;
                if (uri.LocalPath.StartsWith(siteLiveSiteUri.LocalPath, StringComparison.InvariantCultureIgnoreCase))
                {
                    inspectionIndex += subPathLength;
                }
            }
        }

        // custom lib dir + custom global media dir + subdir:   /CustomMediaFolder/CDWCLN2/Subdir1/unnamed.jpg 
        // custom lib dir + custom global media dir:            /CustomMediaFolder/CDWCLN2/84693449_B.png
        // standard site dir:                                   /MTExtensibilityTests/media/MediaWithDirectPath/84693449_B.png

        string? globalMediaLibraryFolder = cmsMediaLibrariesFolder.OrderBy(x => x.siteId ?? -1).FirstOrDefault(x => x.siteId == linkSiteId || x.siteId == null).value;
        if (!string.IsNullOrWhiteSpace(globalMediaLibraryFolder))
        {
            if (globalMediaLibraryFolder.Equals(spl[inspectionIndex], StringComparison.InvariantCultureIgnoreCase))
            {
                // it is inside global media folder
                mediaKind = MediaKind.MediaFile;
                mediaLinkKind = MediaLinkKind.DirectMediaPath;
                inspectionIndex++;
            }
        }

        bool siteMediaFolder = "True".Equals(cmsUseMediaLibrariesSiteFolder.OrderBy(x => x.siteId ?? -1).FirstOrDefault(x => x.siteId == linkSiteId || x.siteId == null).value, StringComparison.InvariantCultureIgnoreCase);
        if (siteMediaFolder)
        {
            if (sites.FirstOrDefault(x => x.siteId == linkSiteId) is var (_, siteName, _))
            {
                if (siteName.Equals(spl[inspectionIndex], StringComparison.InvariantCultureIgnoreCase))
                {
                    // it is direct path media
                    mediaKind = MediaKind.MediaFile;
                    mediaLinkKind = MediaLinkKind.DirectMediaPath;
                    inspectionIndex++;
                }
            }
        }

        if ("media".Equals(spl[inspectionIndex], StringComparison.InvariantCultureIgnoreCase))
        {
            mediaKind = MediaKind.MediaFile;
            mediaLinkKind = MediaLinkKind.DirectMediaPath;
            inspectionIndex++;
        }

        if (mediaLinkKind is MediaLinkKind.DirectMediaPath)
        {
            // try match libreary name
            if (linkSiteId is { } lsid && siteLibraryNames.TryGetValue(lsid, out var libraryNames) && libraryNames.Contains(spl[inspectionIndex]))
            {
                return new MatchMediaLinkResult(true, mediaLinkKind, mediaKind, $"/{string.Join("/", spl[inspectionIndex..])}", null, linkSiteId, spl[inspectionIndex]);
            }
            else
            {
                return new MatchMediaLinkResult(true, mediaLinkKind, mediaKind, $"/{string.Join("/", spl[inspectionIndex..])}", null, linkSiteId, null);
            }
        }

        for (int i = 0; i < spl.Length; i++)
        {
            string cs = spl[i];
            if (cs.Equals("getattachment", StringComparison.InvariantCultureIgnoreCase))
            {
                mediaKind = MediaKind.Attachment;
                inspectNext = true;
            }

            if (cs.Equals("getimage", StringComparison.InvariantCultureIgnoreCase))
            {
                mediaKind = MediaKind.MediaFile;
                inspectNext = true;
            }
            else if (cs.Equals("getmedia", StringComparison.InvariantCultureIgnoreCase))
            {
                mediaKind = MediaKind.MediaFile;
                inspectNext = true;
            }
            else if (copyPath)
            {
                mediaPathResult.Add(cs);
                mediaLinkKind = MediaLinkKind.Path;
            }

            if (inspectNext)
            {
                inspectNext = false;
                // now lets look forward
                int nsi = i + 1;
                if (nsi < spl.Length)
                {
                    string nextSegment = spl[nsi];
                    if (Guid.TryParse(nextSegment, out var mid))
                    {
                        mediaId = mid;
                        mediaLinkKind = MediaLinkKind.Guid;
                    }
                    else
                    {
                        copyPath = true;
                    }
                }
            }
        }

        if (mediaLinkKind == MediaLinkKind.None || mediaKind == MediaKind.None)
        {
            return MatchMediaLinkResult.None;
        }

        return new MatchMediaLinkResult(true, mediaLinkKind, mediaKind, copyPath ? $"/{string.Join("/", mediaPathResult)}" : null, mediaId, linkSiteId, null);
    }
}

public record MatchMediaLinkResult(bool Success, MediaLinkKind LinkKind, MediaKind MediaKind, string? Path, Guid? MediaGuid, int? LinkSiteId, string? LibraryDir)
{
    public static readonly MatchMediaLinkResult None = new(false, MediaLinkKind.None, MediaKind.None, null, null, null, null);
}
