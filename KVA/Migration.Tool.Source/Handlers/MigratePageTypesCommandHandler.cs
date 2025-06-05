using CMS.ContentEngine;
using CMS.DataEngine;
using Kentico.Xperience.UMT.Model;
using Kentico.Xperience.UMT.Services;
using MediatR;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.Builders;
using Migration.Tool.Common.Helpers;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.KXP.Api;
using Migration.Tool.Source.Contexts;
using Migration.Tool.Source.Helpers;
using Migration.Tool.Source.Model;
using Migration.Tool.Source.Providers;
using Migration.Tool.Source.Services;

namespace Migration.Tool.Source.Handlers;

public class MigratePageTypesCommandHandler(
    ILogger<MigratePageTypesCommandHandler> logger,
    IEntityMapper<ICmsClass, DataClassInfo> dataClassMapper,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    KxpClassFacade kxpClassFacade,
    IProtocol protocol,
    ToolConfiguration toolConfiguration,
    ModelFacade modelFacade,
    PageTemplateMigrator pageTemplateMigrator,
    ReusableSchemaService reusableSchemaService,
    ClassMappingProvider classMappingProvider,
    IImporter importer,
    IEnumerable<IReusableSchemaBuilder> reusableSchemaBuilders
    )
    : IRequestHandler<MigratePageTypesCommand, CommandResult>
{
    private const string CLASS_CMS_ROOT = "CMS.Root";

    public async Task<CommandResult> Handle(MigratePageTypesCommand request, CancellationToken cancellationToken)
    {
        var entityConfiguration = toolConfiguration.EntityConfigurations.GetEntityConfiguration<DataClassInfo>();

        using var ksClasses = EnumerableHelper.CreateDeferrableItemWrapper(
            modelFacade.Select<ICmsClass>("ClassIsDocumentType=1", "ClassID")
                .OrderBy(x => x.ClassID)
        );

        var manualMappings = classMappingProvider.ExecuteMappings();
        var manuallyMappedSourceClassIDs = new HashSet<int>();
        var manuallyMappedSourceClassNames = manualMappings.Values.SelectMany(x => x.mappping.SourceClassNames).ToHashSet();

        while (ksClasses.GetNext(out var di))
        {
            var (_, ksClass) = di;

            if (manuallyMappedSourceClassNames.Contains(ksClass.ClassName))
            {
                manuallyMappedSourceClassIDs.Add(ksClass.ClassID);
            }

            if (manualMappings.ContainsKey(ksClass.ClassName))
            {
                continue;
            }

            if (entityConfiguration.ExcludeCodeNames.Contains(ksClass.ClassName,
                    StringComparer.InvariantCultureIgnoreCase))
            {
                continue;
            }

            if (ksClass.ClassInheritsFromClassID is { } classInheritsFromClassId && !(primaryKeyMappingContext.HasMapping<ICmsClass>(c => c.ClassID, classInheritsFromClassId) || manuallyMappedSourceClassIDs.Contains(classInheritsFromClassId)))
            {
                // defer migration to later stage
                if (ksClasses.TryDeferItem(di))
                {
                    logger.LogTrace("Class {Class} inheritance parent not found, deferring migration to end. Attempt {Attempt}", Printer.GetEntityIdentityPrint(ksClass), di.Recurrence);
                }
                else
                {
                    logger.LogErrorMissingDependency(ksClass, nameof(ksClass.ClassInheritsFromClassID), ksClass.ClassInheritsFromClassID, typeof(DataClassInfo));
                    protocol.Append(HandbookReferences
                        .MissingRequiredDependency<DataClassInfo>(nameof(DataClassInfo.ClassID), classInheritsFromClassId)
                        .NeedsManualAction()
                    );
                }

                continue;
            }

            protocol.FetchedSource(ksClass);

            if (entityConfiguration.ExcludeCodeNames.Contains(ksClass.ClassName, StringComparer.InvariantCultureIgnoreCase))
            {
                protocol.Warning(HandbookReferences.EntityExplicitlyExcludedByCodeName(ksClass.ClassName, "PageType"), ksClass);
                logger.LogInformation("CmsClass: {ClassName} was skipped => it is explicitly excluded in configuration", ksClass.ClassName);
                continue;
            }

            if (string.Equals(ksClass.ClassName, CLASS_CMS_ROOT, StringComparison.InvariantCultureIgnoreCase))
            {
                protocol.Warning(HandbookReferences.CmsClassCmsRootClassTypeSkip, ksClass);
                logger.LogInformation("CmsClass: {ClassName} was skipped => CMS.Root cannot be migrated", ksClass.ClassName);
                continue;
            }

            if (string.Equals(ksClass.ClassName, "cms.site", StringComparison.InvariantCultureIgnoreCase))
            {
                continue;
            }

            if (ksClass.ClassName.Equals("cms.folder", StringComparison.InvariantCultureIgnoreCase))
            {
                if (!toolConfiguration.UseDeprecatedFolderPageType.GetValueOrDefault(false))
                {
                    logger.LogInformation("Class {Class} is deprecated, skipping", Printer.GetEntityIdentityPrint(ksClass));
                    continue;
                }

                logger.LogWarning("Class {Class} is deprecated, but migration is enabled with configuration flag 'UseDeprecatedFolderPageType'", Printer.GetEntityIdentityPrint(ksClass));
            }

            var kxoDataClass = kxpClassFacade.GetClass(ksClass.ClassGUID);
            protocol.FetchedTarget(kxoDataClass);

            if (SaveUsingKxoApi(ksClass, kxoDataClass) is { } targetClass)
            {
                if (targetClass.ClassContentTypeType is ClassContentTypeType.WEBSITE)
                {
                    foreach (var cmsClassSite in modelFacade.SelectWhere<ICmsClassSite>("ClassID = @classId", new SqlParameter("classId", ksClass.ClassID)))
                    {
                        if (modelFacade.SelectById<ICmsSite>(cmsClassSite.SiteID) is { SiteGUID: var siteGuid })
                        {
                            if (ChannelInfoProvider.ProviderObject.Get(siteGuid) is { ChannelID: var channelId })
                            {
                                var info = new ContentTypeChannelInfo { ContentTypeChannelChannelID = channelId, ContentTypeChannelContentTypeID = targetClass.ClassID };
                                ContentTypeChannelInfoProvider.ProviderObject.Set(info);
                            }
                            else
                            {
                                logger.LogWarning("Channel for site with SiteGUID '{SiteGuid}' not found", siteGuid);
                            }
                        }
                        else
                        {
                            logger.LogWarning("Source site with SiteID '{SiteId}' not found", cmsClassSite.SiteID);
                        }
                    }
                }
            }
        }

        await MigratePageTemplateConfigurations();

        // By default all restrictions are bypassed.
        // To migrate the restrictions, include --type-restrictions in your migrate command,
        // which will migrate the actual restrictions in the next stage
        await BypassAllowedChildClasses();

        return new GenericCommandResult();
    }

    private async Task BypassAllowedChildClasses()
    {
        var websiteContentTypes = kxpClassFacade.GetClasses(ClassContentTypeType.WEBSITE);
        foreach (var parent in websiteContentTypes)
        {
            logger.LogInformation("Preliminarily allowing any child content type for parent type {ParentTypeName} ({ParentTypeGuid})", parent.ClassName, parent.ClassGUID);
            foreach (var child in websiteContentTypes)
            {
                var model = new AllowedChildContentTypeModel
                {
                    AllowedChildContentTypeParentGuid = parent.ClassGUID,
                    AllowedChildContentTypeChildGuid = child.ClassGUID
                };

                var importResult = await importer.ImportAsync(model);
                if (importResult is { Success: false })
                {
                    logger.LogError("Failed to define allowed child content type '{ChildClassGuid}' of parent type '{ParentClassGuid}'", model.AllowedChildContentTypeChildGuid, model.AllowedChildContentTypeParentGuid);
                }
            }
        }
    }

    private async Task MigratePageTemplateConfigurations()
    {
        if (modelFacade.IsAvailable<ICmsPageTemplateConfiguration>())
        {
            foreach (var ksPageTemplateConfiguration in modelFacade.SelectAll<ICmsPageTemplateConfiguration>())
            {
                await pageTemplateMigrator.MigratePageTemplateConfigurationAsync(ksPageTemplateConfiguration);
            }
        }
    }

    private DataClassInfo? SaveUsingKxoApi(ICmsClass ksClass, DataClassInfo kxoDataClass)
    {
        var mapped = dataClassMapper.Map(ksClass, kxoDataClass);
        protocol.MappedTarget(mapped);

        try
        {
            if (mapped is { Success: true })
            {
                (var dataClassInfo, bool newInstance) = mapped;
                ArgumentNullException.ThrowIfNull(dataClassInfo, nameof(dataClassInfo));

                if (reusableSchemaService.IsConversionToReusableFieldSchemaRequested(dataClassInfo.ClassName))
                {
                    dataClassInfo = reusableSchemaService.ConvertToReusableSchema(dataClassInfo);
                }

                kxpClassFacade.SetClass(dataClassInfo);

                protocol.Success(ksClass, dataClassInfo, mapped);
                logger.LogEntitySetAction(newInstance, dataClassInfo);

                primaryKeyMappingContext.SetMapping<DataClassInfo>(
                    r => r.ClassID,
                    ksClass.ClassID,
                    dataClassInfo.ClassID
                );

                return dataClassInfo;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while saving page type {ClassName}", ksClass.ClassName);
        }

        return null;
    }
}
