namespace Migration.Toolkit.Common.Helpers;

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

public class MediaHelper
{
    public static MatchMediaLinkResult MatchMediaLink(string? linkStr)
    {
        if (linkStr == null)
        {
            return MatchMediaLinkResult.None;
        }

        string link = linkStr.TrimStart(new[] { '~' });


        Guid? mediaId = null;
        var mediaLinkKind = MediaLinkKind.None;
        var mediaKind = MediaKind.None;
        var mediaPathResult = new List<string>();
        bool copyPath = false;
        bool inspectNext = false;

        string path = "";
        if (Uri.IsWellFormedUriString(link, UriKind.Absolute))
        {
            path = new Uri(link, UriKind.Absolute).LocalPath;
        }

        if (Uri.IsWellFormedUriString(link, UriKind.Relative))
        {
            try
            {
                path = new Uri(new Uri("http://mock.local/", UriKind.Absolute), new Uri(link, UriKind.Relative)).LocalPath;
            }
            catch
            {
                path = link;
            }
        }

        string[] spl = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

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

        return new MatchMediaLinkResult(true, mediaLinkKind, mediaKind, copyPath ? $"/{string.Join("/", mediaPathResult)}" : null, mediaId);
    }

    public record MatchMediaLinkResult(bool Success, MediaLinkKind LinkKind, MediaKind MediaKind, string? Path, Guid? MediaGuid)
    {
        public static readonly MatchMediaLinkResult None = new(false, MediaLinkKind.None, MediaKind.None, null, null);
    }
}
