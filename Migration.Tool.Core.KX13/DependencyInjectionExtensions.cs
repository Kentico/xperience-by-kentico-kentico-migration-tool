using CMS.Commerce;
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
using Migration.Tool.Core.KX13.Behaviors;
using Migration.Tool.Core.KX13.Contexts;
using Migration.Tool.Core.KX13.Helpers;
using Migration.Tool.Core.KX13.Mappers;
using Migration.Tool.Core.KX13.Services;

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

        services.AddHttpContextAccessor();
        // mappers
#pragma warning disable CS0618 // Type or member is obsolete
        services.AddTransient<IEntityMapper<CmsAttachmentMapperSource, MediaFileInfo>, CmsAttachmentMapper>();
#pragma warning restore CS0618 // Type or member is obsolete
        services.AddTransient<IEntityMapper<KX13M.CmsConsent, ConsentInfo>, CmsConsentMapper>();
        services.AddTransient<IEntityMapper<KX13M.CmsConsentAgreement, ConsentAgreementInfo>, CmsConsentAgreementMapper>();
        services.AddTransient<IEntityMapper<KX13M.CmsConsentArchive, ConsentArchiveInfo>, CmsConsentArchiveMapper>();
        services.AddTransient<IEntityMapper<AlternativeFormMapperSource, AlternativeFormInfo>, AlternativeFormMapper>();
        services.AddTransient<IEntityMapper<KX13M.CmsRole, RoleInfo>, RoleInfoMapper>();
        services.AddTransient<IEntityMapper<KX13M.CmsSettingsKey, SettingsKeyInfo>, CmsSettingsKeyMapper>();
        services.AddTransient<IEntityMapper<KX13M.CmsUser, UserInfo>, UserInfoMapper>();
        services.AddTransient<IEntityMapper<MemberInfoMapperSource, MemberInfo>, MemberInfoMapper>();
        services.AddTransient<IEntityMapper<KX13M.CmsUserRole, UserRoleInfo>, UserRoleInfoMapper>();
        services.AddTransient<IEntityMapper<KX13M.OmContactGroup, ContactGroupInfo>, OmContactGroupMapper>();
        services.AddTransient<IEntityMapper<KX13M.OmContactStatus, ContactStatusInfo>, OmContactStatusMapper>();
        services.AddTransient<IEntityMapper<KX13M.CmsCountry, CountryInfo>, CountryInfoMapper>();
        services.AddTransient<IEntityMapper<KX13M.CmsState, StateInfo>, StateInfoMapper>();
        services.AddTransient<IEntityMapper<CustomerInfoMapperSource, CustomerInfo>, CustomerInfoMapper>();
        services.AddTransient<IEntityMapper<CustomerAddressInfoMapperSource, CustomerAddressInfo>, CustomerAddressInfoMapper>();
        services.AddTransient<IEntityMapper<OrderInfoMapperSource, OrderInfo>, OrderInfoMapper>();
        services.AddTransient<IEntityMapper<OrderItemInfoMapperSource, OrderItemInfo>, OrderItemInfoMapper>();
        services.AddTransient<IEntityMapper<OrderAddressInfoMapperSource, OrderAddressInfo>, OrderAddressInfoMapper>();

        services.AddUniversalMigrationToolkit();

        return services;
    }
}
