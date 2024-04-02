namespace Migration.Toolkit.Source;

using CMS.DataEngine;
using CMS.FormEngine;
using CMS.MediaLibrary;
using CMS.Membership;
using CMS.Modules;
using CMS.OnlineForms;
using CMS.Websites;
using Kentico.Xperience.UMT;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Common.Services;
using Migration.Toolkit.Common.Services.BulkCopy;
using Migration.Toolkit.Common.Services.Ipc;
using Migration.Toolkit.KXP.Models;
using Migration.Toolkit.Source.Behaviors;
using Migration.Toolkit.Source.Contexts;
using Migration.Toolkit.Source.Handlers;
using Migration.Toolkit.Source.Helpers;
using Migration.Toolkit.Source.Mappers;
using Migration.Toolkit.Source.Model;
using Migration.Toolkit.Source.Services;

public static class KsCoreDiExtensions
{
    public static IServiceCollection UseKsToolkitCore(this IServiceCollection services)
    {
        var printService = new PrintService();
        services.AddSingleton<IPrintService>(printService);
        HandbookReference.PrintService = printService;
        LogExtensions.PrintService = printService;

        services.AddTransient<IModuleLoader, ModuleLoader>();
        // services.AddSingleton<ICommandParser, CommandParser>();

        services.AddSingleton<ModelFacade>();

        // services.AddSingleton<FieldMigrationService>();

        services.AddTransient<BulkDataCopyService>();
        services.AddTransient<CmsRelationshipService>();
        services.AddTransient<CoupledDataService>();
        services.AddScoped<AttachmentMigrator>();
        services.AddScoped<PageTemplateMigrator>();
        // services.AddScoped<CountryMigrator>();
        services.AddScoped<ClassService>();

        services.AddMediatR(typeof(KsCoreDiExtensions));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestHandlingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CommandConstraintBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(XbKApiContextBehavior<,>));

        // services.AddSingleton(s => new TableReflectionService(s.GetRequiredService<ILogger<TableReflectionService>>()));
        services.AddSingleton<SourceInstanceContext>();
        services.AddSingleton<DeferredPathService>();
        services.AddTransient<IpcService>();
        services.AddTransient<ReusableSchemaService>();

        services.AddScoped<PrimaryKeyMappingContext>();
        // services.AddSingleton<KeyMappingContext>();
        services.AddScoped<IPrimaryKeyLocatorService, PrimaryKeyLocatorService>();
        // services.AddSingleton<KeyLocatorService>();

        // commands
        // services.AddTransient<MigrateDataProtectionCommandHandler>();
        // services.AddTransient<MigrateFormsCommandHandler>();
        services.AddTransient<MigratePagesCommandHandler>();
        services.AddTransient<MigrateCustomModulesCommandHandler>();
        // services.AddTransient<MigrateSettingKeysCommandHandler>();
        // services.AddTransient<MigrateUsersCommandHandler>();
        // services.AddTransient<MigrateMembersCommandHandler>();
        // services.AddTransient<MigrateContactManagementCommandHandler>();
        services.AddTransient<MigrateCustomTablesHandler>();
        services.AddTransient<MigratePageTypesCommandHandler>();

        // services.AddTransient<MigrateCustomTablesHandler>();

        // umt mappers
        services.AddTransient<IUmtMapper<CmsTreeMapperSource>, ContentItemMapper>();

        // mappers
        services.AddTransient<IEntityMapper<CmsAttachmentMapperSource, MediaFileInfo>, CmsAttachmentMapper>();
        services.AddTransient<IEntityMapper<ICmsClass, DataClassInfo>, CmsClassMapper>();
        // services.AddTransient<IEntityMapper<Toolkit.K11.Models.CmsConsent, CmsConsent>, CmsConsentMapper>();
        // services.AddTransient<IEntityMapper<Toolkit.K11.Models.CmsConsentAgreement, CmsConsentAgreement>, CmsConsentAgreementMapper>();
        // services.AddTransient<IEntityMapper<Toolkit.K11.Models.CmsConsentArchive, CmsConsentArchive>, CmsConsentArchiveMapper>();
        services.AddTransient<IEntityMapper<ICmsForm, BizFormInfo>, CmsFormMapper>();
        services.AddTransient<IEntityMapper<ICmsForm, CmsForm>, CmsFormMapperEf>();
        services.AddTransient<IEntityMapper<ICmsResource, ResourceInfo>, ResourceMapper>();
        services.AddTransient<IEntityMapper<AlternativeFormMapperSource, AlternativeFormInfo>, AlternativeFormMapper>();
        // services.AddTransient<IEntityMapper<Toolkit.K11.Models.CmsRole, RoleInfo>, RoleInfoMapper>();
        // services.AddTransient<IEntityMapper<Toolkit.K11.Models.CmsSettingsCategory, CmsSettingsCategory>, CmsSettingsCategoryMapper>();
        // services.AddTransient<IEntityMapper<Toolkit.K11.Models.CmsSettingsKey, SettingsKeyInfo>, CmsSettingsKeyMapper>();
        // services.AddTransient<IEntityMapper<Toolkit.K11.Models.CmsUser, UserInfo>, UserInfoMapper>();
        services.AddTransient<IEntityMapper<MemberInfoMapperSource, MemberInfo>, MemberInfoMapper>();
        // services.AddTransient<IEntityMapper<Toolkit.K11.Models.CmsUserRole, UserRoleInfo>, UserRoleInfoMapper>();
        // services.AddTransient<IEntityMapper<Toolkit.K11.Models.MediaLibrary, MediaLibraryInfo>, MediaLibraryInfoMapper>();
        // services.AddTransient<IEntityMapper<Toolkit.K11.Models.OmContact, OmContact>, OmContactMapper>();
        // services.AddTransient<IEntityMapper<Toolkit.K11.Models.OmContactGroup, OmContactGroup>, OmContactGroupMapper>();
        // services.AddTransient<IEntityMapper<Toolkit.K11.Models.OmContactStatus, OmContactStatus>, OmContactStatusMapper>();
        // services.AddTransient<IEntityMapper<MediaFileInfoMapperSource, MediaFileInfo>, MediaFileInfoMapper>();
        // services.AddTransient<IEntityMapper<Toolkit.K11.Models.CmsCountry, CountryInfo>, CountryInfoMapper>();
        // services.AddTransient<IEntityMapper<Toolkit.K11.Models.CmsState, StateInfo>, StateInfoMapper>();
        services.AddTransient<IEntityMapper<ICmsPageTemplateConfiguration, PageTemplateConfigurationInfo>, PageTemplateConfigurationMapper>();

        services.AddUniversalMigrationToolkit();

        return services;
    }
}