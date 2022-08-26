using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.MediaLibrary;
using CMS.OnlineForms;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Behaviors;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.Handlers;
using Migration.Toolkit.Core.Mappers;
using Migration.Toolkit.Core.MigrationProtocol;
using Migration.Toolkit.Core.Services;
using Migration.Toolkit.Core.Services.BulkCopy;
using Migration.Toolkit.Core.Services.CmsClass;
using Migration.Toolkit.Core.Services.CmsRelationship;

namespace Migration.Toolkit.Core;

using CMS.Globalization;
using Migration.Toolkit.KXP.Models;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseToolkitCore(this IServiceCollection services)
    {
        services.AddSingleton<IProtocol, Protocol>();
        services.AddSingleton<IMigrationProtocol, TextMigrationProtocol>();
        services.AddSingleton<IMigrationProtocol, DebugMigrationProtocol>();
        
        services.AddTransient<BulkDataCopyService>();
        services.AddTransient<CmsRelationshipService>();
        services.AddTransient<CoupledDataService>();
        services.AddScoped<AttachmentMigrator>();
        services.AddScoped<CountryMigrator>();
        services.AddScoped<ClassService>();
        
        services.AddMediatR(typeof(DependencyInjectionExtensions));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestHandlingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CommandConstraintBehavior<,>));
        

        services.AddSingleton(s => new TableReflectionService(s.GetRequiredService<ILogger<TableReflectionService>>()));

        services.AddScoped<PrimaryKeyMappingContext>();
        services.AddScoped<IPrimaryKeyLocatorService, PrimaryKeyLocatorService>();

        // commands
        services.AddTransient<MigrateAttachmentsCommandHandler>();
        // services.AddTransient<MigrateContactGroupsCommand>();
        services.AddTransient<MigrateContactManagementCommandHandler>();
        services.AddTransient<MigrateDataProtectionCommandHandler>();
        services.AddTransient<MigrateFormsCommandHandler>();
        services.AddTransient<MigratePageTypesCommandHandler>();
        services.AddTransient<MigratePagesCommand>();
        services.AddTransient<MigrateSettingKeysCommandHandler>();
        services.AddTransient<MigrateSitesCommandHandler>();
        services.AddTransient<MigrateUsersCommandHandler>();
        
        // mappers
        services.AddTransient<IEntityMapper<CmsAttachmentMapperSource, MediaFileInfo>, CmsAttachmentMapper>();
        services.AddTransient<IEntityMapper<CmsTreeMapperSource, TreeNode>, TreeNodeMapper>();
        services.AddTransient<IEntityMapper<KX13M.CmsClass, DataClassInfo>, CmsClassMapper>();
        services.AddTransient<IEntityMapper<KX13M.CmsConsent, CmsConsent>, CmsConsentMapper>();
        services.AddTransient<IEntityMapper<KX13M.CmsConsentAgreement, CmsConsentAgreement>, CmsConsentAgreementMapper>();
        services.AddTransient<IEntityMapper<KX13M.CmsConsentArchive, CmsConsentArchive>, CmsConsentArchiveMapper>();
        services.AddTransient<IEntityMapper<KX13M.CmsForm, BizFormInfo>, CmsFormMapper>();
        services.AddTransient<IEntityMapper<KX13M.CmsForm, CmsForm>, CmsFormMapperEf>();
        services.AddTransient<IEntityMapper<KX13M.CmsPageUrlPath, CmsPageUrlPath>, CmsPageUrlPathMapper>();
        services.AddTransient<IEntityMapper<KX13M.CmsResource, CmsResource>, CmsResourceMapper>();
        services.AddTransient<IEntityMapper<KX13M.CmsRole, CmsRole>, CmsRoleMapper>();
        services.AddTransient<IEntityMapper<KX13M.CmsSettingsCategory, CmsSettingsCategory>, CmsSettingsCategoryMapper>();
        services.AddTransient<IEntityMapper<KX13M.CmsSettingsKey, CmsSettingsKey>, CmsSettingsKeyMapper>();
        services.AddTransient<IEntityMapper<KX13M.CmsSite, CmsSite>, CmsSiteMapper>();
        services.AddTransient<IEntityMapper<KX13M.CmsUser, CmsUser>, CmsUserMapper>();
        services.AddTransient<IEntityMapper<KX13M.MediaLibrary, MediaLibraryInfo>, MediaLibraryInfoMapper>();
        services.AddTransient<IEntityMapper<KX13M.OmContact, OmContact>, OmContactMapper>();
        services.AddTransient<IEntityMapper<KX13M.OmContactGroup, OmContactGroup>, OmContactGroupMapper>();
        services.AddTransient<IEntityMapper<KX13M.OmContactStatus, OmContactStatus>, OmContactStatusMapper>();
        services.AddTransient<IEntityMapper<MediaFileInfoMapperSource, MediaFileInfo>, MediaFileInfoMapper>();
        services.AddTransient<IEntityMapper<KX13M.CmsCountry, CountryInfo>, CountryInfoMapper>();
        services.AddTransient<IEntityMapper<KX13M.CmsState, StateInfo>, StateInfoMapper>();

        return services;
    }
}