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

using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Common.Services;
using Migration.Tool.Common.Services.BulkCopy;
using Migration.Tool.Common.Services.Ipc;
using Migration.Tool.KXP.Models;
using Migration.Tool.Source.Auxiliary;
using Migration.Tool.Source.Behaviors;
using Migration.Tool.Source.Contexts;
using Migration.Tool.Source.Helpers;
using Migration.Tool.Source.Mappers;
using Migration.Tool.Source.Model;
using Migration.Tool.Source.Providers;
using Migration.Tool.Source.Services;

namespace Migration.Tool.Source;

public static class KsCoreDiExtensions
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;
    public static void InitServiceProvider(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

    public static IServiceCollection UseKsToolCore(this IServiceCollection services, bool? migrateMediaToMediaLibrary = false)
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
        services.AddSingleton<ClassMappingProvider>();
        services.AddTransient<PageBuilderPatcher>();

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
