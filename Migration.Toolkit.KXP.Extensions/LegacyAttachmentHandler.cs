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
using Migration.Toolkit.KXP.Extensions;

[assembly: RegisterImplementation(typeof(AttachmentsService), typeof(AttachmentsService), Lifestyle = Lifestyle.Transient, Priority = RegistrationPriority.SystemDefault)]
[assembly: RegisterModule(typeof(LegacyAttachmentHandler))]

namespace Migration.Toolkit.KXP.Extensions
{
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
        private readonly ISiteService siteService;

        public AttachmentsService(ISiteService siteService)
        {
            this.siteService = siteService;
        }

        protected override RequestStatusEnum RequestStatusEnum => RequestStatusEnum.GetFileHandler;

        protected override CMSActionResult GetActionResultInternal()
        {
            var pathAndFileName = QueryHelper.GetString("pathandfilename", null);
            if (pathAndFileName != null)
            {
                pathAndFileName = pathAndFileName.TrimEnd('/');
                var dir = Path.GetDirectoryName(pathAndFileName);
                var mediaFiles = MediaFileInfoProvider.ProviderObject.Get()
                    .WhereStartsWith(nameof(MediaFileInfo.FilePath), dir)
                    .ToArray();

                var mediaFile = mediaFiles
                    .FirstOrDefault(mf =>
                        (string)mf.FileCustomData.GetValue(LegacyOriginalPath) == $"/{dir}" &&
                        mf.FilePath.EqualsCSafe(pathAndFileName)
                    );

                if (mediaFile != null && siteService.CurrentSite is ISiteInfo site && pathAndFileName.LastIndexOf('/') is var fileNameIndex &&
                    fileNameIndex >= 0)
                {
                    var fileName = pathAndFileName.Substring((fileNameIndex + 1), 1);
                    var mediaPath = MediaFileInfoProvider.GetMediaFilePath(mediaFile, site.SiteName, SystemContext.WebApplicationPhysicalPath);
                    var result = new CMSPhysicalFileResult(mediaPath)
                    {
                        ContentType = mediaFile.FileMimeType,
                        ContentDisposition = HTTPHelper.GetFileDisposition(fileName, Path.GetExtension(mediaPath))
                    };
                    return result;
                }
            }

            return new CMSNotFoundResult();
        }
    }
}