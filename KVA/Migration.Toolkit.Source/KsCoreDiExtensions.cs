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

        services.AddSingleton<ModelFacade>();
        services.AddSingleton<SpoiledGuidContext>();

        services.AddTransient<BulkDataCopyService>();
        services.AddTransient<CmsRelationshipService>();
        services.AddTransient<CoupledDataService>();
        services.AddScoped<AttachmentMigrator>();
        services.AddScoped<PageTemplateMigrator>();
        services.AddScoped<ClassService>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Source.KsCoreDiExtensions).Assembly));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestHandlingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CommandConstraintBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(XbKApiContextBehavior<,>));

        services.AddSingleton<SourceInstanceContext>();
        services.AddSingleton<DeferredPathService>();
        services.AddTransient<IpcService>();
        services.AddTransient<ReusableSchemaService>();

        services.AddScoped<PrimaryKeyMappingContext>();
        services.AddScoped<IPrimaryKeyLocatorService, PrimaryKeyLocatorService>();

        // commands
        services.AddTransient<MigratePagesCommandHandler>();
        services.AddTransient<MigrateCustomModulesCommandHandler>();
        services.AddTransient<MigrateCustomTablesHandler>();
        services.AddTransient<MigratePageTypesCommandHandler>();

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

        services.AddUniversalMigrationToolkit();

        return services;
    }
}