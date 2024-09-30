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
using Migration.Toolkit.Common.Helpers;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Common.Services;
using Migration.Toolkit.Common.Services.BulkCopy;
using Migration.Toolkit.Common.Services.Ipc;
using Migration.Toolkit.KXP.Models;
using Migration.Toolkit.Source.Auxiliary;
using Migration.Toolkit.Source.Behaviors;
using Migration.Toolkit.Source.Contexts;
using Migration.Toolkit.Source.Helpers;
using Migration.Toolkit.Source.Mappers;
using Migration.Toolkit.Source.Model;
using Migration.Toolkit.Source.Services;

namespace Migration.Toolkit.Source;

public static class KsCoreDiExtensions
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;
    public static void InitServiceProvider(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

    public static IServiceCollection UseKsToolkitCore(this IServiceCollection services, bool? migrateMediaToMediaLibrary = false)
    {
        var printService = new PrintService();
        services.AddSingleton<IPrintService>(printService);
        HandbookReference.PrintService = printService;
        LogExtensions.PrintService = printService;

        services.AddTransient<IModuleLoader, ModuleLoader>();

        services.AddSingleton<ModelFacade>();
        services.AddSingleton<ISpoiledGuidContext, SpoiledGuidContext>();
        services.AddSingleton(s => s.GetRequiredService<ISpoiledGuidContext>() as SpoiledGuidContext ?? throw new InvalidOperationException());

        services.AddSingleton<ISourceGuidContext, SourceGuidContext>();
        services.AddSingleton<EntityIdentityFacade>();
        services.AddSingleton<IdentityLocator>();
        services.AddSingleton<IAssetFacade, AssetFacade>();
        services.AddSingleton<MediaLinkServiceFactory>();

        services.AddTransient<BulkDataCopyService>();
        services.AddTransient<CmsRelationshipService>();
        services.AddTransient<CoupledDataService>();
        if (migrateMediaToMediaLibrary ?? false)
        {
            services.AddScoped<IAttachmentMigrator, AttachmentMigratorToMediaLibrary>();
            services.AddScoped<IMediaFileMigrator, MediaFileMigrator>();
        }
        else
        {
            services.AddScoped<IAttachmentMigrator, AttachmentMigratorToContentItem>();
            services.AddScoped<IMediaFileMigrator, MediaFileMigratorToContentItem>();
        }
        services.AddScoped<PageTemplateMigrator>();
        services.AddScoped<ClassService>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(KsCoreDiExtensions).Assembly));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestHandlingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CommandConstraintBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(XbKApiContextBehavior<,>));

        services.AddSingleton<SourceInstanceContext>();
        services.AddSingleton<DeferredPathService>();
        services.AddTransient<IpcService>();
        services.AddTransient<ReusableSchemaService>();

        services.AddScoped<PrimaryKeyMappingContext>();
        services.AddScoped<IPrimaryKeyMappingContext, PrimaryKeyMappingContext>(s => s.GetRequiredService<PrimaryKeyMappingContext>());
        services.AddScoped<IPrimaryKeyLocatorService, PrimaryKeyLocatorService>();

        // umt mappers
        services.AddTransient<IUmtMapper<CmsTreeMapperSource>, ContentItemMapper>();
        services.AddTransient<IUmtMapper<TagModelSource>, TagMapper>();

        // mappers
        services.AddTransient<IEntityMapper<CmsAttachmentMapperSource, MediaFileInfo>, CmsAttachmentMapper>();
        services.AddTransient<IEntityMapper<ICmsClass, DataClassInfo>, CmsClassMapper>();
        services.AddTransient<IEntityMapper<ICmsForm, BizFormInfo>, CmsFormMapper>();
        services.AddTransient<IEntityMapper<ICmsForm, CmsForm>, CmsFormMapperEf>();
        services.AddTransient<IEntityMapper<ICmsResource, ResourceInfo>, ResourceMapper>();
        services.AddTransient<IEntityMapper<AlternativeFormMapperSource, AlternativeFormInfo>, AlternativeFormMapper>();
        services.AddTransient<IEntityMapper<MemberInfoMapperSource, MemberInfo>, MemberInfoMapper>();
        services.AddTransient<IEntityMapper<ICmsPageTemplateConfiguration, PageTemplateConfigurationInfo>, PageTemplateConfigurationMapper>();
        services.AddTransient<IEntityMapper<MediaLibraryInfoMapperSource, MediaLibraryInfo>, MediaLibraryInfoMapper>();
        services.AddTransient<IEntityMapper<MediaFileInfoMapperSource, MediaFileInfo>, MediaFileInfoMapper>();

        services.AddUniversalMigrationToolkit();

        return services;
    }
}
