
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.RegularExpressions;

using CMS.Base;
using CMS.Helpers;
using CMS.MediaLibrary;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.KXP.Api;
using Migration.Toolkit.KXP.Api.Auxiliary;
using Migration.Toolkit.KXP.Context;
using Migration.Toolkit.Source.Mappers;
using Migration.Toolkit.Source.Model;

namespace Migration.Toolkit.Source.Services;
public class AttachmentMigrator(
    ILogger<AttachmentMigrator> logger,
    KxpMediaFileFacade mediaFileFacade,
    IDbContextFactory<KxpContext> kxpContextFactory,
    IEntityMapper<CmsAttachmentMapperSource, MediaFileInfo> attachmentMapper,
    IProtocol protocol,
    ModelFacade modelFacade
)
{
    public record MigrateAttachmentResult(
        bool Success,
        bool CanContinue,
        MediaFileInfo? MediaFileInfo = null,
        MediaLibraryInfo? MediaLibraryInfo = null);

    public MigrateAttachmentResult TryMigrateAttachmentByPath(string documentPath, string additionalPath)
    {
        if (string.IsNullOrWhiteSpace(documentPath))
        {
            return new MigrateAttachmentResult(false, false);
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
            ? MigrateAttachment(attachment, additionalPath)
            : new MigrateAttachmentResult(false, false);
    }

    public IEnumerable<MigrateAttachmentResult> MigrateGroupedAttachments(int documentId, Guid attachmentGroupGuid, string fieldName)
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
            yield return MigrateAttachment(cmsAttachment, $"__{fieldName}");
        }
    }

    public MigrateAttachmentResult MigrateAttachment(Guid ksAttachmentGuid, string additionalPath)
    {
        var attachment = modelFacade
            .SelectWhere<ICmsAttachment>("AttachmentGuid = @attachmentGuid", new SqlParameter("attachmentGuid", ksAttachmentGuid))
            .SingleOrDefault();

        if (attachment == null)
        {
            logger.LogWarning("Attachment '{AttachmentGuid}' not found! => skipping", ksAttachmentGuid);
            protocol.Append(HandbookReferences.TemporaryAttachmentMigrationIsNotSupported.WithData(new { AttachmentGuid = ksAttachmentGuid, }));
            return new MigrateAttachmentResult(false, true);
        }

        return MigrateAttachment(attachment, additionalPath);
    }

    public MigrateAttachmentResult MigrateAttachment(ICmsAttachment ksAttachment, string? additionalMediaPath = null)
    {
        // TODO tomas.krch: 2022-08-18 directory validation only -_ replace!
        protocol.FetchedSource(ksAttachment);

        if (ksAttachment.AttachmentFormGUID != null)
        {
            logger.LogWarning("Attachment '{AttachmentGuid}' is temporary => skipping", ksAttachment.AttachmentGUID);
            protocol.Append(HandbookReferences.TemporaryAttachmentMigrationIsNotSupported.WithData(new
            {
                ksAttachment.AttachmentID,
                ksAttachment.AttachmentGUID,
                ksAttachment.AttachmentName,
                ksAttachment.AttachmentSiteID
            }));
            return new(false, true);
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
            return new(false, false);
        }

        var uploadedFile = CreateUploadFileFromAttachment(ksAttachment);
        if (uploadedFile == null)
        {
            protocol.Append(HandbookReferences
                .FailedToCreateTargetInstance<MediaFileInfo>()
                .WithIdentityPrint(ksAttachment)
                .WithMessage("Failed to create dummy upload file containing data")
            );
            return new(false, true);
        }

        var mediaFile = mediaFileFacade.GetMediaFile(ksAttachment.AttachmentGUID);

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

        var mapped = attachmentMapper.Map(new CmsAttachmentMapperSource(ksAttachment, targetMediaLibraryId, uploadedFile, librarySubFolder, ksNode), mediaFile);
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

                return new(true, true, mediaFileInfo, MediaLibraryInfoProvider.ProviderObject.Get(targetMediaLibraryId));
            }
            catch (Exception exception)
            {
                logger.LogEntitySetError(exception, newInstance, mediaFileInfo);
                protocol.Append(HandbookReferences.ErrorCreatingTargetInstance<MediaFileInfo>(exception)
                    .NeedsManualAction()
                    .WithIdentityPrint(mediaFileInfo)
                    .WithData(new
                    {
                        mediaFileInfo.FileGUID,
                        mediaFileInfo.FileName
                    })
                );
            }
        }

        return new(false, true);
    }

    private IUploadedFile? CreateUploadFileFromAttachment(ICmsAttachment attachment)
    {
        if (attachment.AttachmentBinary != null)
        {
            var ms = new MemoryStream(attachment.AttachmentBinary);
            return DummyUploadedFile.FromStream(ms, attachment.AttachmentMimeType, attachment.AttachmentSize, attachment.AttachmentName);
        }
        else
        {
            return null;
        }
    }


    private readonly ConcurrentDictionary<(string libraryName, int siteId), int> _mediaLibraryIdCache = new();

    private bool TryEnsureTargetLibraryExists(int targetSiteId, string targetSiteName, out int targetLibraryId)
    {
        string targetLibraryCodeName = $"AttachmentsForSite{targetSiteName}";
        string targetLibraryDisplayName = $"Attachments for site {targetSiteName}";
        using var dbContext = kxpContextFactory.CreateDbContext();
        try
        {
            targetLibraryId = _mediaLibraryIdCache.GetOrAdd((targetLibraryCodeName, targetSiteId), static (arg, context) => MediaLibraryFactory(arg, context),
                new MediaLibraryFactoryContext(mediaFileFacade, targetLibraryCodeName, targetLibraryDisplayName, dbContext));

            return true;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "creating target media library failed");
            protocol.Append(HandbookReferences.ErrorCreatingTargetInstance<MediaLibraryInfo>(exception)
                .NeedsManualAction()
                .WithData(new { TargetLibraryCodeName = targetLibraryCodeName, targetSiteId, })
            );
        }

        targetLibraryId = 0;
        return false;
    }

    private record MediaLibraryFactoryContext(KxpMediaFileFacade MediaFileFacade, string TargetLibraryCodeName, string TargetLibraryDisplayName, KxpContext DbContext);

    private static readonly Regex SanitizationRegex =
        RegexHelper.GetRegex("[^-_a-z0-9]", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex LibraryPathValidationRegex =
        RegexHelper.GetRegex("^[-_a-z0-9]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);

    private static int MediaLibraryFactory((string libraryName, int siteId) arg, MediaLibraryFactoryContext context)
    {
        var (libraryName, siteId) = arg;

        // TODO tomas.krch: 2023-11-02 libraries now globalized, where do i put conflicting directories?
        var tml = context.DbContext.MediaLibraries.SingleOrDefault(ml => ml.LibraryName == libraryName);

        string libraryDirectory = context.TargetLibraryCodeName;
        if (!LibraryPathValidationRegex.IsMatch(libraryDirectory))
        {
            libraryDirectory = SanitizationRegex.Replace(libraryDirectory, "_");
        }

        return tml?.LibraryId ?? context.MediaFileFacade
            .CreateMediaLibrary(siteId, libraryDirectory, "Created by Xperience Migration.Toolkit", context.TargetLibraryCodeName, context.TargetLibraryDisplayName).LibraryID;
    }
}
