using System.Diagnostics;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Configuration;
using Migration.Toolkit.Core.Contexts;
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
    private readonly PkMappingContext _pkMappingContext;
    private readonly GlobalConfiguration _globalConfiguration;
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
        PkMappingContext pkMappingContext,
        GlobalConfiguration globalConfiguration
        )
    {
        _logger = logger;
        _mapper = mapper;
        _categoryMapper = categoryMapper;
        _resourceMapper = resourceMapper;
        _entityConfigurations = entityConfigurations;
        _kxoContextFactory = kxoContextFactory;
        _kx13ContextFactory = kx13ContextFactory;
        _pkMappingContext = pkMappingContext;
        _globalConfiguration = globalConfiguration;
        _kxoContext = _kxoContextFactory.CreateDbContext();
    }
   
    private void EnsureCmsResourceDependency(KX13.Models.CmsResource? sourceResource)
    {
        if (sourceResource != null)
        {
            _logger.LogTrace("Creating dependency for {cmsSettingsCategory} on {cmsResourceString} in target instance", nameof(KX13.Models.CmsSettingsCategory), nameof(KX13.Models.CmsResource));

            var targetResource = _kxoContext.CmsResources.FirstOrDefault(r => r.ResourceName == sourceResource.ResourceName);

            var mapped = _resourceMapper.Map(sourceResource, targetResource);
            
            mapped.LogResult(_logger);
            
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

                    _logger.LogInformation(newInstance
                        ? $"CmsResource: {cmsResource.ResourceName} was inserted."
                        : $"CmsResource: {cmsResource.ResourceName} was updated.");

                    _pkMappingContext.SetMapping<KX13.Models.CmsResource>(r => r.ResourceId, sourceResource.ResourceId, cmsResource.ResourceId);

                    return;
                default:
                    // TODO tk: 2022-05-18 mapping failed but already logged, do something..
                    break;
            }
        }
    }

    public async Task<MigrateSettingsKeysResult> Handle(MigrateSettingKeysCommand request, CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();
        await using var kx13Context = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        _logger.LogInformation("Selecting source CMS_SettingsCategory");
        var cmsSettingsCategories = kx13Context.CmsSettingsCategories
            .Include(sc => sc.CategoryResource)
            .OrderBy(sc => sc.CategoryLevel)
            .ThenBy(sc => sc.CategoryParentId);
        _logger.LogInformation("Selected source CMS_SettingsCategory, took: {took}", sw.Elapsed);

        foreach (var cmsSettingsCategory in cmsSettingsCategories)
        {
            _logger.LogInformation($"Migrating CmsSettingsCategory: {cmsSettingsCategory.CategoryName}");
            var target = _kxoContext.CmsSettingsCategories.FirstOrDefault(k => k.CategoryName == cmsSettingsCategory.CategoryName);
            
            // check required dependencies
            _logger.LogTrace("Check {cmsSettingsCategory} dependency on {cmsResourceString}", nameof(KX13.Models.CmsSettingsCategory), nameof(KX13.Models.CmsResourceString));
            EnsureCmsResourceDependency(cmsSettingsCategory.CategoryResource);
            
            var mapped = _categoryMapper.Map(cmsSettingsCategory, target);
            mapped.LogResult(_logger);
            
            switch (mapped)
            {
                case ModelMappingSuccess<KXO.Models.CmsSettingsCategory>(var result, var newInstance):
                    ArgumentNullException.ThrowIfNull(result, nameof(result));

                    if (cmsSettingsCategory.CategoryResourceId is int categoryResourceId)
                    {
                        result.CategoryResourceId = _pkMappingContext.MapFromSource<KX13.Models.CmsResource>(r => r.ResourceId, categoryResourceId);
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
                    _pkMappingContext.SetMapping<KX13.Models.CmsCategory>(c => c.CategoryId, cmsSettingsCategory.CategoryId, result.CategoryId);

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
        
        foreach (var cmsSettingsKey in cmsSettingsKeys)
        {
            var target = _kxoContext.CmsSettingsKeys.FirstOrDefault(k => k.KeyName == cmsSettingsKey.KeyName && k.SiteId == cmsSettingsKey.SiteId);
            var mapped = _mapper.Map(cmsSettingsKey, target);

            mapped.LogResult(_logger);
            
            switch (mapped)
            {
                case ModelMappingSuccess<KXO.Models.CmsSettingsKey>(var cmsSettingsKeyResult, var newInstance):
                    ArgumentNullException.ThrowIfNull(cmsSettingsKeyResult, nameof(cmsSettingsKeyResult));
                    
                    if (cmsSettingsKey.KeyCategoryId is int sourceCategoryId)
                    {
                        mapped.Item!.KeyCategoryId = _pkMappingContext.MapFromSource<KX13.Models.CmsCategory>(c => c.CategoryId, sourceCategoryId);
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