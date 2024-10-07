using CMS.ContentEngine;
using CMS.DataEngine;
using CMS.FormEngine;
using MediatR;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.Builders;
using Migration.Toolkit.Common.Helpers;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.KXP.Api;
using Migration.Toolkit.KXP.Api.Services.CmsClass;
using Migration.Toolkit.KXP.Models;
using Migration.Toolkit.Source.Contexts;
using Migration.Toolkit.Source.Helpers;
using Migration.Toolkit.Source.Mappers;
using Migration.Toolkit.Source.Model;
using Migration.Toolkit.Source.Services;

namespace Migration.Toolkit.Source.Handlers;

public class MigratePageTypesCommandHandler(
    ILogger<MigratePageTypesCommandHandler> logger,
    IEntityMapper<ICmsClass, DataClassInfo> dataClassMapper,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    KxpClassFacade kxpClassFacade,
    IProtocol protocol,
    ToolkitConfiguration toolkitConfiguration,
    ModelFacade modelFacade,
    PageTemplateMigrator pageTemplateMigrator,
    ReusableSchemaService reusableSchemaService,
    IEnumerable<IClassMapping> classMappings,
    IFieldMigrationService fieldMigrationService,
    IEnumerable<IReusableSchemaBuilder> reusableSchemaBuilders
    )
    : IRequestHandler<MigratePageTypesCommand, CommandResult>
{
    private const string CLASS_CMS_ROOT = "CMS.Root";

    public async Task<CommandResult> Handle(MigratePageTypesCommand request, CancellationToken cancellationToken)
    {
        var entityConfiguration = toolkitConfiguration.EntityConfigurations.GetEntityConfiguration<CmsClass>();

        using var ksClasses = EnumerableHelper.CreateDeferrableItemWrapper(
            modelFacade.Select<ICmsClass>("ClassIsDocumentType=1", "ClassID")
                .OrderBy(x => x.ClassID)
        );

        ExecReusableSchemaBuilders();

        var manualMappings = new Dictionary<string, DataClassInfo>();
        foreach (var classMapping in classMappings)
        {
            var newDt = DataClassInfoProvider.GetDataClassInfo(classMapping.TargetClassName) ?? DataClassInfo.New();
            classMapping.PatchTargetDataClass(newDt);

            // might not need ClassGUID
            // newDt.ClassGUID = GuidHelper.CreateDataClassGuid($"{newDt.ClassName}|{newDt.ClassTableName}");

            var cmsClasses = new List<ICmsClass>();
            foreach (string sourceClassName in classMapping.SourceClassNames)
            {
                cmsClasses.AddRange(modelFacade.SelectWhere<ICmsClass>("ClassName=@className", new SqlParameter("className", sourceClassName)));
            }

            var nfi = string.IsNullOrWhiteSpace(newDt.ClassFormDefinition) ? new FormInfo() : new FormInfo(newDt.ClassFormDefinition);
            bool hasPrimaryKey = false;
            foreach (var formFieldInfo in nfi.GetFields(true, true, true, true, false))
            {
                if (formFieldInfo.PrimaryKey)
                {
                    hasPrimaryKey = true;
                }
            }

            if (!hasPrimaryKey)
            {
                if (string.IsNullOrWhiteSpace(classMapping.PrimaryKey))
                {
                    throw new InvalidOperationException($"Class mapping has no primary key set");
                }
                else
                {
                    var prototype = FormHelper.GetBasicFormDefinition(classMapping.PrimaryKey);
                    nfi.AddFormItem(prototype.GetFormField(classMapping.PrimaryKey));
                }
            }

            newDt.ClassFormDefinition = nfi.GetXmlDefinition();

            foreach (string schemaName in classMapping.ReusableSchemaNames)
            {
                reusableSchemaService.AddReusableSchemaToDataClass(newDt, schemaName);
            }

            nfi = new FormInfo(newDt.ClassFormDefinition);

            var fieldInReusableSchemas = reusableSchemaService.GetFieldsFromReusableSchema(newDt).ToDictionary(x => x.Name, x => x);

            bool hasFieldsAlready = true;
            foreach (var cmml in classMapping.Mappings.Where(m => m.IsTemplate).ToLookup(x => x.SourceFieldName))
            {
                var cmm = cmml.FirstOrDefault() ?? throw new InvalidOperationException();
                if (fieldInReusableSchemas.ContainsKey(cmm.TargetFieldName))
                {
                    // part of reusable schema
                    continue;
                }

                var sc = cmsClasses.FirstOrDefault(sc => sc.ClassName.Equals(cmm.SourceClassName, StringComparison.InvariantCultureIgnoreCase))
                         ?? throw new NullReferenceException($"The source class '{cmm.SourceClassName}' does not exist - wrong mapping {classMapping}");

                var fi = new FormInfo(sc.ClassFormDefinition);
                if (nfi.GetFormField(cmm.TargetFieldName) is { })
                {
                }
                else
                {
                    var src = fi.GetFormField(cmm.SourceFieldName);
                    src.Name = cmm.TargetFieldName;
                    nfi.AddFormItem(src);
                    hasFieldsAlready = false;
                }
            }

            if (!hasFieldsAlready)
            {
                FormDefinitionHelper.MapFormDefinitionFields(logger, fieldMigrationService, nfi.GetXmlDefinition(), false, true, newDt, false, false);
                CmsClassMapper.PatchDataClassInfo(newDt, out _, out _);
            }

            if (classMapping.TargetFieldPatchers.Count > 0)
            {
                nfi = new FormInfo(newDt.ClassFormDefinition);
                foreach (string fieldName in classMapping.TargetFieldPatchers.Keys)
                {
                    classMapping.TargetFieldPatchers[fieldName].Invoke(nfi.GetFormField(fieldName));
                }

                newDt.ClassFormDefinition = nfi.GetXmlDefinition();
            }

            DataClassInfoProvider.SetDataClassInfo(newDt);
            foreach (var gByClass in classMapping.Mappings.GroupBy(x => x.SourceClassName))
            {
                manualMappings.TryAdd(gByClass.Key, newDt);
            }

            foreach (string sourceClassName in classMapping.SourceClassNames)
            {
                var sourceClass = cmsClasses.First(c => c.ClassName.Equals(sourceClassName, StringComparison.InvariantCultureIgnoreCase));
                foreach (var cmsClassSite in modelFacade.SelectWhere<ICmsClassSite>("ClassId = @classId", new SqlParameter("classId", sourceClass.ClassID)))
                {
                    if (modelFacade.SelectById<ICmsSite>(cmsClassSite.SiteID) is { SiteGUID: var siteGuid })
                    {
                        if (ChannelInfoProvider.ProviderObject.Get(siteGuid) is { ChannelID: var channelId })
                        {
                            var info = new ContentTypeChannelInfo { ContentTypeChannelChannelID = channelId, ContentTypeChannelContentTypeID = newDt.ClassID };
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

        while (ksClasses.GetNext(out var di))
        {
            var (_, ksClass) = di;

            if (manualMappings.ContainsKey(ksClass.ClassName))
            {
                continue;
            }

            if (ksClass.ClassInheritsFromClassID is { } classInheritsFromClassId && !primaryKeyMappingContext.HasMapping<ICmsClass>(c => c.ClassID, classInheritsFromClassId))
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
                        .MissingRequiredDependency<CmsClass>(nameof(CmsClass.ClassId), classInheritsFromClassId)
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
                if (!toolkitConfiguration.UseDeprecatedFolderPageType.GetValueOrDefault(false))
                {
                    logger.LogInformation("Class {Class} is deprecated, skipping", Printer.GetEntityIdentityPrint(ksClass));
                    continue;
                }

                logger.LogWarning("Class {Class} is deprecated, but migration is enabled with configuration flag 'UseDeprecatedFolderPageType'", Printer.GetEntityIdentityPrint(ksClass));
            }

            var kxoDataClass = kxpClassFacade.GetClass(ksClass.ClassGUID);
            protocol.FetchedTarget(kxoDataClass);

            if (SaveUsingKxoApi(ksClass, kxoDataClass) is { } targetClassId)
            {
                foreach (var cmsClassSite in modelFacade.SelectWhere<ICmsClassSite>("ClassID = @classId", new SqlParameter("classId", ksClass.ClassID)))
                {
                    if (modelFacade.SelectById<ICmsSite>(cmsClassSite.SiteID) is { SiteGUID: var siteGuid })
                    {
                        if (ChannelInfoProvider.ProviderObject.Get(siteGuid) is { ChannelID: var channelId })
                        {
                            var info = new ContentTypeChannelInfo { ContentTypeChannelChannelID = channelId, ContentTypeChannelContentTypeID = targetClassId };
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

        await MigratePageTemplateConfigurations();

        return new GenericCommandResult();
    }

    private void ExecReusableSchemaBuilders()
    {
        foreach (var reusableSchemaBuilder in reusableSchemaBuilders)
        {
            reusableSchemaBuilder.AssertIsValid();
            var fieldInfos = reusableSchemaBuilder.FieldBuilders.Select(fb =>
            {
                switch (fb)
                {
                    case { Factory: { } factory }:
                    {
                        return factory();
                    }
                    case { SourceFieldIdentifier: { } fieldIdentifier }:
                    {
                        var sourceClass = modelFacade.SelectWhere<ICmsClass>("ClassName=@className", new SqlParameter("className", fieldIdentifier.ClassName)).SingleOrDefault()
                                          ?? throw new InvalidOperationException($"Invalid reusable schema field builder for field '{fieldIdentifier.ClassName}': DataClass not found, class name '{fieldIdentifier.ClassName}'");

                        if (string.IsNullOrWhiteSpace(sourceClass.ClassFormDefinition))
                        {
                            throw new InvalidOperationException($"Invalid reusable schema field builder for field '{fieldIdentifier.ClassName}': Class '{fieldIdentifier.ClassName}' is missing field '{fieldIdentifier.FieldName}'");
                        }

                        // this might be cached as optimization
                        var patcher = new FormDefinitionPatcher(
                            logger,
                            sourceClass.ClassFormDefinition,
                            fieldMigrationService,
                            sourceClass.ClassIsForm.GetValueOrDefault(false),
                            sourceClass.ClassIsDocumentType,
                            true,
                            false
                        );

                        patcher.PatchFields();
                        patcher.RemoveCategories();

                        var fi = new FormInfo(patcher.GetPatched());
                        return fi.GetFormField(fieldIdentifier.FieldName) switch
                        {
                            { } field => field,
                            _ => throw new InvalidOperationException($"Invalid reusable schema field builder for field '{fieldIdentifier.ClassName}': Class '{fieldIdentifier.ClassName}' is missing field '{fieldIdentifier.FieldName}'")
                        };
                    }
                    default:
                    {
                        throw new InvalidOperationException($"Invalid reusable schema field builder for field '{fb.TargetFieldName}'");
                    }
                }
            });

            reusableSchemaService.EnsureReusableFieldSchema(reusableSchemaBuilder.SchemaName, reusableSchemaBuilder.SchemaDisplayName, reusableSchemaBuilder.SchemaDescription, fieldInfos.ToArray());
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

    private int? SaveUsingKxoApi(ICmsClass ksClass, DataClassInfo kxoDataClass)
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

                primaryKeyMappingContext.SetMapping<CmsClass>(
                    r => r.ClassId,
                    ksClass.ClassID,
                    dataClassInfo.ClassID
                );

                return dataClassInfo.ClassID;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while saving page type {ClassName}", ksClass.ClassName);
        }

        return null;
    }
}
