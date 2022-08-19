using System.Collections.Concurrent;
using System.Diagnostics;
using CMS.Base;
using CMS.MediaLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.Mappers;
using Migration.Toolkit.Core.MigrationProtocol;
using Migration.Toolkit.KX13.Context;

namespace Migration.Toolkit.Core.Services;

using System.Text;
using System.Text.RegularExpressions;
using CMS.Helpers;
using CMS.IO;
using Migration.Toolkit.KXP.Api;
using Migration.Toolkit.KXP.Api.Auxiliary;
using Migration.Toolkit.KXP.Context;
using Migration.Toolkit.KXP.Models;

public class AttachmentMigrator
{
    private readonly ILogger<AttachmentMigrator> _logger;
    private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;
    private readonly ToolkitConfiguration _toolkitConfiguration;
    private readonly PrimaryKeyMappingContext _primaryKeyMappingContext;
    private readonly KxpMediaFileFacade _mediaFileFacade;
    private readonly IDbContextFactory<KxpContext> _kxpContextFactory;
    private readonly IEntityMapper<CmsAttachmentMapperSource, MediaFileInfo> _attachmentMapper;
    private readonly IProtocol _protocol;

    public AttachmentMigrator(
        ILogger<AttachmentMigrator> logger,
        IDbContextFactory<KX13.Context.KX13Context> kx13ContextFactory,
        ToolkitConfiguration toolkitConfiguration,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        KxpMediaFileFacade mediaFileFacade,
        IDbContextFactory<KxpContext> kxpContextFactory,
        IEntityMapper<CmsAttachmentMapperSource, MediaFileInfo> attachmentMapper,
        IProtocol protocol
    )
    {
        _logger = logger;
        _kx13ContextFactory = kx13ContextFactory;
        _toolkitConfiguration = toolkitConfiguration;
        _primaryKeyMappingContext = primaryKeyMappingContext;
        _mediaFileFacade = mediaFileFacade;
        _kxpContextFactory = kxpContextFactory;
        _attachmentMapper = attachmentMapper;
        _protocol = protocol;
    }

    public record MigrateAttachmentResult(bool Success, bool CanContinue, MediaFileInfo? MediaFileInfo = null,
        MediaLibraryInfo? MediaLibraryInfo = null);

    public IEnumerable<MigrateAttachmentResult> MigrateGroupedAttachments(int documentId, Guid attachmentGroupGuid, string fieldName)
    {
        using var kx13Context = _kx13ContextFactory.CreateDbContext();
        var groupedAttachment =
            kx13Context.CmsAttachments.Where(a => a.AttachmentGroupGuid == attachmentGroupGuid && a.AttachmentDocumentId == documentId);
        foreach (var cmsAttachment in groupedAttachment)
        {
            yield return MigrateAttachment(cmsAttachment, $"__{fieldName}");
        }
    }

    public MigrateAttachmentResult MigrateAttachment(Guid kx13CmsAttachmentGuid)
    {
        using var kx13Context = _kx13ContextFactory.CreateDbContext();
        var attachment = kx13Context.CmsAttachments.SingleOrDefault(a => a.AttachmentGuid == kx13CmsAttachmentGuid);
        if (attachment == null)
        {
            _logger.LogWarning("Attachment '{AttachmentGuid}' not found! => skipping", kx13CmsAttachmentGuid);
            _protocol.Append(HandbookReferences.TemporaryAttachmentMigrationIsNotSupported.WithData(new
            {
                AttachmentGuid = kx13CmsAttachmentGuid,
            }));
            return new MigrateAttachmentResult(false, true);
        }

        return MigrateAttachment(attachment);
    }

    private readonly ConcurrentDictionary<int, CmsSite> _targetSites = new();

