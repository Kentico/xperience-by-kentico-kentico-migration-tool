using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Configuration;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.KX13.Context;
using Migration.Toolkit.KX13.Models;
using Migration.Toolkit.KXO.Context;

namespace Migration.Toolkit.Core.MigrateSettingKeysCommand;

public class MigrateSettingKeysCommand: IMigrateSettingKeysCommand
{
    private readonly ILogger<MigratePageTypesCommand.MigratePageTypesCommand> _logger;
    private readonly EntityConfigurations _entityConfigurations;
    private readonly IDbContextFactory<KxoContext> _kxoContextFactory;
    private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;
    private readonly PkMappingContext _pkMappingContext;
    private readonly GlobalConfiguration _globalConfiguration;
    private readonly IEntityMapper<KX13.Models.CmsSettingsKey, KXO.Models.CmsSettingsKey> _mapper;
    private readonly IEntityMapper<CmsSettingsCategory, KXO.Models.CmsSettingsCategory> _categoryMapper;
    private readonly IEntityMapper<KX13.Models.CmsResource, KXO.Models.CmsResource> _resourceMapper;

    public MigrateSettingKeysCommand(
        ILogger<MigratePageTypesCommand.MigratePageTypesCommand> logger,
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
    }
    
    public void Execute()
    {
        var sw = Stopwatch.StartNew();
        using var kx13Context = _kx13ContextFactory.CreateDbContext();
        using var kxoContext = _kxoContextFactory.CreateDbContext();

        _logger.LogInformation("Selecting source CMS_SettingsCategory");
        var cmsSettingsCategories = kx13Context.CmsSettingsCategories
            .Include(sc => sc.CategoryResource)
            .OrderBy(sc => sc.CategoryLevel)
            .ThenBy(sc => sc.CategoryParentId);
        _logger.LogInformation("Selected source CMS_SettingsCategory, took: {took}", sw.Elapsed);

        foreach (var cmsSettingsCategory in cmsSettingsCategories)
        {
            _logger.LogInformation($"CmsSettingsCategory: {cmsSettingsCategory.CategoryName} starting.");
            var target = kxoContext.CmsSettingsCategories.FirstOrDefault(k => k.CategoryName == cmsSettingsCategory.CategoryName);
            
            // check required dependencies
            _logger.LogTrace("Check {cmsSettingsCategory} dependency on {cmsResourceString}", nameof(KX13.Models.CmsSettingsCategory), nameof(KX13.Models.CmsResourceString));
            EnsureCmsResourceDependency(cmsSettingsCategory.CategoryResource, kxoContext);
            
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
                        kxoContext.CmsSettingsCategories.Add(result);
                        _logger.LogInformation($"CmsSettingsCategory: {result.CategoryName} was inserted.");
                    }
                    else
                    {
                        kxoContext.CmsSettingsCategories.Update(result);
                        _logger.LogInformation($"CmsSettingsCategory: {result.CategoryName} was updated.");
                    }

                    kxoContext.SaveChanges();
                    _pkMappingContext.SetMapping<KX13.Models.CmsCategory>(c => c.CategoryId, cmsSettingsCategory.CategoryId, result.CategoryId);

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
            var target = kxoContext.CmsSettingsKeys.FirstOrDefault(k => k.KeyName == cmsSettingsKey.KeyName && k.SiteId == cmsSettingsKey.SiteId);
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
                        kxoContext.CmsSettingsKeys.Add(cmsSettingsKeyResult);
                        _logger.LogInformation($"CmsSettingsKey: {cmsSettingsKeyResult.KeyName} was inserted.");
                    }
                    else
                    {
                        kxoContext.CmsSettingsKeys.Update(cmsSettingsKeyResult);
                        _logger.LogInformation($"CmsSettingsKey: {cmsSettingsKeyResult.KeyName} was updated.");
                    }

                    kxoContext.SaveChanges();
                    // kxoContext.SaveChangesWithIdentityInsert<KXO.Models.CmsClass>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mapped));
            }
        }
        
        _logger.LogInformation("Finished: {took}", sw.Elapsed);
    }

    private int? EnsureCmsResourceDependency(KX13.Models.CmsResource? sourceResource, KxoContext kxoContext)
    {
        if (sourceResource != null)
        {
            _logger.LogTrace("Creating dependency for {cmsSettingsCategory} on {cmsResourceString} in target instance", nameof(KX13.Models.CmsSettingsCategory), nameof(KX13.Models.CmsResource));

            var targetResource = kxoContext.CmsResources.FirstOrDefault(r => r.ResourceName == sourceResource.ResourceName);

            var mapped = _resourceMapper.Map(sourceResource, targetResource);
            
            mapped.LogResult(_logger);
            
            switch (mapped)
            {
                case ModelMappingSuccess<KXO.Models.CmsResource>(var cmsResource, var newInstance):
                    ArgumentNullException.ThrowIfNull(cmsResource, nameof(cmsResource));
                    
                    if (newInstance)
                    {
                        kxoContext.CmsResources.Add(cmsResource);
                        _logger.LogInformation($"CmsSettingsKey: {cmsResource.ResourceName} was inserted.");
                    }
                    else
                    {
                        kxoContext.CmsResources.Update(cmsResource);
                        _logger.LogInformation($"CmsSettingsKey: {cmsResource.ResourceName} was updated.");
                    }

                    kxoContext.SaveChanges();

                    _pkMappingContext.SetMapping<KX13.Models.CmsResource>(r => r.ResourceId, sourceResource.ResourceId, cmsResource.ResourceId);
                    
                    return cmsResource.ResourceId;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mapped));
            }
        }
        
        return null;
    }
}