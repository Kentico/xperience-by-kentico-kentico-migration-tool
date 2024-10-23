using System.Diagnostics;
using CMS.ContentEngine.Internal;
using CMS.Core;
using Kentico.Xperience.UMT.Services;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Source.Auxiliary;
using Migration.Tool.Source.Model;

namespace Migration.Tool.Source.Services;

public class AttachmentMigratorToContentItem(
    ILogger<AttachmentMigratorToContentItem> logger,
    IProtocol protocol,
    ModelFacade modelFacade,
    EntityIdentityFacade entityIdentityFacade,
    IAssetFacade assetFacade,
    IImporter importer
) : IAttachmentMigrator
{
    public async Task<IMigrateAttachmentResult> TryMigrateAttachmentByPath(string documentPath, string additionalPath)
    {
        if (string.IsNullOrWhiteSpace(documentPath))
        {
            return new MigrateAttachmentResultContentItem(false, false, null);
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
            : new MigrateAttachmentResultContentItem(false, false, null);
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
                return new MigrateAttachmentResultContentItem(false, true, null);
            }
            case [var attachment]:
            {
                return await MigrateAttachment(attachment, additionalPath);
            }
            default:
            {
                logger.LogWarning("Attachment '{AttachmentGuid}' found multiple times! => skipping", ksAttachmentGuid);
                protocol.Append(HandbookReferences.NonUniqueEntityGuid.WithData(new { AttachmentGuid = ksAttachmentGuid, AttachmentIds = attachments.Select(a => a.AttachmentID) }));
                return new MigrateAttachmentResultContentItem(false, true, null);
            }
        }
    }

    private bool assetFacadeInitialized;
    public async Task<IMigrateAttachmentResult> MigrateAttachment(ICmsAttachment ksAttachment, string? additionalMediaPath = null)
    {
        if (!assetFacadeInitialized)
        {
            await assetFacade.PreparePrerequisites();
            assetFacadeInitialized = true;
        }

        protocol.FetchedSource(ksAttachment);

        if (ksAttachment.AttachmentFormGUID != null)
        {
            logger.LogWarning("Attachment '{AttachmentGuid}' is temporary => skipping", ksAttachment.AttachmentGUID);
            protocol.Append(HandbookReferences.TemporaryAttachmentMigrationIsNotSupported.WithData(new { ksAttachment.AttachmentID, ksAttachment.AttachmentGUID, ksAttachment.AttachmentName, ksAttachment.AttachmentSiteID }));
            return new MigrateAttachmentResultContentItem(false, true, null);
        }

        var ksAttachmentDocument = ksAttachment.AttachmentDocumentID is { } attachmentDocumentId
            ? modelFacade.SelectById<ICmsDocument>(attachmentDocumentId)
            : null;

        var ksNode = ksAttachmentDocument?.DocumentNodeID is { } documentNodeId
            ? modelFacade.SelectById<ICmsTree>(documentNodeId)
            : null;

        var site = modelFacade.SelectById<ICmsSite>(ksAttachment.AttachmentSiteID) ?? throw new InvalidOperationException("Site not exists!");

        (bool isFixed, var newAttachmentGuid) = entityIdentityFacade.Translate(ksAttachment);
        if (isFixed)
        {
            logger.LogWarning("Attachment {Attachment} link will be broken, new guid {Guid} was required", new { ksAttachment.AttachmentSiteID, ksAttachment.AttachmentID, ksAttachment.AttachmentGUID }, newAttachmentGuid);
        }

        if (ksAttachment.AttachmentBinary is null)
        {
            logger.LogError("Binary data is null, cannot migrate attachment: {Attachment}", ksAttachment);
            throw new InvalidOperationException("Attachment data is null!");
        }

        var contentLanguageRetriever = Service.Resolve<IContentLanguageRetriever>();
        var defaultContentLanguage = await contentLanguageRetriever.GetDefaultContentLanguage();

        var asset = await assetFacade.FromAttachment(ksAttachment, site, ksNode, [ksAttachmentDocument?.DocumentCulture ?? defaultContentLanguage.ContentLanguageName]);
        switch (await importer.ImportAsync(asset))
        {
            case { Success: true }:
            {
                logger.LogInformation("Media file '{File}' imported", ksAttachment.AttachmentGUID);
                return new MigrateAttachmentResultContentItem(true, true, asset.ContentItemGUID);
            }
            case { Success: false, Exception: { } exception }:
            {
                logger.LogError("Media file '{File}' not migrated: {Error}", ksAttachment.AttachmentGUID, exception);
                return new MigrateAttachmentResultContentItem(false, true, null);
            }
            case { Success: false, ModelValidationResults: { } validation }:
            {
                foreach (var validationResult in validation)
                {
                    logger.LogError("Media file '{File}' not migrated: {Members}: {Error}", ksAttachment.AttachmentGUID, string.Join(",", validationResult.MemberNames), validationResult.ErrorMessage);
                }

                return new MigrateAttachmentResultContentItem(false, true, null);
            }
            default:
                return new MigrateAttachmentResultContentItem(false, false, null);
        }
    }
}
