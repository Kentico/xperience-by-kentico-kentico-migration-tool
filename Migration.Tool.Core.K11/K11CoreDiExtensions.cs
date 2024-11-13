using CMS.DataEngine;
using CMS.DataProtection;
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
using Migration.Tool.Core.K11.Behaviors;
using Migration.Tool.Core.K11.Contexts;
using Migration.Tool.Core.K11.Helpers;
using Migration.Tool.Core.K11.Mappers;
using Migration.Tool.Core.K11.Services;
using Migration.Tool.K11.Models;

namespace Migration.Tool.Core.K11;

public static class K11CoreDiExtensions
{
    public static IServiceCollection UseK11ToolCore(this IServiceCollection services)
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

        services.AddScoped<PrimaryKeyMappingContext>();
        services.AddSingleton<KeyMappingContext>();
        services.AddScoped<IPrimaryKeyLocatorService, PrimaryKeyLocatorService>();
        services.AddSingleton<KeyLocatorService>();

        // mappers
        services.AddTransient<IEntityMapper<CmsAttachmentMapperSource, MediaFileInfo>, CmsAttachmentMapper>();
        services.AddTransient<IEntityMapper<CmsConsent, ConsentInfo>, CmsConsentMapper>();
        services.AddTransient<IEntityMapper<CmsConsentAgreement, ConsentAgreementInfo>, CmsConsentAgreementMapper>();
        services.AddTransient<IEntityMapper<CmsConsentArchive, ConsentArchiveInfo>, CmsConsentArchiveMapper>();
        services.AddTransient<IEntityMapper<AlternativeFormMapperSource, AlternativeFormInfo>, AlternativeFormMapper>();
        services.AddTransient<IEntityMapper<CmsRole, RoleInfo>, RoleInfoMapper>();
        services.AddTransient<IEntityMapper<CmsSettingsCategory, KXP.Models.CmsSettingsCategory>, CmsSettingsCategoryMapper>();
        services.AddTransient<IEntityMapper<CmsSettingsKey, SettingsKeyInfo>, CmsSettingsKeyMapper>();
        services.AddTransient<IEntityMapper<CmsUser, UserInfo>, UserInfoMapper>();
        services.AddTransient<IEntityMapper<MemberInfoMapperSource, MemberInfo>, MemberInfoMapper>();
        services.AddTransient<IEntityMapper<CmsUserRole, UserRoleInfo>, UserRoleInfoMapper>();
        services.AddTransient<IEntityMapper<OmContactGroup, KXP.Models.OmContactGroup>, OmContactGroupMapper>();
        services.AddTransient<IEntityMapper<OmContactStatus, KXP.Models.OmContactStatus>, OmContactStatusMapper>();
        services.AddTransient<IEntityMapper<CmsCountry, CountryInfo>, CountryInfoMapper>();
        services.AddTransient<IEntityMapper<CmsState, StateInfo>, StateInfoMapper>();

        services.AddUniversalMigrationToolkit();

        return services;
    }
}
