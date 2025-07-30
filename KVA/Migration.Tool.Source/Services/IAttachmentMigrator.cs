using CMS.MediaLibrary;
using Migration.Tool.Source.Model;

namespace Migration.Tool.Source.Services;

public interface IAttachmentMigrator
{
    Task<IMigrateAttachmentResult> TryMigrateAttachmentByPath(string documentPath, string additionalPath);
    IAsyncEnumerable<IMigrateAttachmentResult> MigrateGroupedAttachments(int documentId, Guid attachmentGroupGuid, string fieldName);
    Task<IMigrateAttachmentResult> MigrateAttachment(Guid ksAttachmentGuid, string additionalPath, int siteId);
    Task<IMigrateAttachmentResult> MigrateAttachment(ICmsAttachment ksAttachment, string? additionalMediaPath = null);
}

public interface IMigrateAttachmentResult
{
    bool Success { get; init; }
}

public record MigrateAttachmentResultMediaFile(
    bool Success,
#pragma warning disable CS0618 // Type or member is obsolete
    MediaFileInfo? MediaFileInfo = null,
    MediaLibraryInfo? MediaLibraryInfo = null
#pragma warning restore CS0618 // Type or member is obsolete
) : IMigrateAttachmentResult;

public record MigrateAttachmentResultContentItem(bool Success, Guid? ContentItemGuid) : IMigrateAttachmentResult;
