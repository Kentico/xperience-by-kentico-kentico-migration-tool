using System.Diagnostics;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Configuration;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.MigrationProtocol;
using Migration.Toolkit.KX13.Context;
using Migration.Toolkit.KX13.Models;
using Migration.Toolkit.KXO.Context;

namespace Migration.Toolkit.Core.MigrateSettingKeys;

public class MigrateSettingKeysCommandHandler: IRequestHandler<MigrateSettingKeysCommand, MigrateSettingsKeysResult>, IDisposable
{
    private readonly ILogger<MigrateSettingKeysCommandHandler> _logger;
    private readonly EntityConfigurations _entityConfigurations;
    private readonly IDbContextFactory<KxoContext> _kxoContextFactory;
    private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;
    private readonly PrimaryKeyMappingContext _primaryKeyMappingContext;
    private readonly GlobalConfiguration _globalConfiguration;
    private readonly IMigrationProtocol _migrationProtocol;
    private readonly IEntityMapper<KX13.Models.CmsSettingsKey, KXO.Models.CmsSettingsKey> _mapper;
    private readonly IEntityMapper<CmsSettingsCategory, KXO.Models.CmsSettingsCategory> _categoryMapper;
    private readonly IEntityMapper<KX13.Models.CmsResource, KXO.Models.CmsResource> _resourceMapper;
    
    private KxoContext _kxoContext;

    public MigrateSettingKeysCommandHandler(
        ILogger<MigrateSettingKeysCommandHandler> logger,
        IEntityMapper<KX13.Models.CmsSettingsKey, KXO.Models.CmsSettingsKey> mapper,
        IEntityMapper<KX13.Models.CmsSettingsCategory, KXO.Models.CmsSettingsCategory> categoryMapper,
        IEntityMapper<KX13.Models.CmsResource, KXO.Models.CmsResource> resourceMapper,
        EntityConfigurations entityConfigurations,
        IDbContextFactory<KXO.Context.KxoContext> kxoContextFactory,
        IDbContextFactory<KX13.Context.KX13Context> kx13ContextFactory,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        GlobalConfiguration globalConfiguration,
        IMigrationProtocol migrationProtocol
        )
    {
        _logger = logger;
        _mapper = mapper;
        _categoryMapper = categoryMapper;
        _resourceMapper = resourceMapper;
        _entityConfigurations = entityConfigurations;
        _kxoContextFactory = kxoContextFactory;
        _kx13ContextFactory = kx13ContextFactory;
        _primaryKeyMappingContext = primaryKeyMappingContext;
        _globalConfiguration = globalConfiguration;
        _migrationProtocol = migrationProtocol;
        _kxoContext = _kxoContextFactory.CreateDbContext();
    }
   
    private void EnsureCmsResourceDependency(KX13.Models.CmsResource? kx13CmsResource)
    {
        _migrationProtocol.FetchedSource(kx13CmsResource);
        
        if (kx13CmsResource != null)
        {
            _logger.LogTrace("Creating dependency for {cmsSettingsCategory} on {cmsResourceString} in target instance", nameof(KX13.Models.CmsSettingsCategory), nameof(KX13.Models.CmsResource));

            var kxoCmsResource = _kxoContext.CmsResources.FirstOrDefault(r => r.ResourceName == kx13CmsResource.ResourceName);
            _migrationProtocol.FetchedTarget(kxoCmsResource);

            var mapped = _resourceMapper.Map(kx13CmsResource, kxoCmsResource);
            mapped.LogResult(_logger);
            _migrationProtocol.MappedTarget(mapped);
            
            switch (mapped)
            {
                case ModelMappingSuccess<KXO.Models.CmsResource>(var cmsResource, var newInstance):
                    ArgumentNullException.ThrowIfNull(cmsResource, nameof(cmsResource));
                    
                    if (newInstance)
                    {
                        _kxoContext.CmsResources.Add(cmsResource);
                    }
                    else
                    {
                        _kxoContext.CmsResources.Update(cmsResource);
                    }

                    _kxoContext.SaveChanges(); // TODO tk: 2022-05-18 context needs to be disposed/recreated after error
                    _migrationProtocol.Success(kx13CmsResource, kxoCmsResource, mapped);
                    
                    _logger.LogInformation(newInstance
                        ? $"CmsResource: {cmsResource.ResourceName} was inserted."
                        : $"CmsResource: {cmsResource.ResourceName} was updated.");

                    _primaryKeyMappingContext.SetMapping<KX13.Models.CmsResource>(r => r.ResourceId, kx13CmsResource.ResourceId, cmsResource.ResourceId);

                    return;
                default:
                    // TODO tk: 2022-05-18 mapping failed but already logged, do something..
                    break;
            }
        }
    }

