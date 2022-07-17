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

using Migration.Toolkit.KXP.Models;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseToolkitCore(this IServiceCollection services)
    {
        services.AddSingleton<IMigrationProtocol, TextMigrationProtocol>();
        // services.AddSingleton<IMigrationProtocol, NullMigrationProtocol>();
        services.AddScoped<IPrimaryKeyLocatorService, PrimaryKeyLocatorService>();
        services.AddScoped<AttachmentMigrator>();
        services.AddTransient<BulkDataCopyService>();
        services.AddTransient<CoupledDataService>();
        services.AddTransient<CmsRelationshipService>();
        services.AddScoped<ClassService>();
        
        services.AddMediatR(typeof(DependencyInjectionExtensions));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestHandlingBehavior<,>));

        services.AddSingleton(s => new TableReflectionService(s.GetRequiredService<ILogger<TableReflectionService>>()));

        services.AddScoped<PrimaryKeyMappingContext>();

        services.AddTransient<IEntityMapper<Migration.Toolkit.KX13.Models.CmsSettingsKey, CmsSettingsKey>, CmsSettingsKeyMapper>();

        // forms
        services.AddTransient<MigrateFormsCommandHandler>();
        services.AddTransient<IEntityMapper<KX13.Models.CmsForm, BizFormInfo>, CmsFormMapper>();

        // page type synchronizer
        services.AddTransient<MigratePageTypesCommandHandler>();
        services.AddTransient<IEntityMapper<KX13.Models.CmsClass, DataClassInfo>, CmsClassMapper>();

        // setting keys migrate command
        services.AddTransient<MigrateSettingKeysCommandHandler>();
        services.AddTransient<IEntityMapper<KX13.Models.CmsSettingsCategory, CmsSettingsCategory>, CmsSettingsCategoryMapper>();

        // cms resource
        services.AddTransient<IEntityMapper<Migration.Toolkit.KX13.Models.CmsResource, CmsResource>, CmsResourceMapper>();

        // cms user
        services.AddTransient<IEntityMapper<KX13.Models.CmsUser, CmsUser>, CmsUserMapper>();
        services.AddTransient<IEntityMapper<KX13.Models.CmsRole, CmsRole>, CmsRoleMapper>();
        services.AddTransient<MigrateUsersCommandHandler>();

        // pages
        services.AddTransient<MigratePagesCommand>();
        services.AddTransient<IEntityMapper<CmsTreeMapperSource, TreeNode>, TreeNodeMapper>();
        services.AddTransient<IEntityMapper<KX13.Models.CmsPageUrlPath, CmsPageUrlPath>, CmsPageUrlPathMapper>();

        // media libraries
        services.AddTransient<IEntityMapper<MediaFileInfoMapperSource, MediaFileInfo>, MediaFileInfoMapper>();
        services.AddTransient<IEntityMapper<KX13.Models.MediaLibrary, MediaLibraryInfo>, MediaLibraryInfoMapper>();

        // sites
        services.AddTransient<IEntityMapper<KX13.Models.CmsSite, CmsSite>, CmsSiteMapper>();
        services.AddTransient<IRequestHandler<MigrateSitesCommand, CommandResult>, MigrateSitesCommandHandler>();

        // cms data protection
        services.AddTransient<IEntityMapper<KX13.Models.CmsConsent, CmsConsent>, CmsConsentMapper>();
        services.AddTransient<IEntityMapper<KX13.Models.CmsConsentArchive, CmsConsentArchive>, CmsConsentArchiveMapper>();
        services.AddTransient<IEntityMapper<KX13.Models.CmsConsentAgreement, CmsConsentAgreement>, CmsConsentAgreementMapper>();
        services.AddTransient<MigrateDataProtectionCommandHandler>();

        // contact groups
        services.AddTransient<IEntityMapper<KX13.Models.OmContactGroup, OmContactGroup>, OmContactGroupMapper>();
        services.AddTransient<MigrateContactGroupsCommand>();

        // contacts
        services.AddTransient<IEntityMapper<KX13.Models.OmContact, OmContact>, OmContactMapper>();
        services.AddTransient<IEntityMapper<KX13.Models.OmContactStatus, OmContactStatus>, OmContactStatusMapper>();
        services.AddTransient<MigrateContactManagementCommandHandler>();

        // attachments
        services.AddTransient<IEntityMapper<CmsAttachmentMapperSource, MediaFileInfo>, CmsAttachmentMapper>();
        services.AddTransient<MigrateAttachmentsCommandHandler>();

        // IPipelineBehavior 
        return services;
    }
}