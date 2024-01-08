namespace Migration.Toolkit.Core.KX12;

using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Globalization;
using CMS.MediaLibrary;
using CMS.Membership;
using CMS.Modules;
using CMS.OnlineForms;
using CMS.Websites;
using Kentico.Xperience.UMT;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Common.Services;
using Migration.Toolkit.Common.Services.BulkCopy;
using Migration.Toolkit.Common.Services.Ipc;
using Migration.Toolkit.Core.KX12.Behaviors;
using Migration.Toolkit.Core.KX12.Contexts;
using Migration.Toolkit.Core.KX12.Handlers;
using Migration.Toolkit.Core.KX12.Helpers;
using Migration.Toolkit.Core.KX12.Mappers;
using Migration.Toolkit.Core.KX12.Services;
using Migration.Toolkit.Core.KX12.Services.CmsClass;
using Migration.Toolkit.Core.KX12.Services.CmsRelationship;
using Migration.Toolkit.KXP.Models;

public static class Kx12CoreDiExtensions
{
    public static IServiceCollection UseKx12ToolkitCore(this IServiceCollection services)
    {
        var printService = new PrintService();
        services.AddSingleton<IPrintService>(printService);
        HandbookReference.PrintService = printService;
        LogExtensions.PrintService = printService;

        services.AddTransient<IModuleLoader, ModuleLoader>();
        services.AddSingleton<ICommandParser, CommandParser>();

        services.AddSingleton<IProtocol, Protocol>();

        services.AddSingleton<IMigrationProtocol, TextMigrationProtocol>();
        services.AddSingleton<IMigrationProtocol, DebugMigrationProtocol>();
        services.AddSingleton<FieldMigrationService>();

        services.AddTransient<BulkDataCopyService>();
        services.AddTransient<CmsRelationshipService>();
        services.AddTransient<CoupledDataService>();
        services.AddScoped<AttachmentMigrator>();
        services.AddScoped<PageTemplateMigrator>();
        services.AddScoped<CountryMigrator>();
        services.AddScoped<ClassService>();

        services.AddMediatR(typeof(Kx12CoreDiExtensions));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestHandlingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CommandConstraintBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(XbKApiContextBehavior<,>));


        services.AddSingleton(s => new TableReflectionService(s.GetRequiredService<ILogger<TableReflectionService>>()));
        services.AddSingleton<SourceInstanceContext>();
        services.AddTransient<IpcService>();

        services.AddScoped<PrimaryKeyMappingContext>();
        services.AddSingleton<KeyMappingContext>();
        services.AddScoped<IPrimaryKeyLocatorService, PrimaryKeyLocatorService>();
        services.AddSingleton<KeyLocatorService>();

        // commands
        // services.AddTransient<MigrateAttachmentsCommandHandler>();
        // services.AddTransient<MigrateContactGroupsCommand>();
        // services.AddTransient<MigrateContactManagementCommandHandler>();
        services.AddTransient<MigrateDataProtectionCommandHandler>();
        services.AddTransient<MigrateFormsCommandHandler>();
        services.AddTransient<MigratePagesCommandHandler>();
        //services.AddTransient<MigratePageTypesCommandHandler>();
        services.AddTransient<MigratePagesCommand>();
        services.AddTransient<MigrateSettingKeysCommandHandler>();
        // services.AddTransient<MigrateSitesCommandHandler>();
        services.AddTransient<MigrateUsersCommandHandler>();
        services.AddTransient<MigrateMembersCommandHandler>();
        services.AddTransient<MigrateContactManagementCommandHandler>();


        // umt mappers
        services.AddTransient<IUmtMapper<CmsTreeMapperSource>, ContentItemMapper>();

        // mappers
        services.AddTransient<IEntityMapper<CmsAttachmentMapperSource, MediaFileInfo>, CmsAttachmentMapper>();
        services.AddTransient<IEntityMapper<KX12M.CmsClass, DataClassInfo>, CmsClassMapper>();
        services.AddTransient<IEntityMapper<KX12M.CmsConsent, CmsConsent>, CmsConsentMapper>();
        services.AddTransient<IEntityMapper<KX12M.CmsConsentAgreement, CmsConsentAgreement>, CmsConsentAgreementMapper>();
        services.AddTransient<IEntityMapper<KX12M.CmsConsentArchive, CmsConsentArchive>, CmsConsentArchiveMapper>();
        services.AddTransient<IEntityMapper<KX12M.CmsForm, BizFormInfo>, CmsFormMapper>();
        services.AddTransient<IEntityMapper<KX12M.CmsForm, CmsForm>, CmsFormMapperEf>();
        services.AddTransient<IEntityMapper<KX12M.CmsResource, ResourceInfo>, ResourceMapper>();
        services.AddTransient<IEntityMapper<AlternativeFormMapperSource, AlternativeFormInfo>, AlternativeFormMapper>();
        services.AddTransient<IEntityMapper<KX12M.CmsRole, RoleInfo>, RoleInfoMapper>();
        services.AddTransient<IEntityMapper<KX12M.CmsSettingsCategory, CmsSettingsCategory>, CmsSettingsCategoryMapper>();
        services.AddTransient<IEntityMapper<KX12M.CmsSettingsKey, SettingsKeyInfo>, CmsSettingsKeyMapper>();
        services.AddTransient<IEntityMapper<KX12M.CmsUser, UserInfo>, UserInfoMapper>();
        services.AddTransient<IEntityMapper<MemberInfoMapperSource, MemberInfo>, MemberInfoMapper>();
        services.AddTransient<IEntityMapper<KX12M.CmsUserRole, UserRoleInfo>, UserRoleInfoMapper>();
        services.AddTransient<IEntityMapper<KX12M.MediaLibrary, MediaLibraryInfo>, MediaLibraryInfoMapper>();
        services.AddTransient<IEntityMapper<KX12M.OmContact, OmContact>, OmContactMapper>();
        services.AddTransient<IEntityMapper<KX12M.OmContactGroup, OmContactGroup>, OmContactGroupMapper>();
        services.AddTransient<IEntityMapper<KX12M.OmContactStatus, OmContactStatus>, OmContactStatusMapper>();
        services.AddTransient<IEntityMapper<MediaFileInfoMapperSource, MediaFileInfo>, MediaFileInfoMapper>();
        services.AddTransient<IEntityMapper<KX12M.CmsCountry, CountryInfo>, CountryInfoMapper>();
        services.AddTransient<IEntityMapper<KX12M.CmsState, StateInfo>, StateInfoMapper>();
        services.AddTransient<IEntityMapper<KX12M.CmsPageTemplateConfiguration, PageTemplateConfigurationInfo>, PageTemplateConfigurationMapper>();

        services.AddUniversalMigrationToolkit();

        return services;
    }
}