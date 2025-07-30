using System.Reflection;

using CMS;
using CMS.Base;
using CMS.Base.Routing;
using CMS.Core;
using CMS.Helpers;
using CMS.MediaLibrary;
using CMS.Routing.Web;

[assembly: RegisterImplementation(typeof(AttachmentsService), typeof(AttachmentsService), Lifestyle = Lifestyle.Transient, Priority = RegistrationPriority.SystemDefault)]
[assembly: RegisterModule(typeof(LegacyAttachmentHandler))]

public class LegacyAttachmentHandler : CMS.DataEngine.Module
{
    public LegacyAttachmentHandler()
        : base("LegacyAttachmentHandler.Handler")
    {
    }

    /// <summary>
    ///     Handles the module pre-initialization.
    /// </summary>
    protected override void OnPreInit()
    {
        base.OnPreInit();
        RegisterMediaFileHandler("getimage/{fileguid:guid}/{filename}", 4);
        RegisterMediaFileHandler("getattachment/{fileguid:guid}/{filename}", 5);
        RegisterMediaFileHandler("getattachment/{*pathandfilename}", 6, typeof(AttachmentsService));
    }

    private static void RegisterMediaFileHandler(string routeTemplate, int order, Type customHandlerType = null)
    {
        var registerMethod = HttpHandlerRouteTable.Default.GetType().GetMethod("Register", BindingFlags.NonPublic | BindingFlags.Instance);
        var handlerType = customHandlerType
#pragma warning disable CS0618 // Type or member is obsolete
            ?? typeof(MediaFileInfo).Assembly.GetType("CMS.MediaLibrary.GetMediaService");
#pragma warning restore CS0618 // Type or member is obsolete

        registerMethod.Invoke(HttpHandlerRouteTable.Default, new object[] { new RegisterHttpHandlerAttribute(routeTemplate, handlerType) { Order = order } });
    }
}

public class AttachmentsService : ActionResultServiceBase
{
    private const string LEGACY_ORIGINAL_PATH = "__LegacyOriginalPath";

    protected override RequestStatusEnum RequestStatusEnum => RequestStatusEnum.GetFileHandler;

    protected override CMSActionResult GetActionResultInternal()
    {
        string pathAndFileName = QueryHelper.GetString("pathandfilename", null);
        if (pathAndFileName != null)
        {
            pathAndFileName = pathAndFileName.TrimEnd('/');
            string dir = System.IO.Path.GetDirectoryName(pathAndFileName)?.Replace("\\", "/");
            string fileName = System.IO.Path.GetFileNameWithoutExtension(pathAndFileName);

#pragma warning disable CS0618 // Type or member is obsolete
            var mediaFiles = MediaFileInfoProvider.ProviderObject.Get()
                .Columns(
                    nameof(MediaFileInfo.FileID), nameof(MediaFileInfo.FilePath), nameof(MediaFileInfo.FileCustomData),
                    nameof(MediaFileInfo.FileLibraryID), nameof(MediaFileInfo.FileName), nameof(MediaFileInfo.FileMimeType)
                )
                .WhereEquals(nameof(MediaFileInfo.FileName), fileName)
                .WhereStartsWith(nameof(MediaFileInfo.FilePath), $"{dir}")
                .ToArray();
#pragma warning restore CS0618 // Type or member is obsolete

#pragma warning disable CS0618 // Type or member is obsolete
            MediaFileInfo mediaFile;
#pragma warning restore CS0618 // Type or member is obsolete
            if (mediaFiles.Length > 1)
            {
                var narrowedByOriginalPath = mediaFiles
                    .Where(mf => mf.FileCustomData.GetValue(LEGACY_ORIGINAL_PATH)?.ToString()
                        ?.Equals($"/{dir}", StringComparison.InvariantCultureIgnoreCase) == true)
                    .ToArray();

                mediaFile = narrowedByOriginalPath.Length > 1
                    ? null
                    : narrowedByOriginalPath.FirstOrDefault();
            }
            else
            {
                mediaFile = mediaFiles.FirstOrDefault();
            }

#pragma warning disable CS0618 // Type or member is obsolete
            string mediaPath = MediaFileInfoProvider.GetMediaFilePath(mediaFile.FilePath, mediaFile.FileLibraryID, SystemContext.WebApplicationPhysicalPath);
#pragma warning restore CS0618 // Type or member is obsolete
            var result = new CMSPhysicalFileResult(mediaPath) { ContentType = mediaFile.FileMimeType, ContentDisposition = HTTPHelper.GetFileDisposition(mediaPath, System.IO.Path.GetExtension(mediaPath)) };
            return result;
        }

        return new CMSNotFoundResult();
    }
}
