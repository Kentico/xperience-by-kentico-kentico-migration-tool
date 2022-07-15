namespace Migration.Toolkit.Core.Handlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.MigrationProtocol;
using Migration.Toolkit.KX13.Context;
using Migration.Toolkit.KX13.Models;
using Migration.Toolkit.KXO.Context;

// ReSharper disable once UnusedType.Global
public class MigrateSitesCommandHandler: IRequestHandler<MigrateSitesCommand, CommandResult>, IDisposable
{
    private readonly ILogger<MigrateSitesCommandHandler> _logger;
    private readonly IDbContextFactory<KxoContext> _kxoContextFactory;
    private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;
    private readonly IEntityMapper<CmsSite, KXO.Models.CmsSite> _cmsSiteMapper;
    private readonly PrimaryKeyMappingContext _primaryKeyMappingContext;
    private readonly IMigrationProtocol _migrationProtocol;

    private KxoContext _kxoContext;

    public MigrateSitesCommandHandler(
        ILogger<MigrateSitesCommandHandler> logger,
        IDbContextFactory<KxoContext> kxoContextFactory,
        IDbContextFactory<KX13Context> kx13ContextFactory,
        IEntityMapper<CmsSite, KXO.Models.CmsSite> cmsSiteMapper,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IMigrationProtocol migrationProtocol
    )
    {
        _logger = logger;
        _kxoContextFactory = kxoContextFactory;
        _kx13ContextFactory = kx13ContextFactory;
        _cmsSiteMapper = cmsSiteMapper;
        _primaryKeyMappingContext = primaryKeyMappingContext;
        _migrationProtocol = migrationProtocol;
        _kxoContext = _kxoContextFactory.CreateDbContext();
    }
    
    public async Task<CommandResult> Handle(MigrateSitesCommand request, CancellationToken cancellationToken)
    {
        await using var kx13Context = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        foreach (var kx13CmsSite in kx13Context.CmsSites)
        {
            _migrationProtocol.FetchedSource(kx13CmsSite);
            _logger.LogTrace("Migrating site {SiteName} with UserGuid {SiteGuid}", kx13CmsSite.SiteName, kx13CmsSite.SiteGuid);

            var targetSiteId = _primaryKeyMappingContext.MapFromSourceOrNull<CmsSite>(s => s.SiteId, kx13CmsSite.SiteId);
            if (targetSiteId is null)
            {
                // TODO tk: 2022-05-26 add site guid mapping
                _logger.LogWarning("Site '{SiteName}' with Guid {Guid} migration skipped", kx13CmsSite.SiteName, kx13CmsSite.SiteGuid);
                continue;
            }
            
            var kxoCmsSite = await _kxoContext.CmsSites.FirstOrDefaultAsync(u => u.SiteId == targetSiteId, cancellationToken);
            _migrationProtocol.FetchedTarget(kxoCmsSite);

            var mapped = _cmsSiteMapper.Map(kx13CmsSite, kxoCmsSite);
            _migrationProtocol.MappedTarget(mapped);

            if (mapped is { Success : true } result)
            {
                var (cmsSite, newInstance) = result;
                ArgumentNullException.ThrowIfNull(cmsSite, nameof(cmsSite));

                if (newInstance)
                {
                    _kxoContext.CmsSites.Add(cmsSite);
                }
                else
                {
                    _kxoContext.CmsSites.Update(cmsSite);
                }

                await _kxoContext.SaveChangesAsync(cancellationToken);

                _migrationProtocol.Success(kx13CmsSite, cmsSite, mapped);
                _logger.LogEntitySetAction(newInstance, cmsSite);
                    
                _primaryKeyMappingContext.SetMapping<CmsSite>(r => r.SiteId, kx13CmsSite.SiteId, cmsSite.SiteId);
            }
        }

        return new GenericCommandResult();
    }

    public void Dispose()
    {
        _kxoContext.Dispose();
    }
}