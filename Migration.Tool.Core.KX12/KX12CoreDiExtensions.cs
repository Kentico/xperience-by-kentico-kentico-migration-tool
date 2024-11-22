using CMS.ContactManagement;
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
using Migration.Tool.Core.KX12.Behaviors;
using Migration.Tool.Core.KX12.Contexts;
using Migration.Tool.Core.KX12.Helpers;
using Migration.Tool.Core.KX12.Mappers;
using Migration.Tool.Core.KX12.Services;

namespace Migration.Tool.Core.KX12;

public static class Kx12CoreDiExtensions
{
    public static IServiceCollection UseKx12ToolCore(this IServiceCollection services)
    {
        var printService = new PrintService();
        services.AddSingleton<IPrintService>(printService);
        HandbookReference.PrintService = printService;
        LogExtensions.PrintService = printService;

        services.AddTransient<BulkDataCopyService>();
        services.AddTransient<CoupledDataService>();
        services.AddScoped<CountryMigrator>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Kx12CoreDiExtensions).Assembly));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestHandlingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CommandConstraintBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(XbKApiContextBehavior<,>));

        services.AddScoped<PrimaryKeyMappingContext>();
        services.AddSingleton<KeyMappingContext>();
        services.AddScoped<IPrimaryKeyLocatorService, PrimaryKeyLocatorService>();
        services.AddSingleton<KeyLocatorService>();

        // mappers
        services.AddTransient<IEntityMapper<CmsAttachmentMapperSource, MediaFileInfo>, CmsAttachmentMapper>();
        services.AddTransient<IEntityMapper<KX12M.CmsConsent, ConsentInfo>, CmsConsentMapper>();
        services.AddTransient<IEntityMapper<KX12M.CmsConsentAgreement, ConsentAgreementInfo>, CmsConsentAgreementMapper>();
        services.AddTransient<IEntityMapper<KX12M.CmsConsentArchive, ConsentArchiveInfo>, CmsConsentArchiveMapper>();
        services.AddTransient<IEntityMapper<AlternativeFormMapperSource, AlternativeFormInfo>, AlternativeFormMapper>();
        services.AddTransient<IEntityMapper<KX12M.CmsRole, RoleInfo>, RoleInfoMapper>();
        services.AddTransient<IEntityMapper<KX12M.CmsSettingsKey, SettingsKeyInfo>, CmsSettingsKeyMapper>();
        services.AddTransient<IEntityMapper<KX12M.CmsUser, UserInfo>, UserInfoMapper>();
        services.AddTransient<IEntityMapper<MemberInfoMapperSource, MemberInfo>, MemberInfoMapper>();
        services.AddTransient<IEntityMapper<KX12M.CmsUserRole, UserRoleInfo>, UserRoleInfoMapper>();
        services.AddTransient<IEntityMapper<KX12M.OmContactGroup, ContactGroupInfo>, OmContactGroupMapper>();
        services.AddTransient<IEntityMapper<KX12M.OmContactStatus, ContactStatusInfo>, OmContactStatusMapper>();
        services.AddTransient<IEntityMapper<KX12M.CmsCountry, CountryInfo>, CountryInfoMapper>();
        services.AddTransient<IEntityMapper<KX12M.CmsState, StateInfo>, StateInfoMapper>();

        services.AddUniversalMigrationToolkit();

        return services;
    }
}
