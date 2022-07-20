namespace Migration.Toolkit.Core.Handlers;

using CMS.DataEngine;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.MigrationProtocol;
using Migration.Toolkit.KX13.Context;
using Migration.Toolkit.KX13.Models;
using Migration.Toolkit.KXP.Context;

// TODO tk: 2022-06-01 Q - dle jakého klíče přenášet kategorie settings? jen pro klíče, které budou přeneseny do cílové instance?

public class MigrateSettingKeysCommandHandler: IRequestHandler<MigrateSettingKeysCommand, CommandResult>, IDisposable
{
    private readonly ILogger<MigrateSettingKeysCommandHandler> _logger;
    private readonly IDbContextFactory<KxpContext> _kxpContextFactory;
    private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;
    private readonly ToolkitConfiguration _toolkitConfiguration;
    private readonly IProtocol _protocol;
    private readonly IEntityMapper<CmsSettingsKey, KXP.Models.CmsSettingsKey> _mapper;

    private KxpContext _kxpContext;

    public MigrateSettingKeysCommandHandler(
        ILogger<MigrateSettingKeysCommandHandler> logger,
        IEntityMapper<CmsSettingsKey, KXP.Models.CmsSettingsKey> mapper,
        IDbContextFactory<KxpContext> kxpContextFactory,
        IDbContextFactory<KX13Context> kx13ContextFactory,
        ToolkitConfiguration toolkitConfiguration,
        IProtocol protocol
        )
    {
        _logger = logger;
        _mapper = mapper;
        _kxpContextFactory = kxpContextFactory;
        _kx13ContextFactory = kx13ContextFactory;
        _toolkitConfiguration = toolkitConfiguration;
        _protocol = protocol;
        _kxpContext = _kxpContextFactory.CreateDbContext();
    }

    public async Task<CommandResult> Handle(MigrateSettingKeysCommand request, CancellationToken cancellationToken)
    {
        var entityConfiguration = _toolkitConfiguration.EntityConfigurations.GetEntityConfiguration<CmsSettingsKey>();
        var migratedSiteIds = _toolkitConfiguration.RequireExplicitMapping<CmsSite>(s => s.SiteId).Keys.ToList();
        
        await using var kx13Context = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        _logger.LogInformation("CmsSettingsKey synchronization starting");
        var cmsSettingsKeys = kx13Context.CmsSettingsKeys
                // .Include(sk => sk.KeyCategory.CategoryResource)
                // .Include(sk => sk.KeyCategory.CategoryParent.CategoryResource)
                // .Include(sk => sk.KeyCategory.CategoryParent.CategoryParent.CategoryResource)
                .Where(csk => migratedSiteIds.Contains(csk.SiteId!.Value) || csk.SiteId == null)
                .AsNoTrackingWithIdentityResolution()
            ;

        foreach (var kx13CmsSettingsKey in cmsSettingsKeys)
        {   
            _protocol.FetchedSource(kx13CmsSettingsKey);

            var kxoGlobalSettingsKey = GetKxoSettingsKey(kx13CmsSettingsKey, null);
            
            var canBeMigrated = !kxoGlobalSettingsKey?.KeyIsHidden ?? false;
            var kxoCmsSettingsKey = kx13CmsSettingsKey.SiteId is null ? kxoGlobalSettingsKey : GetKxoSettingsKey(kx13CmsSettingsKey, kx13CmsSettingsKey.SiteId);

            if (!canBeMigrated)
            {
                _logger.LogWarning("Setting with key '{KeyName}' is currently not supported for migration", kx13CmsSettingsKey.KeyName);
                _protocol.Append(
                    HandbookReferences
                        .NotCurrentlySupportedSkip<SettingsKeyInfo>()
                        .WithId(nameof(kx13CmsSettingsKey.KeyId), kx13CmsSettingsKey.KeyId)
                        .WithMessage("Settings key is not supported in target instance")
                        .WithData(new
                        {
                            kx13CmsSettingsKey.KeyName,
                            kx13CmsSettingsKey.SiteId,
                            kx13CmsSettingsKey.KeyGuid
                        })
                );
                continue;
            }

            _protocol.FetchedTarget(kxoCmsSettingsKey);

            if (entityConfiguration.ExcludeCodeNames.Contains(kx13CmsSettingsKey.KeyName))
            {
                _protocol.Warning(HandbookReferences.CmsSettingsKeyExclusionListSkip, kx13CmsSettingsKey);
                _logger.LogWarning("KeyName {KeyName} is excluded => skipping", kx13CmsSettingsKey.KeyName);
                continue;
            }

            var mapped = _mapper.Map(kx13CmsSettingsKey, kxoCmsSettingsKey);
            _protocol.MappedTarget(mapped);

            if (mapped is { Success: true } result)
            {
                ArgumentNullException.ThrowIfNull(result.Item, nameof(result.Item));

                if (result.NewInstance)
                {
                    _kxpContext.CmsSettingsKeys.Add(result.Item);
                }
                else
                {
                    _kxpContext.CmsSettingsKeys.Update(result.Item);
                }

                await _kxpContext.SaveChangesAsync(cancellationToken);
                _protocol.Success(kx13CmsSettingsKey, kxoCmsSettingsKey, mapped);
                _logger.LogEntitySetAction(result.NewInstance, result.Item);

                // TODO tk: 2022-07-15 catch error & reset dbContext on error
            }
        }

        return new GenericCommandResult();
    }

    private KXP.Models.CmsSettingsKey? GetKxoSettingsKey(CmsSettingsKey kx13CmsSettingsKey, int? siteId)
    {
        using var kxpContext = _kxpContextFactory.CreateDbContext();
        
        return kxpContext.CmsSettingsKeys
            .SingleOrDefault(k => k.KeyName == kx13CmsSettingsKey.KeyName && k.SiteId == siteId);
    }

    public void Dispose()
    {
        _kxpContext.Dispose();
    }
}