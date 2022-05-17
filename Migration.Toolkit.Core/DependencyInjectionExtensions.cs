using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Migration.Toolkit.Common;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.CmsResource;
using Migration.Toolkit.Core.CmsSettingsKey;
using Migration.Toolkit.Core.CmsSite;
using Migration.Toolkit.Core.Commands;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.MigratePageTypes;
using Migration.Toolkit.Core.MigrateSettingKeys;
using Migration.Toolkit.Core.Services;

namespace Migration.Toolkit.Core;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseToolkitCore(this IServiceCollection services)
    {
        // TODO tk: 2022-05-17 rem
        // services.AddTransient<IPkMappingService, PkMappingService>();
        services.AddScoped<PkMappingContext>();
        
        services.AddTransient<IDataEqualityComparer<KX13.Models.CmsSite, KXO.Models.CmsSite>, CmsSiteEqualityComparer>();
        services.AddTransient<IEntityMapper<Migration.Toolkit.KX13.Models.CmsSite, Migration.Toolkit.KXO.Models.CmsSite>, CmsSiteModelMapper>();

        // TODO tk: 2022-05-17 rem
        //services.AddTransient<ISynchronizer<Migration.Toolkit.KX13.Models.CmsSettingsKey, Migration.Toolkit.KXO.Models.CmsSettingsKey>, CmsSettingsKeySynchronizer>();
        
        services.AddTransient<IDataEqualityComparer<Migration.Toolkit.KX13.Models.CmsSettingsKey, Migration.Toolkit.KXO.Models.CmsSettingsKey>, CmsSettingsKeyComparer>();
        services.AddTransient<IEntityMapper<Migration.Toolkit.KX13.Models.CmsSettingsKey, Migration.Toolkit.KXO.Models.CmsSettingsKey>, CmsSettingsKeyMapper>();


        // page type synchronizer
        services.AddTransient<MigratePageTypesCommandHandler>();
        services.AddTransient<IEntityMapper<Migration.Toolkit.KX13.Models.CmsClass, Migration.Toolkit.KXO.Models.CmsClass>, CmsClassMapper>();
        // TODO tk: 2022-05-17 rem
        // services.AddTransient<ISynchronizer<Migration.Toolkit.KX13.Models.CmsClass, Migration.Toolkit.KXO.Models.CmsClass>, PageTypeSynchronizer>();
        
        // setting keys migrate command
        services.AddTransient<MigrateSettingKeysCommandHandler>();
        services.AddTransient<IEntityMapper<Migration.Toolkit.KX13.Models.CmsSettingsCategory, Migration.Toolkit.KXO.Models.CmsSettingsCategory>, CmsSettingsCategoryMapper>();
        
        // cms resource
        services.AddTransient<IEntityMapper<Migration.Toolkit.KX13.Models.CmsResource, Migration.Toolkit.KXO.Models.CmsResource>, CmsResourceMapper>();

        services.AddMediatR(typeof(DependencyInjectionExtensions));
            
        return services;
    }
}