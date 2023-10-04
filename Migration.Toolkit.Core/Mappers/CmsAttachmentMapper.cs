using CMS.Base;
using CMS.MediaLibrary;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.Helpers;
using Migration.Toolkit.Core.MigrationProtocol;

namespace Migration.Toolkit.Core.Mappers;

using Migration.Toolkit.KX13.Models;

public record CmsAttachmentMapperSource(CmsAttachment Attachment, int TargetLibraryId, IUploadedFile File, string LibrarySubFolder,
    CmsDocument? AttachmentDocument);

public class CmsAttachmentMapper: EntityMapperBase<CmsAttachmentMapperSource, MediaFileInfo>
{
    private const string LegacyOriginalPath = "__LegacyOriginalPath";

    public CmsAttachmentMapper(ILogger<CmsAttachmentMapper> logger, PrimaryKeyMappingContext pkContext, IProtocol protocol) : base(logger, pkContext, protocol)
    {
    }

    protected override MediaFileInfo? CreateNewInstance(CmsAttachmentMapperSource source, MappingHelper mappingHelper, AddFailure addFailure)
    {
        // TODOV27 tomas.krch: 2023-09-05: remove site mapping (or replace with mapping to channel)
        if (mappingHelper.TranslateRequiredId<KX13M.CmsSite>(s => s.SiteId, source.Attachment.AttachmentSiteId, out var siteId))
        {
            // TODOV27 tomas.krch: 2023-09-05: MediaFileInfo - site id removed from .ctor
            return new MediaFileInfo(source.File, source.TargetLibraryId, source.LibrarySubFolder, 0, 0, 0);
        }

        var error = HandbookReferences
            .FailedToCreateTargetInstance<MediaFileInfo>()
            .WithData(source);

        addFailure(new MapperResultFailure<MediaFileInfo>(error));
        return null;
    }

    protected override MediaFileInfo MapInternal(CmsAttachmentMapperSource args, MediaFileInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        var (cmsAttachment, targetLibraryId, _, _, attachmentDocument) = args;

        target.FileName = Path.GetFileNameWithoutExtension(cmsAttachment.AttachmentName);
        target.FileTitle = cmsAttachment.AttachmentTitle ?? cmsAttachment.AttachmentName;
        target.FileDescription = cmsAttachment.AttachmentDescription ?? string.Empty;
        target.FileExtension = cmsAttachment.AttachmentExtension;
        target.FileMimeType = cmsAttachment.AttachmentMimeType;
        target.FileSize = cmsAttachment.AttachmentSize;
        target.FileImageWidth = cmsAttachment.AttachmentImageWidth ?? 0;
        target.FileImageHeight = cmsAttachment.AttachmentImageHeight ?? 0;
        target.FileGUID = cmsAttachment.AttachmentGuid;
        target.FileLibraryID = targetLibraryId;

        // TODOV27 tomas.krch: 2023-09-05: remove site id mapping (or map to channel)
        // if (mappingHelper.TranslateRequiredId<KX13.Models.CmsSite>(s => s.SiteId, cmsAttachment.AttachmentSiteId, out var siteId))
        // {
        //     target.FileSiteID = siteId;
        // }

        // target.FileCreatedByUserID = cmsAttachment.?;
        // target.FileModifiedByUserID = cmsAttachment.?;
        // target.FileCreatedWhen = cmsAttachment.?;

        target.FileModifiedWhen = cmsAttachment.AttachmentLastModified;

        KenticoHelper.CopyCustomData(target.FileCustomData, cmsAttachment.AttachmentCustomData);

        if (attachmentDocument != null)
        {
            target.FileCustomData.SetValue(LegacyOriginalPath, attachmentDocument.DocumentNode.NodeAliasPath);
        }

        return target;
    }
}