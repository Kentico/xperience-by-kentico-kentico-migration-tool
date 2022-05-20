using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Migration.Toolkit.Common;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Behaviors;
using Migration.Toolkit.Core.CmsResource;
using Migration.Toolkit.Core.CmsSettingsKey;
using Migration.Toolkit.Core.CmsSite;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.MigratePages;
using Migration.Toolkit.Core.MigratePageTypes;
using Migration.Toolkit.Core.MigrateSettingKeys;
using Migration.Toolkit.Core.MigrateUsers;
using Migration.Toolkit.Core.MigrateWebFarms;
using Migration.Toolkit.Core.MigrationProtocol;
using Migration.Toolkit.Core.Services;

namespace Migration.Toolkit.Core;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseToolkitCore(this IServiceCollection services)
    {
        // services.AddSingleton<IMigrationProtocol, DebugMigrationProtocol>();
        services.AddSingleton<IMigrationProtocol, NullMigrationProtocol>();
        services.AddScoped<IPrimaryKeyLocatorService, PrimaryKeyLocatorService>();
        
        // TODO tk: 2022-05-17 rem
        // services.AddTransient<IPkMappingService, PkMappingService>();
        services.AddScoped<PrimaryKeyMappingContext>();
        
        services.AddTransient<IDataEqualityComparer<KX13.Models.CmsSite, KXO.Models.CmsSite>, CmsSiteEqualityComparer>();
        services.AddTransient<IEntityMapper<Migration.Toolkit.KX13.Models.CmsSite, Migration.Toolkit.KXO.Models.CmsSite>, CmsSiteModelMapper>();

        // TODO tk: 2022-05-17 rem
        //services.AddTransient<ISynchronizer<Migration.Toolkit.KX13.Models.CmsSettingsKey, Migration.Toolkit.KXO.Models.CmsSettingsKey>, CmsSettingsKeySynchronizer>();
        
        services.AddTransient<IDataEqualityComparer<Migration.Toolkit.KX13.Models.CmsSettingsKey, Migration.Toolkit.KXO.Models.CmsSettingsKey>, CmsSettingsKeyComparer>();
        services.AddTransient<IEntityMapper<Migration.Toolkit.KX13.Models.CmsSettingsKey, Migration.Toolkit.KXO.Models.CmsSettingsKey>, CmsSettingsKeyMapper>();


        // page type synchronizer
        services.AddTransient<MigratePageTypesCommandHandler>();
        services.AddTransient<IEntityMapper<KX13.Models.CmsClass, KXO.Models.CmsClass>, CmsClassMapper>();
        // TODO tk: 2022-05-17 rem
        // services.AddTransient<ISynchronizer<Migration.Toolkit.KX13.Models.CmsClass, Migration.Toolkit.KXO.Models.CmsClass>, PageTypeSynchronizer>();
        
        // setting keys migrate command
        services.AddTransient<MigrateSettingKeysCommandHandler>();
        services.AddTransient<IEntityMapper<KX13.Models.CmsSettingsCategory, KXO.Models.CmsSettingsCategory>, CmsSettingsCategoryMapper>();
        
        // cms resource
        services.AddTransient<IEntityMapper<Migration.Toolkit.KX13.Models.CmsResource, Migration.Toolkit.KXO.Models.CmsResource>, CmsResourceMapper>();
        
        // cms user
        services.AddTransient<IEntityMapper<KX13.Models.CmsUser, KXO.Models.CmsUser>, CmsUserMapper>();
        services.AddTransient<IEntityMapper<KX13.Models.CmsRole, KXO.Models.CmsRole>, CmsRoleMapper>();
        services.AddTransient<MigrateUsersCommandHandler>();

        // cms web farm
        services.AddTransient<IEntityMapper<KX13.Models.CmsWebFarmServer, KXO.Models.CmsWebFarmServer>, CmsWebFarmMapper>();
        services.AddTransient<MigrateWebFarmsCommandHandler>();
        
        // pages
        services.AddTransient<MigratePagesCommand>();
        services.AddTransient<IEntityMapper<KX13.Models.CmsTree, KXO.Models.CmsTree>, CmsTreeMapper>();
        services.AddTransient<IEntityMapper<KX13.Models.CmsDocument, KXO.Models.CmsDocument>, CmsDocumentMapper>();
        services.AddTransient<IEntityMapper<KX13.Models.CmsAcl, KXO.Models.CmsAcl>, CmsAclMapper>();
        services.AddTransient<IEntityMapper<KX13.Models.CmsPageUrlPath, KXO.Models.CmsPageUrlPath>, CmsPageUrlPathMapper>();

        services.AddMediatR(typeof(DependencyInjectionExtensions));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestHandlingBehavior<,>));
        
        // IPipelineBehavior 
        return services;
    }
}