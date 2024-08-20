namespace Migration.Toolkit.Common;

public class UrlProtocol: IDisposable, IAsyncDisposable
{
    private readonly bool migrationToAssets;
    private readonly StreamWriter streamWriter;

    public UrlProtocol(ToolkitConfiguration toolkitConfiguration)
    {
        string? dirName = Path.GetDirectoryName(toolkitConfiguration.UrlProtocol);
        string? fileName = Path.GetFileNameWithoutExtension(toolkitConfiguration.UrlProtocol);
        string? extension = Path.GetExtension(toolkitConfiguration.UrlProtocol);
        if (!Directory.Exists(dirName))
        {
            throw new InvalidOperationException($"Directory {dirName} does not exist");
        }

        streamWriter = new StreamWriter(Path.Combine(dirName, $"{fileName}_{DateTime.UtcNow:yyyyMMdd_hhmm}{extension}"));
        migrationToAssets = toolkitConfiguration.MigrateMediaToAssets;
    }

    #region Media file urls

    // https://docs.kentico.com/13/developing-websites/retrieving-content/displaying-content-from-media-libraries#getting-media-file-urls

    public record MediaFileUrlInfo(string MediaLibraryFolder, Guid MediaGuid, string FileName, int SiteId, Guid NewMediaGuid, string NewLibraryFolder);

    public void AppendMediaFileUrlIfNeeded(MediaFileUrlInfo info)
    {
        (string? mediaLibraryFolder, var mediaGuid, string fileName, int siteId, var newMediaGuid, string? newLibraryFolder) = info;

        if (migrationToAssets)
        {
            
        }
        else
        {
            // Relative path:  ~/getmedia/0140bccc-9d47-41ea-94a9-ca5d35b2964c/sample_image.jpg
            if (mediaGuid != newMediaGuid)
            {
                FormatAndWriteUriRow($"~/getmedia/{mediaGuid}/{fileName}", $"~/getmedia/{newMediaGuid}/{fileName}", "mediafile", siteId);
            }

            // Direct path: ~/MediaLibraryFolder/sample_image.jpg
            if (mediaLibraryFolder != newLibraryFolder)
            {
                FormatAndWriteUriRow($"~/{mediaLibraryFolder}/{fileName}", $"~/{newLibraryFolder}/{fileName}", "mediafile", siteId);
            }
        }
    }

    #endregion

    #region Attachment file urls

    // https://docs.kentico.com/13/developing-websites/retrieving-content/displaying-page-attachments#getting-page-attachment-urls
    
    public record AttachmentUrlInfo(Guid AttachmentGuid, string AttachmentFileName, int SiteId, Guid NewMediaGuid, string NewLibraryFolder);
    
    public void AppendAttachmentUrlIfNeeded(AttachmentUrlInfo info)
    {
        (var attachmentGuid, string? attachmentFileName, int siteId, var newMediaGuid, string? newLibraryFolder) = info;
        if (migrationToAssets)
        {
                
        }
        else
        {
            // Relative path: ~/getattachment/0140bccc-9d47-41ea-94a9-ca5d35b2964c/sample_image.jpg
            FormatAndWriteUriRow($"~/getattachment/{attachmentGuid}/{attachmentFileName}", $"~/getmedia/{newMediaGuid}/{attachmentFileName}", "attachment", siteId);
        
            // AbsoluteUrl:
        }
    }

    #endregion


    private void FormatAndWriteUriRow(string oldUrl, string newUrl, string kind, int siteId) => streamWriter.WriteLine($"{kind}|{oldUrl}|{newUrl}|{siteId}");

    public void Dispose() => streamWriter.Dispose();

    public async ValueTask DisposeAsync() => await streamWriter.DisposeAsync();
}
