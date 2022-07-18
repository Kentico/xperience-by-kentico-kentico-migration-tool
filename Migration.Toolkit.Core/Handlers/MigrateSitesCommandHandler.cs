namespace Migration.Toolkit.Core.Handlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.MigrationProtocol;
using Migration.Toolkit.KX13.Context;
using Migration.Toolkit.KX13.Models;
using Migration.Toolkit.KXP.Context;

// ReSharper disable once UnusedType.Global
public class MigrateSitesCommandHandler: IRequestHandler<MigrateSitesCommand, CommandResult>, IDisposable
{
    private readonly ILogger<MigrateSitesCommandHandler> _logger;
    private readonly IDbContextFactory<KxpContext> _kxpContextFactory;
    private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;
    private readonly IEntityMapper<CmsSite, KXP.Models.CmsSite> _cmsSiteMapper;
    private readonly PrimaryKeyMappingContext _primaryKeyMappingContext;
    private readonly IProtocol _protocol;

    private KxpContext _kxpContext;

    public MigrateSitesCommandHandler(
        ILogger<MigrateSitesCommandHandler> logger,
        IDbContextFactory<KxpContext> kxpContextFactory,
        IDbContextFactory<KX13Context> kx13ContextFactory,
        IEntityMapper<CmsSite, KXP.Models.CmsSite> cmsSiteMapper,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol
    )
    {
        _logger = logger;
        _kxpContextFactory = kxpContextFactory;
        _kx13ContextFactory = kx13ContextFactory;
        _cmsSiteMapper = cmsSiteMapper;
        _primaryKeyMappingContext = primaryKeyMappingContext;
        _protocol = protocol;
        _kxpContext = _kxpContextFactory.CreateDbContext();
    }
    
    public async Task<CommandResult> Handle(MigrateSitesCommand request, CancellationToken cancellationToken)
    {
        await using var kx13Context = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        foreach (var kx13CmsSite in kx13Context.CmsSites)
        {
            _protocol.FetchedSource(kx13CmsSite);
            _logger.LogTrace("Migrating site {SiteName} with UserGuid {SiteGuid}", kx13CmsSite.SiteName, kx13CmsSite.SiteGuid);

            var targetSiteId = _primaryKeyMappingContext.MapFromSourceOrNull<CmsSite>(s => s.SiteId, kx13CmsSite.SiteId);
            if (targetSiteId is null)
            {
                // TODO tk: 2022-05-26 add site guid mapping
                _logger.LogWarning("Site '{SiteName}' with Guid {Guid} migration skipped", kx13CmsSite.SiteName, kx13CmsSite.SiteGuid);
                continue;
            }
            
            var kxoCmsSite = await _kxpContext.CmsSites.FirstOrDefaultAsync(u => u.SiteId == targetSiteId, cancellationToken);
            _protocol.FetchedTarget(kxoCmsSite);

            var mapped = _cmsSiteMapper.Map(kx13CmsSite, kxoCmsSite);
            _protocol.MappedTarget(mapped);

            if (mapped is { Success : true } result)
            {
                var (cmsSite, newInstance) = result;
                ArgumentNullException.ThrowIfNull(cmsSite, nameof(cmsSite));

                if (newInstance)
                {
                    _kxpContext.CmsSites.Add(cmsSite);
                }
                else
                {
                    _kxpContext.CmsSites.Update(cmsSite);
                }

                await _kxpContext.SaveChangesAsync(cancellationToken);

                _protocol.Success(kx13CmsSite, cmsSite, mapped);
                _logger.LogEntitySetAction(newInstance, cmsSite);
                    
                _primaryKeyMappingContext.SetMapping<CmsSite>(r => r.SiteId, kx13CmsSite.SiteId, cmsSite.SiteId);
            }
        }

        return new GenericCommandResult();
    }

    public void Dispose()
    {
        _kxpContext.Dispose();
    }
}