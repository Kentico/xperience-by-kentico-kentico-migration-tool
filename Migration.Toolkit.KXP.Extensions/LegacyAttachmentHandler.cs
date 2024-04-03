using System;
using System.IO;
using System.Linq;
using System.Reflection;
using CMS;
using CMS.Core;
using CMS.Base;
using CMS.Base.Routing;
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
    /// Handles the module pre-initialization.
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
        var handlerType = customHandlerType ?? typeof(MediaFileInfo).Assembly.GetType("CMS.MediaLibrary.GetMediaService");

        registerMethod.Invoke(HttpHandlerRouteTable.Default, new object[]
        {
            new RegisterHttpHandlerAttribute(routeTemplate, handlerType)
            {
                Order = order
            }
        });
    }
}

public class AttachmentsService : ActionResultServiceBase
{
    private const string LegacyOriginalPath = "__LegacyOriginalPath";

    protected override RequestStatusEnum RequestStatusEnum => RequestStatusEnum.GetFileHandler;

    protected override CMSActionResult GetActionResultInternal()
    {
        var pathAndFileName = QueryHelper.GetString("pathandfilename", null);
        if (pathAndFileName != null)
        {
            pathAndFileName = pathAndFileName.TrimEnd('/');
            var dir = System.IO.Path.GetDirectoryName(pathAndFileName)?.Replace("\\", "/");
            var fileName = System.IO.Path.GetFileNameWithoutExtension(pathAndFileName);

            var mediaFiles = MediaFileInfoProvider.ProviderObject.Get()
                .Columns(
                    nameof(MediaFileInfo.FileID), nameof(MediaFileInfo.FilePath), nameof(MediaFileInfo.FileCustomData),
                    nameof(MediaFileInfo.FileLibraryID), nameof(MediaFileInfo.FileName), nameof(MediaFileInfo.FileMimeType)
                )
                .WhereEquals(nameof(MediaFileInfo.FileName), fileName)
                .WhereStartsWith(nameof(MediaFileInfo.FilePath), $"{dir}")
                .ToArray();

            MediaFileInfo mediaFile;
            if (mediaFiles.Length > 1)
            {
                var narrowedByOriginalPath = mediaFiles
                    .Where(mf => mf.FileCustomData.GetValue(LegacyOriginalPath)?.ToString()
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

            var mediaPath = MediaFileInfoProvider.GetMediaFilePath(mediaFile.FilePath, mediaFile.FileLibraryID, SystemContext.WebApplicationPhysicalPath);
            var result = new CMSPhysicalFileResult(mediaPath)
            {
                ContentType = mediaFile.FileMimeType,
                ContentDisposition = HTTPHelper.GetFileDisposition(mediaPath, System.IO.Path.GetExtension(mediaPath))
            };
            return result;
        }

        return new CMSNotFoundResult();
    }
}