using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Globalization;
using CMS.MediaLibrary;
using CMS.Membership;
using Kentico.Xperience.UMT;

using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Common.Services;
using Migration.Tool.Common.Services.BulkCopy;
using Migration.Tool.Core.KX13.Behaviors;
using Migration.Tool.Core.KX13.Contexts;
using Migration.Tool.Core.KX13.Helpers;
using Migration.Tool.Core.KX13.Mappers;
using Migration.Tool.Core.KX13.Services;
using Migration.Tool.KXP.Models;

namespace Migration.Tool.Core.KX13;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseKx13ToolCore(this IServiceCollection services)
    {
        var printService = new PrintService();
        services.AddSingleton<IPrintService>(printService);
        HandbookReference.PrintService = printService;
        LogExtensions.PrintService = printService;

        services.AddTransient<BulkDataCopyService>();
        services.AddTransient<CoupledDataService>();
        services.AddScoped<CountryMigrator>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjectionExtensions).Assembly));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestHandlingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CommandConstraintBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(XbKApiContextBehavior<,>));

        services.AddScoped<PrimaryKeyMappingContext>();
        services.AddSingleton<KeyMappingContext>();
        services.AddScoped<IPrimaryKeyLocatorService, PrimaryKeyLocatorService>();
        services.AddSingleton<KeyLocatorService>();

        // mappers
        services.AddTransient<IEntityMapper<CmsAttachmentMapperSource, MediaFileInfo>, CmsAttachmentMapper>();
        services.AddTransient<IEntityMapper<KX13M.CmsConsent, CmsConsent>, CmsConsentMapper>();
        services.AddTransient<IEntityMapper<KX13M.CmsConsentAgreement, CmsConsentAgreement>, CmsConsentAgreementMapper>();
        services.AddTransient<IEntityMapper<KX13M.CmsConsentArchive, CmsConsentArchive>, CmsConsentArchiveMapper>();
        services.AddTransient<IEntityMapper<AlternativeFormMapperSource, AlternativeFormInfo>, AlternativeFormMapper>();
        services.AddTransient<IEntityMapper<KX13M.CmsRole, RoleInfo>, RoleInfoMapper>();
        services.AddTransient<IEntityMapper<KX13M.CmsSettingsCategory, CmsSettingsCategory>, CmsSettingsCategoryMapper>();
        services.AddTransient<IEntityMapper<KX13M.CmsSettingsKey, SettingsKeyInfo>, CmsSettingsKeyMapper>();
        services.AddTransient<IEntityMapper<KX13M.CmsUser, UserInfo>, UserInfoMapper>();
        services.AddTransient<IEntityMapper<MemberInfoMapperSource, MemberInfo>, MemberInfoMapper>();
        services.AddTransient<IEntityMapper<KX13M.CmsUserRole, UserRoleInfo>, UserRoleInfoMapper>();
        services.AddTransient<IEntityMapper<KX13M.OmContactGroup, OmContactGroup>, OmContactGroupMapper>();
        services.AddTransient<IEntityMapper<KX13M.OmContactStatus, OmContactStatus>, OmContactStatusMapper>();
        services.AddTransient<IEntityMapper<KX13M.CmsCountry, CountryInfo>, CountryInfoMapper>();
        services.AddTransient<IEntityMapper<KX13M.CmsState, StateInfo>, StateInfoMapper>();

        services.AddUniversalMigrationToolkit();

        return services;
    }
}
