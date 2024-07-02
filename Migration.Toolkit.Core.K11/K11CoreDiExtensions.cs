namespace Migration.Toolkit.Core.K11;

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
using Migration.Toolkit.Core.K11.Behaviors;
using Migration.Toolkit.Core.K11.Contexts;
using Migration.Toolkit.Core.K11.Handlers;
using Migration.Toolkit.Core.K11.Helpers;
using Migration.Toolkit.Core.K11.Mappers;
using Migration.Toolkit.Core.K11.Services;
using Migration.Toolkit.KXP.Models;

public static class K11CoreDiExtensions
{
    public static IServiceCollection UseK11ToolkitCore(this IServiceCollection services)
    {
        var printService = new PrintService();
        services.AddSingleton<IPrintService>(printService);
        HandbookReference.PrintService = printService;
        LogExtensions.PrintService = printService;

        services.AddTransient<BulkDataCopyService>();
        services.AddTransient<CoupledDataService>();
        services.AddScoped<CountryMigrator>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(K11CoreDiExtensions).Assembly));
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
        services.AddTransient<MigrateDataProtectionCommandHandler>();
        services.AddTransient<MigratePagesCommand>();
        services.AddTransient<MigrateSettingKeysCommandHandler>();
        services.AddTransient<MigrateUsersCommandHandler>();
        services.AddTransient<MigrateMembersCommandHandler>();
        services.AddTransient<MigrateContactManagementCommandHandler>();

        // mappers
        services.AddTransient<IEntityMapper<CmsAttachmentMapperSource, MediaFileInfo>, CmsAttachmentMapper>();
        services.AddTransient<IEntityMapper<Toolkit.K11.Models.CmsConsent, CmsConsent>, CmsConsentMapper>();
        services.AddTransient<IEntityMapper<Toolkit.K11.Models.CmsConsentAgreement, CmsConsentAgreement>, CmsConsentAgreementMapper>();
        services.AddTransient<IEntityMapper<Toolkit.K11.Models.CmsConsentArchive, CmsConsentArchive>, CmsConsentArchiveMapper>();
        services.AddTransient<IEntityMapper<Toolkit.K11.Models.CmsResource, ResourceInfo>, ResourceMapper>();
        services.AddTransient<IEntityMapper<AlternativeFormMapperSource, AlternativeFormInfo>, AlternativeFormMapper>();
        services.AddTransient<IEntityMapper<Toolkit.K11.Models.CmsRole, RoleInfo>, RoleInfoMapper>();
        services.AddTransient<IEntityMapper<Toolkit.K11.Models.CmsSettingsCategory, CmsSettingsCategory>, CmsSettingsCategoryMapper>();
        services.AddTransient<IEntityMapper<Toolkit.K11.Models.CmsSettingsKey, SettingsKeyInfo>, CmsSettingsKeyMapper>();
        services.AddTransient<IEntityMapper<Toolkit.K11.Models.CmsUser, UserInfo>, UserInfoMapper>();
        services.AddTransient<IEntityMapper<MemberInfoMapperSource, MemberInfo>, MemberInfoMapper>();
        services.AddTransient<IEntityMapper<Toolkit.K11.Models.CmsUserRole, UserRoleInfo>, UserRoleInfoMapper>();
        services.AddTransient<IEntityMapper<Toolkit.K11.Models.MediaLibrary, MediaLibraryInfo>, MediaLibraryInfoMapper>();
        services.AddTransient<IEntityMapper<Toolkit.K11.Models.OmContact, OmContact>, OmContactMapper>();
        services.AddTransient<IEntityMapper<Toolkit.K11.Models.OmContactGroup, OmContactGroup>, OmContactGroupMapper>();
        services.AddTransient<IEntityMapper<Toolkit.K11.Models.OmContactStatus, OmContactStatus>, OmContactStatusMapper>();
        services.AddTransient<IEntityMapper<MediaFileInfoMapperSource, MediaFileInfo>, MediaFileInfoMapper>();
        services.AddTransient<IEntityMapper<Toolkit.K11.Models.CmsCountry, CountryInfo>, CountryInfoMapper>();
        services.AddTransient<IEntityMapper<Toolkit.K11.Models.CmsState, StateInfo>, StateInfoMapper>();

        services.AddUniversalMigrationToolkit();

        return services;
    }
}