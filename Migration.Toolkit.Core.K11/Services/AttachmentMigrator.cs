// namespace Migration.Toolkit.Core.K11.Services;
//
// using System.Collections.Concurrent;
// using System.Diagnostics;
// using System.Text.RegularExpressions;
// using CMS.Base;
// using CMS.Helpers;
// using CMS.MediaLibrary;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Logging;
// using Migration.Toolkit.Common;
// using Migration.Toolkit.Common.Abstractions;
// using Migration.Toolkit.Common.MigrationProtocol;
// using Migration.Toolkit.Core.K11.Mappers;
// using Migration.Toolkit.K11;
// using Migration.Toolkit.K11.Models;
// using Migration.Toolkit.KXP.Api;
// using Migration.Toolkit.KXP.Api.Auxiliary;
// using Migration.Toolkit.KXP.Context;
//
// public class AttachmentMigrator(ILogger<AttachmentMigrator> logger,
//     IDbContextFactory<K11Context> k11ContextFactory,
//     KxpMediaFileFacade mediaFileFacade,
//     IDbContextFactory<KxpContext> kxpContextFactory,
//     IEntityMapper<CmsAttachmentMapperSource, MediaFileInfo> attachmentMapper,
//     IProtocol protocol)
// {
//     public record MigrateAttachmentResult(bool Success, bool CanContinue, MediaFileInfo? MediaFileInfo = null,
//         MediaLibraryInfo? MediaLibraryInfo = null);
//
//     public MigrateAttachmentResult TryMigrateAttachmentByPath(string documentPath, string additionalPath)
//     {
//         if(string.IsNullOrWhiteSpace(documentPath)) return new MigrateAttachmentResult(false, false);
//         documentPath = $"/{documentPath.Trim('/')}";
//
//         using var k11Context = k11ContextFactory.CreateDbContext();
//         var attachment =
//             k11Context.CmsAttachments
//                 .Include(a => a.AttachmentDocument)
//                 .ThenInclude(d => d.DocumentNode)
//                 .FirstOrDefault(a => a.AttachmentDocument.DocumentNode.NodeAliasPath == documentPath);
//
//         return attachment != null
//             ? MigrateAttachment(attachment, additionalPath)
//             : new MigrateAttachmentResult(false, false);
//     }
//
//     public IEnumerable<MigrateAttachmentResult> MigrateGroupedAttachments(int documentId, Guid attachmentGroupGuid, string fieldName)
//     {
//         using var k11Context = k11ContextFactory.CreateDbContext();
//         var groupedAttachment =
//             k11Context.CmsAttachments.Where(a => a.AttachmentGroupGuid == attachmentGroupGuid && a.AttachmentDocumentId == documentId);
//         foreach (var cmsAttachment in groupedAttachment)
//         {
//             yield return MigrateAttachment(cmsAttachment, $"__{fieldName}");
//         }
//     }
//
//     public MigrateAttachmentResult MigrateAttachment(Guid k11CmsAttachmentGuid, string additionalPath)
//     {
//         using var k11Context = k11ContextFactory.CreateDbContext();
//         var attachment = k11Context.CmsAttachments.SingleOrDefault(a => a.AttachmentGuid == k11CmsAttachmentGuid);
//         if (attachment == null)
//         {
//             logger.LogWarning("Attachment '{AttachmentGuid}' not found! => skipping", k11CmsAttachmentGuid);
//             protocol.Append(HandbookReferences.TemporaryAttachmentMigrationIsNotSupported.WithData(new
//             {
//                 AttachmentGuid = k11CmsAttachmentGuid,
//             }));
//             return new MigrateAttachmentResult(false, true);
//         }
//
//         return MigrateAttachment(attachment, additionalPath);
//     }
//
//     public MigrateAttachmentResult MigrateAttachment(CmsAttachment k11CmsAttachment, string? additionalMediaPath = null)
//     {
//         // TODO tomas.krch: 2022-08-18 directory validation only -_ replace!
//         protocol.FetchedSource(k11CmsAttachment);
//
//         if (k11CmsAttachment.AttachmentFormGuid != null)
//         {
//             logger.LogWarning("Attachment '{AttachmentGuid}' is temporary => skipping", k11CmsAttachment.AttachmentGuid);
//             protocol.Append(HandbookReferences.TemporaryAttachmentMigrationIsNotSupported.WithData(new
//             {
//                 k11CmsAttachment.AttachmentId,
//                 k11CmsAttachment.AttachmentGuid,
//                 k11CmsAttachment.AttachmentName,
//                 k11CmsAttachment.AttachmentSiteId
//             }));
//             return new(false, true);
//         }
//
//         var k11AttachmentDocument = k11CmsAttachment.AttachmentDocumentId is int attachmentDocumentId
//             ? GetK11CmsDocument(attachmentDocumentId)
//             : null;
//
//         using var k11Context = k11ContextFactory.CreateDbContext();
//         var site = k11Context.CmsSites.FirstOrDefault(s=>s.SiteId == k11CmsAttachment.AttachmentSiteId) ?? throw new InvalidOperationException("Site not exists!");
//         if (!TryEnsureTargetLibraryExists(k11CmsAttachment.AttachmentSiteId, site.SiteName, out var targetMediaLibraryId))
//         {
//             return new(false, false);
//         }
//
//         var uploadedFile = CreateUploadFileFromAttachment(k11CmsAttachment);
//         if (uploadedFile == null)
//         {
//             protocol.Append(HandbookReferences
//                 .FailedToCreateTargetInstance<MediaFileInfo>()
//                 .WithIdentityPrint(k11CmsAttachment)
//                 .WithMessage("Failed to create dummy upload file containing data")
//             );
//             if (k11CmsAttachment.AttachmentBinary == null)
//             {
//                 logger.LogWarning("Failed to migrate attachment {Guid}, AttachmentBinary is null", k11CmsAttachment.AttachmentGuid);
//             }
//             else
//             {
//                 logger.LogWarning("Failed to migrate attachment {Guid}", k11CmsAttachment.AttachmentGuid);
//             }
//
//             return new(false, true);
//         }
//
//         var mediaFile = mediaFileFacade.GetMediaFile(k11CmsAttachment.AttachmentGuid);
//
//         protocol.FetchedTarget(mediaFile);
//
//         var librarySubFolder = "";
//         if (k11AttachmentDocument != null)
//         {
//             librarySubFolder = k11AttachmentDocument.DocumentNode.NodeAliasPath;
//         }
//
//         if (!string.IsNullOrWhiteSpace(additionalMediaPath) && (k11CmsAttachment.AttachmentIsUnsorted != true || k11CmsAttachment.AttachmentGroupGuid != null))
//         {
//             librarySubFolder = System.IO.Path.Combine(librarySubFolder, additionalMediaPath);
//         }
//
//         var mapped = attachmentMapper.Map(new CmsAttachmentMapperSource(k11CmsAttachment, targetMediaLibraryId, uploadedFile, librarySubFolder, k11AttachmentDocument), mediaFile);
//         protocol.MappedTarget(mapped);
//
//         if (mapped is (var mediaFileInfo, var newInstance) { Success: true })
//         {
//             Debug.Assert(mediaFileInfo != null, nameof(mediaFileInfo) + " != null");
//
//             try
//             {
//                 if (newInstance)
//                 {
//                     mediaFileFacade.EnsureMediaFilePathExistsInLibrary(mediaFileInfo, targetMediaLibraryId);
//                 }
//
//                 mediaFileFacade.SetMediaFile(mediaFileInfo, newInstance);
//
//                 protocol.Success(k11AttachmentDocument, mediaFileInfo, mapped);
//                 logger.LogEntitySetAction(newInstance, mediaFileInfo);
//
//                 return new(true, true, mediaFileInfo, MediaLibraryInfoProvider.ProviderObject.Get(targetMediaLibraryId));
//             }
//             catch (Exception exception)
//             {
//                 logger.LogEntitySetError(exception, newInstance, mediaFileInfo);
//                 protocol.Append(HandbookReferences.ErrorCreatingTargetInstance<MediaFileInfo>(exception)
//                     .NeedsManualAction()
//                     .WithIdentityPrint(mediaFileInfo)
//                     .WithData(new
//                     {
//                         mediaFileInfo.FileGUID,
//                         mediaFileInfo.FileName,
//                         // TODOV27 tomas.krch: 2023-09-05: obsolete - fileSiteID
//                         // mediaFileInfo.FileSiteID
//                     })
//                 );
//             }
//         }
//
//         return new(false, true);
//     }
//
//     private CmsDocument? GetK11CmsDocument(int documentId)
//     {
//         using var dbContext = k11ContextFactory.CreateDbContext();
//         return dbContext.CmsDocuments
//             .Include(d => d.DocumentNode)
//             .SingleOrDefault(a => a.DocumentId == documentId);
//     }
//
//
//     private IUploadedFile? CreateUploadFileFromAttachment(CmsAttachment attachment)
//     {
//         if (attachment.AttachmentBinary != null)
//         {
//             var ms = new MemoryStream(attachment.AttachmentBinary);
//             return DummyUploadedFile.FromStream(ms, attachment.AttachmentMimeType, attachment.AttachmentSize, attachment.AttachmentName);
//         }
//         else
//         {
//             return null;
//         }
//     }
//
//
//     private readonly ConcurrentDictionary<(string libraryName, int siteId), int> _mediaLibraryIdCache = new();
//
//     private bool TryEnsureTargetLibraryExists(int targetSiteId, string targetSiteName, out int targetLibraryId)
//     {
//         var targetLibraryCodeName = $"AttachmentsForSite{targetSiteName}";
//         var targetLibraryDisplayName = $"Attachments for site {targetSiteName}";
//         using var dbContext = kxpContextFactory.CreateDbContext();
//         try
//         {
//             targetLibraryId = _mediaLibraryIdCache.GetOrAdd((targetLibraryCodeName, targetSiteId), static (arg, context) => MediaLibraryFactory(arg, context), new MediaLibraryFactoryContext(mediaFileFacade, targetLibraryCodeName, targetLibraryDisplayName, dbContext));
//
//             return true;
//         }
//         catch (Exception exception)
//         {
//             logger.LogError(exception, "creating target media library failed");
//             protocol.Append(HandbookReferences.ErrorCreatingTargetInstance<MediaLibraryInfo>(exception)
//                 .NeedsManualAction()
//                 .WithData(new
//                 {
//                     TargetLibraryCodeName = targetLibraryCodeName,
//                     targetSiteId,
//                 })
//             );
//         }
//
//         targetLibraryId = 0;
//         return false;
//     }
//
//     private record MediaLibraryFactoryContext(KxpMediaFileFacade MediaFileFacade, string TargetLibraryCodeName, string TargetLibraryDisplayName, KxpContext DbContext);
//
//     private static readonly Regex SanitizationRegex =
//         RegexHelper.GetRegex("[^-_a-z0-9]", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
//
//     private static readonly Regex LibraryPathValidationRegex =
//         RegexHelper.GetRegex("^[-_a-z0-9]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);
//
//     private static int MediaLibraryFactory((string libraryName, int siteId) arg, MediaLibraryFactoryContext context)
//     {
//         var (libraryName, siteId) = arg;
//
//         // TODO tomas.krch: 2023-11-02 libraries now globalized, where do i put conflicting directories?
//         var tml = context.DbContext.MediaLibraries.SingleOrDefault(ml => ml.LibraryName == libraryName);
//
//         var libraryDirectory = context.TargetLibraryCodeName;
//         if (!LibraryPathValidationRegex.IsMatch(libraryDirectory))
//         {
//             libraryDirectory = SanitizationRegex.Replace(libraryDirectory, "_");
//         }
//
//         return tml?.LibraryId ?? context.MediaFileFacade.CreateMediaLibrary(siteId, libraryDirectory, "Created by Xperience Migration.Toolkit", context.TargetLibraryCodeName, context.TargetLibraryDisplayName).LibraryID;
//     }
// }