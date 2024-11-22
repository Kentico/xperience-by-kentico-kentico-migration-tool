using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.RegularExpressions;
using CMS.Base;
using CMS.Helpers;
using CMS.MediaLibrary;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.KXP.Api;
using Migration.Tool.KXP.Api.Auxiliary;
using Migration.Tool.Source.Auxiliary;
using Migration.Tool.Source.Mappers;
using Migration.Tool.Source.Model;

namespace Migration.Tool.Source.Services;

public class AttachmentMigratorToMediaLibrary(
    ILogger<AttachmentMigratorToMediaLibrary> logger,
    KxpMediaFileFacade mediaFileFacade,
    IEntityMapper<CmsAttachmentMapperSource, MediaFileInfo> attachmentMapper,
    IProtocol protocol,
    ModelFacade modelFacade,
    EntityIdentityFacade entityIdentityFacade
) : IAttachmentMigrator
{
    private static readonly Regex sanitizationRegex =
        RegexHelper.GetRegex("[^-_a-z0-9]", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex libraryPathValidationRegex =
        RegexHelper.GetRegex("^[-_a-z0-9]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);


    private readonly ConcurrentDictionary<(string libraryName, int siteId), int> mediaLibraryIdCache = new();

    public async Task<IMigrateAttachmentResult> TryMigrateAttachmentByPath(string documentPath, string additionalPath)
    {
        if (string.IsNullOrWhiteSpace(documentPath))
        {
            return new MigrateAttachmentResultMediaFile(false, false);
        }

        documentPath = $"/{documentPath.Trim('/')}";

        var attachments =
                modelFacade.SelectWhere<ICmsAttachment>(
                    """
                    EXISTS (
                        SELECT T.NodeAliasPath
                        FROM CMS_Document [D] JOIN
                             CMS_Tree [T] ON D.DocumentNodeID = T.NodeID
                        WHERE D.DocumentID = AttachmentDocumentID AND T.NodeAliasPath = @nodeAliasPath
                    )
                    """, new SqlParameter("nodeAliasPath", documentPath)).ToList()
            ;

        Debug.Assert(attachments.Count == 1, "attachments.Count == 1");
        var attachment = attachments.FirstOrDefault();

        return attachment != null
            ? await MigrateAttachment(attachment, additionalPath)
            : new MigrateAttachmentResultMediaFile(false, false);
    }

    public async IAsyncEnumerable<IMigrateAttachmentResult> MigrateGroupedAttachments(int documentId, Guid attachmentGroupGuid, string fieldName)
    {
        var groupedAttachment = modelFacade.SelectWhere<ICmsAttachment>(
            """
            AttachmentGroupGuid = @attachmentGroupGuid AND
            AttachmentDocumentId = @attachmentDocumentId
            """,
            new SqlParameter("attachmentGroupGuid", attachmentGroupGuid),
            new SqlParameter("attachmentDocumentId", documentId)
        );

        foreach (var cmsAttachment in groupedAttachment)
        {
            yield return await MigrateAttachment(cmsAttachment, $"__{fieldName}");
        }
    }

    public async Task<IMigrateAttachmentResult> MigrateAttachment(Guid ksAttachmentGuid, string additionalPath, int siteId)
    {
        var attachments = modelFacade
            .SelectWhere<ICmsAttachment>("AttachmentGuid = @attachmentGuid AND AttachmentSiteID = @siteId",
                new SqlParameter("attachmentGuid", ksAttachmentGuid),
                new SqlParameter("siteId", siteId)
            )
            .ToList();

        switch (attachments)
        {
            case { Count: 0 }:
            {
                logger.LogWarning("Attachment '{AttachmentGuid}' not found! => skipping", ksAttachmentGuid);
                protocol.Append(HandbookReferences.TemporaryAttachmentMigrationIsNotSupported.WithData(new { AttachmentGuid = ksAttachmentGuid }));
                return new MigrateAttachmentResultMediaFile(false, true);
            }
            case [var attachment]:
            {
                return await MigrateAttachment(attachment, additionalPath);
            }
            default:
            {
                logger.LogWarning("Attachment '{AttachmentGuid}' found multiple times! => skipping", ksAttachmentGuid);
                protocol.Append(HandbookReferences.NonUniqueEntityGuid.WithData(new { AttachmentGuid = ksAttachmentGuid, AttachmentIds = attachments.Select(a => a.AttachmentID) }));
                return new MigrateAttachmentResultMediaFile(false, true);
            }
        }
    }

    public async Task<IMigrateAttachmentResult> MigrateAttachment(ICmsAttachment ksAttachment, string? additionalMediaPath = null)
    {
        protocol.FetchedSource(ksAttachment);

        if (ksAttachment.AttachmentFormGUID != null)
        {
            logger.LogWarning("Attachment '{AttachmentGuid}' is temporary => skipping", ksAttachment.AttachmentGUID);
            protocol.Append(HandbookReferences.TemporaryAttachmentMigrationIsNotSupported.WithData(new { ksAttachment.AttachmentID, ksAttachment.AttachmentGUID, ksAttachment.AttachmentName, ksAttachment.AttachmentSiteID }));
            return new MigrateAttachmentResultMediaFile(false, true);
        }

        var ksAttachmentDocument = ksAttachment.AttachmentDocumentID is { } attachmentDocumentId
            ? modelFacade.SelectById<ICmsDocument>(attachmentDocumentId)
            : null;

        var ksNode = ksAttachmentDocument?.DocumentNodeID is { } documentNodeId
            ? modelFacade.SelectById<ICmsTree>(documentNodeId)
            : null;

        var site = modelFacade.SelectById<ICmsSite>(ksAttachment.AttachmentSiteID) ?? throw new InvalidOperationException("Site not exists!");
        if (!TryEnsureTargetLibraryExists(ksAttachment.AttachmentSiteID, site.SiteName, out int targetMediaLibraryId))
        {
            return new MigrateAttachmentResultMediaFile(false, false);
        }

        var uploadedFile = CreateUploadFileFromAttachment(ksAttachment);
        if (uploadedFile == null)
        {
            protocol.Append(HandbookReferences
                .FailedToCreateTargetInstance<MediaFileInfo>()
                .WithIdentityPrint(ksAttachment)
                .WithMessage("Failed to create dummy upload file containing data")
            );
            return new MigrateAttachmentResultMediaFile(false, true);
        }

        (bool isFixed, var newAttachmentGuid) = entityIdentityFacade.Translate(ksAttachment);
        if (isFixed)
        {
            logger.LogWarning("Attachment {Attachment} link will be broken, new guid {Guid} was required", new { ksAttachment.AttachmentSiteID, ksAttachment.AttachmentID, ksAttachment.AttachmentGUID }, newAttachmentGuid);
        }
        var mediaFile = mediaFileFacade.GetMediaFile(newAttachmentGuid);

        protocol.FetchedTarget(mediaFile);

        string librarySubFolder = "";

        if (ksNode != null)
        {
            librarySubFolder = ksNode.NodeAliasPath;
        }

        if (!string.IsNullOrWhiteSpace(additionalMediaPath) && (ksAttachment.AttachmentIsUnsorted != true || ksAttachment.AttachmentGroupGUID != null))
        {
            librarySubFolder = Path.Combine(librarySubFolder, additionalMediaPath);
        }

        var mapped = attachmentMapper.Map(new CmsAttachmentMapperSource(ksAttachment, newAttachmentGuid, targetMediaLibraryId, uploadedFile, librarySubFolder, ksNode), mediaFile);
        protocol.MappedTarget(mapped);

        if (mapped is (var mediaFileInfo, var newInstance) { Success: true })
        {
            Debug.Assert(mediaFileInfo != null, nameof(mediaFileInfo) + " != null");

            try
            {
                if (newInstance)
                {
                    mediaFileFacade.EnsureMediaFilePathExistsInLibrary(mediaFileInfo, targetMediaLibraryId);
                }

                mediaFileFacade.SetMediaFile(mediaFileInfo, newInstance);

                protocol.Success(ksAttachmentDocument, mediaFileInfo, mapped);
                logger.LogEntitySetAction(newInstance, mediaFileInfo);

                return new MigrateAttachmentResultMediaFile(true, true, mediaFileInfo, MediaLibraryInfoProvider.ProviderObject.Get(targetMediaLibraryId));
            }
            catch (Exception exception)
            {
                logger.LogEntitySetError(exception, newInstance, mediaFileInfo);
                protocol.Append(HandbookReferences.ErrorCreatingTargetInstance<MediaFileInfo>(exception)
                    .NeedsManualAction()
                    .WithIdentityPrint(mediaFileInfo)
                    .WithData(new { mediaFileInfo.FileGUID, mediaFileInfo.FileName })
                );
            }
        }

        return new MigrateAttachmentResultMediaFile(false, true);
    }

    private IUploadedFile? CreateUploadFileFromAttachment(ICmsAttachment attachment)
    {
        if (attachment.AttachmentBinary != null)
        {
            var ms = new MemoryStream(attachment.AttachmentBinary);
            return DummyUploadedFile.FromStream(ms, attachment.AttachmentMimeType, attachment.AttachmentSize, attachment.AttachmentName);
        }

        logger.LogError("Attachment binary data is null {Attachment} " +
            "Option 1: Via admin web interface of your source instance navigate to the attachment and update the data. " +
            "Option 2: Update the database directly - table CMS_Attachment, column AttachmentBinary. " +
            "Option 3: Via admin web interface of your source instance remove all attachment references, then remove the attachment",
            new { attachment.AttachmentName, attachment.AttachmentSiteID, attachment.AttachmentID });

        return null;
    }

    private bool TryEnsureTargetLibraryExists(int targetSiteId, string targetSiteName, out int targetLibraryId)
    {
        string targetLibraryCodeName = $"AttachmentsForSite{targetSiteName}";
        string targetLibraryDisplayName = $"Attachments for site {targetSiteName}";
        try
        {
            targetLibraryId = mediaLibraryIdCache.GetOrAdd((targetLibraryCodeName, targetSiteId), static (arg, context) => MediaLibraryFactory(arg, context),
                new MediaLibraryFactoryContext(mediaFileFacade, targetLibraryCodeName, targetLibraryDisplayName));

            return true;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "creating target media library failed");
            protocol.Append(HandbookReferences.ErrorCreatingTargetInstance<MediaLibraryInfo>(exception)
                .NeedsManualAction()
                .WithData(new { TargetLibraryCodeName = targetLibraryCodeName, targetSiteId })
            );
        }

        targetLibraryId = 0;
        return false;
    }

    private static int MediaLibraryFactory((string libraryName, int siteId) arg, MediaLibraryFactoryContext context)
    {
        (string libraryName, int siteId) = arg;

        // TODO tomas.krch: 2023-11-02 libraries now globalized, where do i put conflicting directories?
        var tml = MediaLibraryInfo.Provider.Get().WhereEquals(nameof(MediaLibraryInfo.LibraryName), libraryName).SingleOrDefault();

        string libraryDirectory = context.TargetLibraryCodeName;
        if (!libraryPathValidationRegex.IsMatch(libraryDirectory))
        {
            libraryDirectory = sanitizationRegex.Replace(libraryDirectory, "_");
        }

        return tml?.LibraryID ?? context.MediaFileFacade
            .CreateMediaLibrary(siteId, libraryDirectory, "Created by Xperience Migration.Tool", context.TargetLibraryCodeName, context.TargetLibraryDisplayName).LibraryID;
    }

    private record MediaLibraryFactoryContext(KxpMediaFileFacade MediaFileFacade, string TargetLibraryCodeName, string TargetLibraryDisplayName);
}