    public async Task<MigrateSettingsKeysResult> Handle(MigrateSettingKeysCommand request, CancellationToken cancellationToken)
    {
        using var protocolScope = _migrationProtocol.CreateScope<MigrateSettingKeysCommandHandler>();
        
        var sw = Stopwatch.StartNew();
        await using var kx13Context = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        _logger.LogInformation("Selecting source CMS_SettingsCategory");
        var cmsSettingsCategories = kx13Context.CmsSettingsCategories
            .Include(sc => sc.CategoryResource)
            .OrderBy(sc => sc.CategoryLevel)
            .ThenBy(sc => sc.CategoryParentId);
        _logger.LogInformation("Selected source CMS_SettingsCategory, took: {took}", sw.Elapsed);

        foreach (var kx13CmsSettingsCategory in cmsSettingsCategories)
        {
            _migrationProtocol.FetchedSource(kx13CmsSettingsCategory);
            
            _logger.LogInformation($"Migrating CmsSettingsCategory: {kx13CmsSettingsCategory.CategoryName}");
            var kxoCmsSettingsCategory = _kxoContext.CmsSettingsCategories.FirstOrDefault(k => k.CategoryName == kx13CmsSettingsCategory.CategoryName);
            _migrationProtocol.FetchedTarget(kxoCmsSettingsCategory);

            // check required dependencies
            _logger.LogTrace("Check {cmsSettingsCategory} dependency on {cmsResourceString}", nameof(KX13.Models.CmsSettingsCategory), nameof(KX13.Models.CmsResourceString));
            EnsureCmsResourceDependency(kx13CmsSettingsCategory.CategoryResource);
            
            var mapped = _categoryMapper.Map(kx13CmsSettingsCategory, kxoCmsSettingsCategory);
            mapped.LogResult(_logger);
            _migrationProtocol.MappedTarget(mapped);
            
            switch (mapped)
            {
                case ModelMappingSuccess<KXO.Models.CmsSettingsCategory>(var result, var newInstance):
                    ArgumentNullException.ThrowIfNull(result, nameof(result));

                    if (kx13CmsSettingsCategory.CategoryResourceId is int categoryResourceId)
                    {
                        result.CategoryResourceId = _primaryKeyMappingContext.RequireMapFromSource<KX13.Models.CmsResource>(r => r.ResourceId, categoryResourceId);
                    }
                    
                    if (newInstance)
                    {
                        _kxoContext.CmsSettingsCategories.Add(result);
                    }
                    else
                    {
                        _kxoContext.CmsSettingsCategories.Update(result);
                    }

                    await _kxoContext.SaveChangesAsync(cancellationToken);
                    
                    _migrationProtocol.Success(kx13CmsSettingsCategory, kxoCmsSettingsCategory, mapped);
                    
                    _primaryKeyMappingContext.SetMapping<KX13.Models.CmsCategory>(c => c.CategoryId, kx13CmsSettingsCategory.CategoryId, result.CategoryId);

                    _logger.LogInformation(newInstance
                        ? $"CmsSettingsCategory: {result.CategoryName} was inserted."
                        : $"CmsSettingsCategory: {result.CategoryName} was updated.");

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mapped));
            }
        }

        _logger.LogInformation($"CmsSettingsKey synchronization starting");
        var cmsSettingsKeys = kx13Context.CmsSettingsKeys
                .Where(csk=> _globalConfiguration.SiteIdMapping.Keys.Contains(csk.SiteId) || csk.SiteId == null)
            // .Where(k => k.KeyIsCustom == true)
            ;
        
        foreach (var kx13CmsSettingsKey in cmsSettingsKeys)
        {
            _migrationProtocol.FetchedSource(kx13CmsSettingsKey);
            
            var kxoCmsSettingsKey = _kxoContext.CmsSettingsKeys.FirstOrDefault(k => k.KeyName == kx13CmsSettingsKey.KeyName && k.SiteId == kx13CmsSettingsKey.SiteId);
            _migrationProtocol.FetchedTarget(kxoCmsSettingsKey);
            
            var mapped = _mapper.Map(kx13CmsSettingsKey, kxoCmsSettingsKey);
            mapped.LogResult(_logger);
            _migrationProtocol.MappedTarget(mapped);
            
            switch (mapped)
            {
                case ModelMappingSuccess<KXO.Models.CmsSettingsKey>(var cmsSettingsKeyResult, var newInstance):
                    ArgumentNullException.ThrowIfNull(cmsSettingsKeyResult, nameof(cmsSettingsKeyResult));
                    
                    if (kx13CmsSettingsKey.KeyCategoryId is int sourceCategoryId)
                    {
                        mapped.Item!.KeyCategoryId = _primaryKeyMappingContext.RequireMapFromSource<KX13.Models.CmsCategory>(c => c.CategoryId, sourceCategoryId);
                    }
                    
                    if (newInstance)
                    {
                        _kxoContext.CmsSettingsKeys.Add(cmsSettingsKeyResult);
                    }
                    else
                    {
                        _kxoContext.CmsSettingsKeys.Update(cmsSettingsKeyResult);
                    }

                    await _kxoContext.SaveChangesAsync(cancellationToken); // TODO tk: 2022-05-18 context needs to be disposed/recreated after error
                    
                    _migrationProtocol.Success(kx13CmsSettingsKey, kxoCmsSettingsKey, mapped);

                    _logger.LogInformation(newInstance
                        ? $"CmsSettingsKey: {cmsSettingsKeyResult.KeyName} was inserted."
                        : $"CmsSettingsKey: {cmsSettingsKeyResult.KeyName} was updated.");

                    // kxoContext.SaveChangesWithIdentityInsert<KXO.Models.CmsClass>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mapped));
            }
        }
        
        _logger.LogInformation("Finished: {took}", sw.Elapsed);

        return new MigrateSettingsKeysResult();
    }

    public void Dispose()
    {
        _kxoContext.Dispose();
    }
}