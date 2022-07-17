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
    private readonly IMigrationProtocol _migrationProtocol;

    public AttachmentMigrator(
        ILogger<AttachmentMigrator> logger,
        IDbContextFactory<KX13.Context.KX13Context> kx13ContextFactory,
        ToolkitConfiguration toolkitConfiguration,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        KxpMediaFileFacade mediaFileFacade,
        IDbContextFactory<KxpContext> kxpContextFactory,
        IEntityMapper<CmsAttachmentMapperSource, MediaFileInfo> attachmentMapper,
        IMigrationProtocol migrationProtocol
    )
    {
        _logger = logger;
        _kx13ContextFactory = kx13ContextFactory;
        _toolkitConfiguration = toolkitConfiguration;
        _primaryKeyMappingContext = primaryKeyMappingContext;
        _mediaFileFacade = mediaFileFacade;
        _kxpContextFactory = kxpContextFactory;
        _attachmentMapper = attachmentMapper;
        _migrationProtocol = migrationProtocol;
    }

    public record MigrateAttachmentResult(bool Success, bool CanContinue, MediaFileInfo? MediaFileInfo = null, MediaLibraryInfo? MediaLibraryInfo = null);

    public IEnumerable<MigrateAttachmentResult> MigrateGroupedAttachments(int documentId, Guid attachmentGroupGuid, string fieldName)
    {
        using var kx13Context = _kx13ContextFactory.CreateDbContext();
        var groupedAttachment = kx13Context.CmsAttachments.Where(a => a.AttachmentGroupGuid == attachmentGroupGuid && a.AttachmentDocumentId == documentId);
        foreach (var cmsAttachment in groupedAttachment)
        {
            yield return MigrateAttachment(cmsAttachment, fieldName);
        }
    }
    
    public MigrateAttachmentResult MigrateAttachment(Guid kx13CmsAttachmentGuid)
    {
        using var kx13Context = _kx13ContextFactory.CreateDbContext();
        var attachment = kx13Context.CmsAttachments.SingleOrDefault(a => a.AttachmentGuid == kx13CmsAttachmentGuid);
        if (attachment == null)
        {
            _logger.LogWarning("Attachment '{AttachmentGuid}' not found! => skipping", kx13CmsAttachmentGuid);
            _migrationProtocol.Append(HandbookReferences.TemporaryAttachmentMigrationIsNotSupported.WithData(new
            {
                AttachmentGuid = kx13CmsAttachmentGuid,
            }));
            return new MigrateAttachmentResult(false, true);
        }
        
        return MigrateAttachment(attachment);
    } 
    
    private readonly ConcurrentDictionary<int, CmsSite> _targetSites = new();
    
    public MigrateAttachmentResult MigrateAttachment(KX13M.CmsAttachment kx13CmsAttachment, string? additionalMedialPath = null)
    {
        var libraryNameMask = _toolkitConfiguration.TargetAttachmentMediaLibraryName;
        if (string.IsNullOrWhiteSpace(libraryNameMask))
        {
            _migrationProtocol.Append(HandbookReferences
                .MissingConfiguration<MigrateAttachmentsCommand>(nameof(_toolkitConfiguration.TargetAttachmentMediaLibraryName))
            );
            return new MigrateAttachmentResult(false, false);
        }
        
        _migrationProtocol.FetchedSource(kx13CmsAttachment);

        if (kx13CmsAttachment.AttachmentFormGuid != null)
        {
            _logger.LogWarning("Attachment '{AttachmentGuid}' is temporary => skipping", kx13CmsAttachment.AttachmentGuid);
            _migrationProtocol.Append(HandbookReferences.TemporaryAttachmentMigrationIsNotSupported.WithData(new
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

        var targetLibraryName = libraryNameMask
                .Replace("{sitename}", targetSite.SiteName, StringComparison.InvariantCultureIgnoreCase)
                .Replace("{siteid}", targetSite.SiteId.ToString(), StringComparison.InvariantCultureIgnoreCase)
            ;

        if (!TryEnsureTargetLibraryExists(targetLibraryName, targetSite.SiteId, out var targetMediaLibraryId))
        {
            return new(false, false);
        }

        var uploadedFile = CreateUploadFileFromAttachment(kx13CmsAttachment);
        if (uploadedFile == null)
        {
            _migrationProtocol.Append(HandbookReferences
                .FailedToCreateTargetInstance<MediaFileInfo>()
                .WithIdentityPrint(kx13CmsAttachment)
                .WithMessage("Failed to create dummy upload file containing data")
            );
            return new(false, true);
        }

        var mediaFile = _mediaFileFacade.GetMediaFile(kx13CmsAttachment.AttachmentGuid);

        _migrationProtocol.FetchedTarget(mediaFile);

        var librarySubFolder = "";
        if (kx13AttachmentDocument != null)
        {
            librarySubFolder = kx13AttachmentDocument.DocumentNode.NodeAliasPath;
        }

        if (!string.IsNullOrWhiteSpace(additionalMedialPath))
        {
            librarySubFolder = Path.Combine(librarySubFolder, additionalMedialPath);
        }

        var mapped = _attachmentMapper.Map(new CmsAttachmentMapperSource(kx13CmsAttachment, targetMediaLibraryId, uploadedFile, librarySubFolder), mediaFile);
        _migrationProtocol.MappedTarget(mapped);

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

                _migrationProtocol.Success(kx13AttachmentDocument, mediaFileInfo, mapped);
                _logger.LogEntitySetAction(newInstance, mediaFileInfo);

                return new(true, true, mediaFileInfo, MediaLibraryInfoProvider.ProviderObject.Get(targetMediaLibraryId));
            }
            catch (Exception exception)
            {
                _logger.LogEntitySetError(exception, newInstance, mediaFileInfo);
                _migrationProtocol.Append(HandbookReferences.ErrorCreatingTargetInstance<MediaFileInfo>(exception)
                    .NeedsManualAction()
                    .WithIdentityPrint(mediaFileInfo)
                    .WithData(new
                    {
                        targetLibraryName,
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
    private int EnsureMediaFileLibrary((string libraryName, int siteId) arg, KxpContext db)
    {
        var (libraryName, siteId) = arg;
        var tml = db.MediaLibraries.SingleOrDefault(ml => ml.LibrarySiteId == siteId && ml.LibraryName == libraryName);
        return tml?.LibraryId ?? _mediaFileFacade.CreateMediaLibrary(siteId, libraryName, "", libraryName, libraryName).LibraryID;
    }
    private bool TryEnsureTargetLibraryExists(string targetLibraryName, int targetSiteId, out int targetLibraryId)
    {
        using var dbContext = _kxpContextFactory.CreateDbContext();
        try
        {
            targetLibraryId = _mediaLibraryIdCache.GetOrAdd((targetLibraryName, targetSiteId), EnsureMediaFileLibrary, dbContext);
            
            return true;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "creating target media library failed");
            _migrationProtocol.Append(HandbookReferences.ErrorCreatingTargetInstance<MediaLibraryInfo>(exception)
                .NeedsManualAction()
                .WithData(new
                {
                    targetLibraryName,
                    targetSiteId,
                })
            );
        }

        targetLibraryId = 0;
        return false;
    }
}