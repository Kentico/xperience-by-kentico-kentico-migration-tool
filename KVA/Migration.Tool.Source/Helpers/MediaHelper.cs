using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Migration.Tool.Common.Helpers;
using Migration.Tool.Source.Model;

namespace Migration.Tool.Source.Helpers;

public static class MediaHelper
{
    public static IMediaFile? GetMediaFile(MatchMediaLinkResult matchResult, ModelFacade modelFacade, string originalLink, ILogger logger)
    {
        switch (matchResult)
        {
            case { Success: true, LinkKind: MediaLinkKind.Guid, MediaGuid: var mediaGuid, MediaKind: MediaKind.MediaFile, LinkSiteId: var linkSiteId }:
            {
                var mediaFile = modelFacade.SelectWhere<IMediaFile>("FileGUID = @mediaFileGuid AND FileSiteID = @fileSiteID",
                        new SqlParameter("mediaFileGuid", mediaGuid),
                        new SqlParameter("fileSiteID", linkSiteId)
                    )
                    .FirstOrDefault();

                return mediaFile;
            }
            case { Success: true, LinkKind: MediaLinkKind.DirectMediaPath, LibraryDir: var libraryDir, Path: var path, LinkSiteId: var linkSiteId }:
            {
                if (libraryDir is null || path is null || linkSiteId is null)
                {
                    logger.LogWarning("Cannot determine media file for link '{OriginalLink}'", originalLink);
                    return null;
                }

                string where = modelFacade.SelectVersion() is { Major: 13 }
                    ? "LibraryUseDirectPathForContent = 1 AND LibraryFolder = @libraryFolder AND LibrarySiteID = @librarySiteID"
                    : "LibraryFolder = @libraryFolder AND LibrarySiteID = @librarySiteID";
                var mediaLibraries = modelFacade.SelectWhere<IMediaLibrary>(
                    where,
                    new SqlParameter("libraryFolder", libraryDir),
                    new SqlParameter("librarySiteID", linkSiteId)
                ).ToList();
                switch (mediaLibraries)
                {
                    case [var mediaLibrary]:
                    {
                        string filePath = path.Replace($"/{mediaLibrary.LibraryFolder}/", "", StringComparison.InvariantCultureIgnoreCase);
                        return modelFacade.SelectWhere<IMediaFile>("FileLibraryID = @fileLibraryID AND FilePath = @filePath AND FileSiteID = @fileSiteID",
                                    new SqlParameter("fileLibraryID", mediaLibrary.LibraryID),
                                    new SqlParameter("filePath", filePath),
                                    new SqlParameter("fileSiteID", linkSiteId))
                                .ToList() switch
                        {
                            [var mediaFile] => mediaFile,
                            { Count: > 1 } => throw new InvalidOperationException($"Multiple media file were found for path {path}, site {linkSiteId} and library {libraryDir}"),
                            { Count: 0 } =>
                                // this may happen and is valid scenaria
                                null,
                            _ => null
                        };
                    }
                    case { Count: > 1 }:
                    {
                        break;
                    }
                    case { Count: 0 }:
                    default:
                    {
                        break;
                    }
                }

                return null;
            }
            default:
            {
                return null;
            }
        }
    }

    public static ICmsAttachment? GetAttachment(MatchMediaLinkResult matchResult, ModelFacade modelFacade) =>
        modelFacade.SelectWhere<ICmsAttachment>("AttachmentSiteID = @attachmentSiteID AND AttachmentGUID = @attachmentGUID",
                new SqlParameter("attachmentSiteID", matchResult.LinkSiteId),
                new SqlParameter("attachmentGUID", matchResult.MediaGuid)
            )
            .FirstOrDefault();
}
