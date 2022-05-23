using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Migration.Toolkit.Common;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.CmsResource;
using Migration.Toolkit.Core.CmsSettingsKey;
using Migration.Toolkit.Core.CmsSite;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.MigrateDataProtection;
using Migration.Toolkit.Core.MigratePageTypes;
using Migration.Toolkit.Core.MigrateSettingKeys;
using Migration.Toolkit.Core.MigrateUsers;
using Migration.Toolkit.Core.MigrateWebFarms;
using Migration.Toolkit.Core.MigrationProtocol;

namespace Migration.Toolkit.Core;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseToolkitCore(this IServiceCollection services)
    {
        services.AddSingleton<IMigrationProtocol, NullMigrationProtocol>();
        
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
        services.AddTransient<IEntityMapper<Migration.Toolkit.KX13.Models.CmsClass, Migration.Toolkit.KXO.Models.CmsClass>, CmsClassMapper>();
        // TODO tk: 2022-05-17 rem
        // services.AddTransient<ISynchronizer<Migration.Toolkit.KX13.Models.CmsClass, Migration.Toolkit.KXO.Models.CmsClass>, PageTypeSynchronizer>();
        
        // setting keys migrate command
        services.AddTransient<MigrateSettingKeysCommandHandler>();
        services.AddTransient<IEntityMapper<Migration.Toolkit.KX13.Models.CmsSettingsCategory, Migration.Toolkit.KXO.Models.CmsSettingsCategory>, CmsSettingsCategoryMapper>();
        
        // cms resource
        services.AddTransient<IEntityMapper<Migration.Toolkit.KX13.Models.CmsResource, Migration.Toolkit.KXO.Models.CmsResource>, CmsResourceMapper>();
        
        // cms user
        services.AddTransient<IEntityMapper<KX13.Models.CmsUser, KXO.Models.CmsUser>, CmsUserMapper>();
        services.AddTransient<MigrateUsersCommandHandler>();

        // cms web farm
        services.AddTransient<IEntityMapper<KX13.Models.CmsWebFarmServer, KXO.Models.CmsWebFarmServer>, CmsWebFarmMapper>();
        services.AddTransient<MigrateWebFarmsCommandHandler>();

        // cms data protection
        services.AddTransient<IEntityMapper<KX13.Models.CmsConsent, KXO.Models.CmsConsent>, CmsConsentMapper>();
        services.AddTransient<IEntityMapper<KX13.Models.CmsConsentArchive, KXO.Models.CmsConsentArchive>, CmsConsentArchiveMapper>();
        services.AddTransient<IEntityMapper<KX13.Models.CmsConsentAgreement, KXO.Models.CmsConsentAgreement>, CmsConsentAgreementMapper>();
        services.AddTransient<MigrateDataProtectionCommandHandler>();


        services.AddMediatR(typeof(DependencyInjectionExtensions));
            
        return services;
    }
}