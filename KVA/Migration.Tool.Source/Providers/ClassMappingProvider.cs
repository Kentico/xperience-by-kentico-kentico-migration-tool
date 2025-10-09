using CMS.ContentEngine;
using CMS.DataEngine;
using CMS.FormEngine;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.Builders;
using Migration.Tool.Common.Enumerations;
using Migration.Tool.KXP.Api;
using Migration.Tool.KXP.Api.Services.CmsClass;
using Migration.Tool.Source.Contexts;
using Migration.Tool.Source.Helpers;
using Migration.Tool.Source.Model;
using Migration.Tool.Source.Services;
using static Migration.Tool.Source.Mappers.CmsClassMapper;

namespace Migration.Tool.Source.Providers;

public class ClassMappingProvider(
    ILogger<ClassMappingProvider> logger,
    IEnumerable<IClassMapping> classMappings,
    ModelFacade modelFacade,
    ReusableSchemaService reusableSchemaService,
    IFieldMigrationService fieldMigrationService,
    ToolConfiguration configuration,
    IEnumerable<IReusableSchemaBuilder> reusableSchemaBuilders,
    KxpClassFacade kxpClassFacade,
    IEntityMapper<ICmsClass, DataClassInfo> dataClassMapper)
{
    private readonly List<IClassMapping> configuredClassMappings = [];
    private bool settingsInitialized = false;

    private Dictionary<string, IClassMapping> MappingsByClassName
    {
        get
        {
            var codedMappings = classMappings.Aggregate(new Dictionary<string, IClassMapping>(StringComparer.InvariantCultureIgnoreCase),
                (current, sourceClassMapping) =>
                {
                    foreach (string s2Cl in sourceClassMapping.SourceClassNames)
                    {
                        if (!current.TryAdd(s2Cl, sourceClassMapping))
                        {
                            throw new InvalidOperationException($"Incorrectly defined class mapping - duplicate found for class '{s2Cl}'. Fix mapping before proceeding with migration.");
                        }
                    }

                    return current;
                });

            foreach (var classMapping in configuredClassMappings)
            {
                foreach (string classMappingSourceClassName in classMapping.SourceClassNames)
                {
                    if (!codedMappings.TryAdd(classMappingSourceClassName, classMapping))
                    {
                        throw new InvalidOperationException($"Duplicate class mapping '{classMapping.TargetClassName}'. (check configuration 'ConvertClassesToContentHub')");
                    }
                }
            }

            return codedMappings;
        }
    }

    public IClassMapping? GetMapping(string className)
    {
        EnsureSettings();
        return MappingsByClassName.GetValueOrDefault(className);
    }

    private Dictionary<string, (DataClassInfo target, IClassMapping mappping)>? manualMappings = null;
    public Dictionary<string, (DataClassInfo target, IClassMapping mappping)> ExecuteMappings()
    {
        if (manualMappings is not null)
        {
            return manualMappings;
        }

        EnsureSettings();
        ExecReusableSchemaBuilders();

        manualMappings = new Dictionary<string, (DataClassInfo target, IClassMapping mappping)>();
        var metadataFields = GetLegacyMetadataFields(modelFacade.SelectVersion(), IncludedMetadata.Extended).ToArray();

        foreach (var classMapping in MappingsByClassName.Values)
        {
            var newDt = DataClassInfoProvider.GetDataClassInfo(classMapping.TargetClassName) ?? DataClassInfo.New();
            classMapping.PatchTargetDataClass(newDt);

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

            var reusableSchemas = new Dictionary<Guid, string>();
            foreach (string schemaName in classMapping.ReusableSchemaNames)
            {
                Guid schemaGuid = reusableSchemaService.AddReusableSchemaToDataClass(newDt, schemaName);
                reusableSchemas[schemaGuid] = schemaName;
            }

            nfi = new FormInfo(newDt.ClassFormDefinition);

            var fieldInReusableSchemas = reusableSchemaService.GetFieldsFromReusableSchema(newDt).ToDictionary(x => x.Name, x => x);

            foreach (var cmml in classMapping.Mappings.Where(m => m.IsTemplate).ToLookup(x => x.SourceFieldName))
            {
                foreach (var cmm in cmml)
                {
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
                        FormFieldInfo src;
                        if (fi.GetFormField(cmm.SourceFieldName) is { } ffi)
                        {
                            src = ffi;
                            src.Name = cmm.TargetFieldName;
                        }
                        else
                        {
                            var legacyMetadataFieldMapping =
                                metadataFields.FirstOrDefault(x => x.LegacyFieldName == cmm.SourceFieldName) ?? throw new Exception(
                                    $"Invalid mapping for target class {classMapping.TargetClassName}. Field {cmm.SourceFieldName} not found in source class {cmm.SourceClassName}");
                            src = GetLegacyMetadataFormFieldInfo(legacyMetadataFieldMapping,
                                    cmm.TargetFieldName, classMapping.TargetClassName);
                        }

                        nfi.AddFormItem(src);
                    }
                }
            }

            var includedMetadata = cmsClasses.Any(x => x.ClassResourceID.HasValue) ? IncludedMetadata.None : (configuration.IncludeExtendedMetadata.GetValueOrDefault(false) ? IncludedMetadata.Extended : IncludedMetadata.Basic);
            FormDefinitionHelper.MapFormDefinitionFields(logger, fieldMigrationService, nfi.GetXmlDefinition(), false, true, newDt, false, false, new FormInfo(newDt.ClassFormDefinition).GetFormElements(true, true).Select(x => GetFormElementName(x)));
            PatchDataClassInfo(newDt, [], modelFacade.SelectVersion(), reusableSchemas, includedMetadata, out _, out _);

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
                manualMappings.TryAdd(gByClass.Key, (newDt, classMapping));
            }

            foreach (string sourceClassName in classMapping.SourceClassNames)
            {
                if (newDt.ClassContentTypeType is ClassContentTypeType.REUSABLE)
                {
                    continue;
                }

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

        return manualMappings;
    }

    private string GetFormElementName(IDataDefinitionItem item)
    {
        if (item is FormFieldInfo ffi)
        {
            return ffi.Name;
        }
        else if (item is FormSchemaInfo fsi)
        {
            return fsi.Name;
        }
        else
        {
            throw new NotImplementedException("Internal error 06bd8437-b1e5-4016-853f-8a577288e06e. Report this issue.");
        }
    }

    private void ExecReusableSchemaBuilders()
    {
        foreach (var reusableSchemaBuilder in reusableSchemaBuilders)
        {
            reusableSchemaBuilder.AssertIsValid();

            if (reusableSchemaBuilder.SourceClassName is not null)
            {
                var ksClass = modelFacade.SelectWhere<ICmsClass>("ClassName = @className", new SqlParameter("className", reusableSchemaBuilder.SourceClassName)).FirstOrDefault();
                if (ksClass is null)
                {
                    logger.LogError("Source class {ClassName} required for reusable field schema {SchemaName} not found",
                        reusableSchemaBuilder.SourceClassName, reusableSchemaBuilder.SchemaName);
                    continue;
                }
                var kxoDataClass = kxpClassFacade.GetClass(ksClass.ClassGUID);

                var mapped = dataClassMapper.Map(ksClass, kxoDataClass);

                try
                {
                    if (mapped is { Success: true })
                    {
                        var (dataClassInfo, _) = mapped;
                        reusableSchemaService.ConvertToReusableSchema(dataClassInfo!, reusableSchemaBuilder.SchemaName,
                            reusableSchemaBuilder.SchemaDisplayName, reusableSchemaBuilder.SchemaDescription,
                            reusableSchemaBuilder.FieldNameTransformation);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error while saving page type {ClassName}", ksClass.ClassName);
                }
            }
            else
            {
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
                            var sourceClass = modelFacade.SelectWhere<ICmsClass>("ClassName=@className",
                                                      new SqlParameter("className", fieldIdentifier.ClassName))
                                                  .SingleOrDefault()
                                              ?? throw new InvalidOperationException(
                                                  $"Invalid reusable schema field builder for field '{fieldIdentifier.ClassName}': DataClass not found, class name '{fieldIdentifier.ClassName}'");

                            if (string.IsNullOrWhiteSpace(sourceClass.ClassFormDefinition))
                            {
                                throw new InvalidOperationException(
                                    $"Invalid reusable schema field builder for field '{fieldIdentifier.ClassName}': Class '{fieldIdentifier.ClassName}' is missing field '{fieldIdentifier.FieldName}'");
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
                                _ => throw new InvalidOperationException(
                                    $"Invalid reusable schema field builder for field '{fieldIdentifier.ClassName}': Class '{fieldIdentifier.ClassName}' is missing field '{fieldIdentifier.FieldName}'")
                            };
                        }
                        default:
                        {
                            throw new InvalidOperationException(
                                $"Invalid reusable schema field builder for field '{fb.TargetFieldName}'");
                        }
                    }
                });

                reusableSchemaService.EnsureReusableFieldSchema(reusableSchemaBuilder.SchemaName,
                    reusableSchemaBuilder.SchemaDisplayName, reusableSchemaBuilder.SchemaDescription,
                    fieldInfos.ToArray());
            }
        }
    }

    private void EnsureSettings()
    {
        if (!settingsInitialized)
        {
            settingsInitialized = true;
            var customTableClasses = modelFacade.Select<ICmsClass>("ClassIsCustomTable=1", "ClassID ASC").ToList();

            foreach (string classNameForConversion in configuration.ClassNamesConvertToContentHub)
            {
                if (customTableClasses.FirstOrDefault(c => c.ClassName.Equals(classNameForConversion, StringComparison.InvariantCultureIgnoreCase)) is { } mappedClass)
                {
                    var m = new MultiClassMapping(classNameForConversion, target =>
                    {
                        target.ClassName = classNameForConversion;
                        target.ClassTableName = mappedClass.ClassTableName;
                        target.ClassDisplayName = mappedClass.ClassDisplayName;
                        target.ClassType = ClassType.CONTENT_TYPE;
                        target.ClassContentTypeType = ClassContentTypeType.REUSABLE;
                    });

                    var fi = new FormInfo(mappedClass.ClassFormDefinition);
                    foreach (var formFieldInfo in fi.GetFields(true, true))
                    {
                        if (formFieldInfo.PrimaryKey)
                        {
                            m.PrimaryKey = formFieldInfo.Name;
                        }
                        else
                        {
                            if (formFieldInfo.Settings["controlname"] is string controlName
                                && string.Equals(controlName, Kx13FormControls.UserControlForText.MediaSelectionControl, StringComparison.InvariantCultureIgnoreCase))
                            {
                                var fieldMigration = fieldMigrationService.GetFieldMigration(new(string.Empty, controlName, null, new CustomTableSourceObjectContext(string.Empty)));
                                if (fieldMigration is not null)
                                {
                                    m.BuildField(formFieldInfo.Name).ConvertFrom(mappedClass.ClassName, formFieldInfo.Name, true, (x, _) => x);
                                }
                                else
                                {
                                    throw new Exception($"No field migration found for {Kx13FormControls.UserControlForText.MediaSelectionControl} field {formFieldInfo.Name}");
                                }
                            }
                            else
                            {
                                m.BuildField(formFieldInfo.Name).SetFrom(mappedClass.ClassName, formFieldInfo.Name, true);
                            }
                        }
                    }

                    AppendConfiguredMapping(m);
                }
            }
        }
    }

    private void AppendConfiguredMapping(IClassMapping configuredClassMapping)
    {
        if (classMappings.Any(cm => cm.SourceClassNames.Any(scn => string.Equals(scn, configuredClassMapping.TargetClassName, StringComparison.InvariantCultureIgnoreCase))))
        {
            throw new InvalidOperationException($"Duplicate class mapping '{configuredClassMapping.TargetClassName}'. (check configuration 'ConvertClassesToContentHub')");
        }

        configuredClassMappings.Add(configuredClassMapping);
    }
}
