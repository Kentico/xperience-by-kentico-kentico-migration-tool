using CMS.Base;
using CMS.MediaLibrary;

using Microsoft.Extensions.Logging;

using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.KX13.Contexts;
using Migration.Tool.Core.KX13.Helpers;

namespace Migration.Tool.Core.KX13.Mappers;

public record CmsAttachmentMapperSource(
    KX13M.CmsAttachment Attachment,
    int TargetLibraryId,
    IUploadedFile File,
    string LibrarySubFolder,
    KX13M.CmsDocument? AttachmentDocument);

public class CmsAttachmentMapper : EntityMapperBase<CmsAttachmentMapperSource, MediaFileInfo>
{
    private const string LEGACY_ORIGINAL_PATH = "__LegacyOriginalPath";

    public CmsAttachmentMapper(ILogger<CmsAttachmentMapper> logger, PrimaryKeyMappingContext pkContext, IProtocol protocol) : base(logger, pkContext, protocol)
    {
    }

    protected override MediaFileInfo? CreateNewInstance(CmsAttachmentMapperSource source, MappingHelper mappingHelper, AddFailure addFailure) =>
        // library name is generated with site name in it
        new(source.File, source.TargetLibraryId, source.LibrarySubFolder, 0, 0, 0);

    protected override MediaFileInfo MapInternal(CmsAttachmentMapperSource args, MediaFileInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        (var cmsAttachment, int targetLibraryId, _, _, var attachmentDocument) = args;

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

        // target.FileCreatedByUserID = cmsAttachment.?;
        // target.FileModifiedByUserID = cmsAttachment.?;
        // target.FileCreatedWhen = cmsAttachment.?;

        target.FileModifiedWhen = cmsAttachment.AttachmentLastModified;

        KenticoHelper.CopyCustomData(target.FileCustomData, cmsAttachment.AttachmentCustomData);

        if (attachmentDocument != null)
        {
            target.FileCustomData.SetValue(LEGACY_ORIGINAL_PATH, attachmentDocument.DocumentNode.NodeAliasPath);
        }

        return target;
    }
}