    public MigrateAttachmentResult MigrateAttachment(KX13M.CmsAttachment kx13CmsAttachment, string? additionalMediaPath = null)
    {
        // TODO tomas.krch: 2022-08-18 directory validation only -_ replace!
        _protocol.FetchedSource(kx13CmsAttachment);

        if (kx13CmsAttachment.AttachmentFormGuid != null)
        {
            _logger.LogWarning("Attachment '{AttachmentGuid}' is temporary => skipping", kx13CmsAttachment.AttachmentGuid);
            _protocol.Append(HandbookReferences.TemporaryAttachmentMigrationIsNotSupported.WithData(new
            {
                kx13CmsAttachment.AttachmentId,
                kx13CmsAttachment.AttachmentGuid,
                kx13CmsAttachment.AttachmentName,
                kx13CmsAttachment.AttachmentSiteId
            }));
            return new(false, true);
        }

        var kx13AttachmentDocument = kx13CmsAttachment.AttachmentDocumentId is int attachmentDocumentId
            ? GetKx13CmsDocument(attachmentDocumentId)
            : null;
        
        var targetSiteId = _primaryKeyMappingContext.RequireMapFromSource<KX13.Models.CmsSite>(s => s.SiteId, kx13CmsAttachment.AttachmentSiteId);
        var targetSite = _targetSites.GetOrAdd(targetSiteId, i =>
        {
            using var kxoDbContext = _kxpContextFactory.CreateDbContext();
            return kxoDbContext.CmsSites.Single(s => s.SiteId == targetSiteId);
        });

        if (!TryEnsureTargetLibraryExists(targetSite.SiteId, targetSite.SiteName, out var targetMediaLibraryId))
        {
            return new(false, false);
        }

        var uploadedFile = CreateUploadFileFromAttachment(kx13CmsAttachment);
        if (uploadedFile == null)
        {
            _protocol.Append(HandbookReferences
                .FailedToCreateTargetInstance<MediaFileInfo>()
                .WithIdentityPrint(kx13CmsAttachment)
                .WithMessage("Failed to create dummy upload file containing data")
            );
            return new(false, true);
        }

        var mediaFile = _mediaFileFacade.GetMediaFile(kx13CmsAttachment.AttachmentGuid);

        _protocol.FetchedTarget(mediaFile);

        var librarySubFolder = "";
        if (kx13AttachmentDocument != null)
        {
            librarySubFolder = kx13AttachmentDocument.DocumentNode.NodeAliasPath;
        }

        if (!string.IsNullOrWhiteSpace(additionalMediaPath))
        {
            librarySubFolder = System.IO.Path.Combine(librarySubFolder, additionalMediaPath);
        }

        var mapped = _attachmentMapper.Map(new CmsAttachmentMapperSource(kx13CmsAttachment, targetMediaLibraryId, uploadedFile, librarySubFolder, kx13AttachmentDocument), mediaFile);
        _protocol.MappedTarget(mapped);

        if (mapped is (var mediaFileInfo, var newInstance) { Success: true })
        {
            Debug.Assert(mediaFileInfo != null, nameof(mediaFileInfo) + " != null");

            try
            {
                if (newInstance)
                {
                    _mediaFileFacade.EnsureMediaFilePathExistsInLibrary(mediaFileInfo, targetMediaLibraryId, targetSite.SiteName);
                }

                _mediaFileFacade.SetMediaFile(mediaFileInfo, newInstance);

                _protocol.Success(kx13AttachmentDocument, mediaFileInfo, mapped);
                _logger.LogEntitySetAction(newInstance, mediaFileInfo);

                return new(true, true, mediaFileInfo, MediaLibraryInfoProvider.ProviderObject.Get(targetMediaLibraryId));
            }
            catch (Exception exception)
            {
                _logger.LogEntitySetError(exception, newInstance, mediaFileInfo);
                _protocol.Append(HandbookReferences.ErrorCreatingTargetInstance<MediaFileInfo>(exception)
                    .NeedsManualAction()
                    .WithIdentityPrint(mediaFileInfo)
                    .WithData(new
                    {
                        targetSiteId,
                        mediaFileInfo.FileGUID,
                        mediaFileInfo.FileName,
                        mediaFileInfo.FileSiteID
                    })
                );
            }
        }

        return new(false, true);
    }

    private KX13M.CmsDocument? GetKx13CmsDocument(int documentId)
    {
        using var dbContext = _kx13ContextFactory.CreateDbContext();
        return dbContext.CmsDocuments
            .Include(d => d.DocumentNode)
            .SingleOrDefault(a => a.DocumentId == documentId);
    }


    private IUploadedFile? CreateUploadFileFromAttachment(KX13M.CmsAttachment attachment)
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
        var targetLibraryCodeName = $"AttachmentsForSite{targetSiteName}";
        var targetLibraryDisplayName = $"Attachments for site {targetSiteName}";
        using var dbContext = _kxpContextFactory.CreateDbContext();
        try
        {
            targetLibraryId = _mediaLibraryIdCache.GetOrAdd((targetLibraryCodeName, targetSiteId), static (arg, context) => MediaLibraryFactory(arg, context), new MediaLibraryFactoryContext(_mediaFileFacade, targetLibraryCodeName, targetLibraryDisplayName, dbContext));

            return true;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "creating target media library failed");
            _protocol.Append(HandbookReferences.ErrorCreatingTargetInstance<MediaLibraryInfo>(exception)
                .NeedsManualAction()
                .WithData(new
                {
                    TargetLibraryCodeName = targetLibraryCodeName,
                    targetSiteId,
                })
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
        var tml = context.DbContext.MediaLibraries.SingleOrDefault(ml => ml.LibrarySiteId == siteId && ml.LibraryName == libraryName);
        
        var libraryDirectory = context.TargetLibraryCodeName;
        if (!LibraryPathValidationRegex.IsMatch(libraryDirectory))
        {
            libraryDirectory = SanitizationRegex.Replace(libraryDirectory, "_");
        }

        return tml?.LibraryId ?? context.MediaFileFacade.CreateMediaLibrary(siteId, libraryDirectory, "Created by Xperience Migration.Toolkit", context.TargetLibraryCodeName, context.TargetLibraryDisplayName).LibraryID;
    }
}