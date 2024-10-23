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
    bool CanContinue { get; init; }
    void Deconstruct(out bool success, out bool canContinue)
    {
        success = Success;
        canContinue = CanContinue;
    }
}

public record MigrateAttachmentResultMediaFile(
    bool Success,
    bool CanContinue,
    MediaFileInfo? MediaFileInfo = null,
    MediaLibraryInfo? MediaLibraryInfo = null
) : IMigrateAttachmentResult;

public record MigrateAttachmentResultContentItem(bool Success, bool CanContinue, Guid? ContentItemGuid) : IMigrateAttachmentResult;
