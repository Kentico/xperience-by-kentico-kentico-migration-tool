using CMS.MediaLibrary;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.Mappers;
using Migration.Toolkit.Core.MigrationProtocol;
using Migration.Toolkit.Core.Services;
using Migration.Toolkit.KX13.Context;
using Migration.Toolkit.KXO.Api;
using Migration.Toolkit.KXO.Context;

namespace Migration.Toolkit.Core.Handlers;

public class MigrateAttachmentsCommandHandler : IRequestHandler<MigrateAttachmentsCommand, CommandResult>, IDisposable
{
    private readonly ILogger<MigrateAttachmentsCommandHandler> _logger;
    private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;
    private readonly ToolkitConfiguration _toolkitConfiguration;
    private readonly PrimaryKeyMappingContext _primaryKeyMappingContext;
    private readonly KxoMediaFileFacade _mediaFileFacade;
    private readonly IDbContextFactory<KxoContext> _kxoContextFactory;

    private readonly IEntityMapper<CmsAttachmentMapperSource, MediaFileInfo> _attachmentMapper;
    private readonly IMigrationProtocol _migrationProtocol;
    private readonly AttachmentMigrator _attachmentMigrator;

    public MigrateAttachmentsCommandHandler(
        ILogger<MigrateAttachmentsCommandHandler> logger,
        IDbContextFactory<KX13.Context.KX13Context> kx13ContextFactory,
        ToolkitConfiguration toolkitConfiguration,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        KxoMediaFileFacade mediaFileFacade,
        IDbContextFactory<KXO.Context.KxoContext> kxoContextFactory,
        IEntityMapper<CmsAttachmentMapperSource, MediaFileInfo> attachmentMapper,
        IMigrationProtocol migrationProtocol,
        AttachmentMigrator attachmentMigrator
    )
    {
        _logger = logger;
        _kx13ContextFactory = kx13ContextFactory;
        _toolkitConfiguration = toolkitConfiguration;
        _primaryKeyMappingContext = primaryKeyMappingContext;
        _mediaFileFacade = mediaFileFacade;
        _kxoContextFactory = kxoContextFactory;
        _attachmentMapper = attachmentMapper;
        _migrationProtocol = migrationProtocol;
        _attachmentMigrator = attachmentMigrator;
    }
    
    public async Task<CommandResult> Handle(MigrateAttachmentsCommand request, CancellationToken cancellationToken)
    {
        var migratedSiteIds = _toolkitConfiguration.RequireExplicitMapping<KX13.Models.CmsSite>(s => s.SiteId).Keys.ToList();
        await using var kx13Context = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        var kx13CmsAttachments = kx13Context.CmsAttachments
            .Where(x => migratedSiteIds.Contains(x.AttachmentSiteId));

        foreach (var kx13CmsAttachment in kx13CmsAttachments)
        {
            var (_, canContinue, _, _) = _attachmentMigrator.MigrateAttachment(kx13CmsAttachment);
            if (!canContinue)
                break;
        }

        return new GenericCommandResult();
    }

    public void Dispose()
    {
        
    }
}