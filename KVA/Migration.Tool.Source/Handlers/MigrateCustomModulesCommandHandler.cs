using System.Collections.Immutable;
using System.Diagnostics;
using System.Xml.Linq;
using CMS.ContentEngine;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Modules;
using Kentico.Xperience.UMT.Services;
using MediatR;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.Enumerations;
using Migration.Tool.Common.Helpers;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Common.Services.BulkCopy;
using Migration.Tool.KXP.Api;
using Migration.Tool.KXP.Api.Services.CmsClass;
using Migration.Tool.Source.Contexts;
using Migration.Tool.Source.Helpers;
using Migration.Tool.Source.Mappers;
using Migration.Tool.Source.Model;
using Migration.Tool.Source.Providers;
using Newtonsoft.Json;

namespace Migration.Tool.Source.Handlers;

public class MigrateCustomModulesCommandHandler(
    ILogger<MigrateCustomModulesCommandHandler> logger,
    KxpClassFacade kxpClassFacade,
    IEntityMapper<ICmsClass, DataClassInfo> dataClassMapper,
    IEntityMapper<ICmsResource, ResourceInfo> resourceMapper,
    IEntityMapper<AlternativeFormMapperSource, AlternativeFormInfo> alternativeFormMapper,
    ToolConfiguration toolConfiguration,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    IProtocol protocol,
    BulkDataCopyService bulkDataCopyService,
    FieldMigrationService fieldMigrationService,
    ModelFacade modelFacade,
    ClassMappingProvider classMappingProvider,
    IUmtMapper<CustomModuleItemMapperSource> contentItemMapper,
    IImporter importer
)
    : IRequestHandler<MigrateCustomModulesCommand, CommandResult>
{
    public async Task<CommandResult> Handle(MigrateCustomModulesCommand request, CancellationToken cancellationToken)
    {
        var entityConfiguration = toolConfiguration.EntityConfigurations.GetEntityConfiguration<DataClassInfo>();

        await MigrateResources(cancellationToken);

        await MigrateClasses(entityConfiguration, cancellationToken);

        return new GenericCommandResult();
    }

    private async Task MigrateClasses(EntityConfiguration entityConfiguration, CancellationToken cancellationToken)
    {
        var manualMappings = classMappingProvider.ExecuteMappings();

        using var cmsClasses = EnumerableHelper.CreateDeferrableItemWrapper(
            modelFacade.SelectWhere<ICmsClass>("(ClassIsForm=0 OR ClassIsForm IS NULL) AND (ClassIsDocumentType=0 OR ClassIsDocumentType IS NULL)")
                .OrderBy(x => x.ClassID)
        );

        while (cmsClasses.GetNext(out var di))
        {
            var (_, cmsClass) = di;

            DataClassInfo? remappedClass = null;
            var remapped = () => remappedClass is not null;

            if (manualMappings.TryGetValue(cmsClass.ClassName, out var mappedToClass))
            {
                if (toolConfiguration.ClassNamesConvertToContentHub.Contains(cmsClass.ClassName, StringComparer.InvariantCultureIgnoreCase))
                {
                    remappedClass = mappedToClass.target;
                }
                else
                {
                    logger.LogError("Class {ClassName} has custom mapping, but is not listed as to be converted to reusable item by appsettings ConvertClassesToContentHub", cmsClass.ClassName);
                }
            }

            var resource = modelFacade.SelectById<ICmsResource>(cmsClass.ClassResourceID);
            if (resource?.ResourceName.StartsWith("CMS.") == true)
            {
                continue;
            }

            if (cmsClass.ClassIsCustomTable)
            {
                continue;
            }

            if (!remapped())
            {
                if (cmsClass.ClassInheritsFromClassID is { } classInheritsFromClassId && !primaryKeyMappingContext.HasMapping<ICmsClass>(c => c.ClassID, classInheritsFromClassId))
                {
                    // defer migration to later stage
                    if (cmsClasses.TryDeferItem(di))
                    {
                        logger.LogTrace("Class {Class} inheritance parent not found, deferring migration to end. Attempt {Attempt}", Printer.GetEntityIdentityPrint(cmsClass), di.Recurrence);
                    }
                    else
                    {
                        logger.LogErrorMissingDependency(cmsClass, nameof(cmsClass.ClassInheritsFromClassID), cmsClass.ClassInheritsFromClassID, typeof(DataClassInfo));
                        protocol.Append(HandbookReferences
                            .MissingRequiredDependency<ICmsClass>(nameof(ICmsClass.ClassID), classInheritsFromClassId)
                            .NeedsManualAction()
                        );
                    }

                    continue;
                }
            }

            protocol.FetchedSource(cmsClass);

            string? k12ResourceName = modelFacade.SelectById<ICmsResource>(cmsClass.ClassResourceID)?.ResourceName;
            if (k12ResourceName != null && K12SystemResource.All.Contains(k12ResourceName) && !K12SystemResource.ConvertToNonSysResource.Contains(k12ResourceName))
            {
                if (!K12SystemClass.Customizable.Contains(cmsClass.ClassName))
                {
                    logger.LogDebug("Class '{ClassClassName}' is part of system resource '{ResourceName}' and is not customizable", cmsClass.ClassName, cmsClass.ClassName);
                    continue;
                }

                logger.LogInformation("Class '{ClassClassName}' is part of system resource '{ResourceName}' and is customizable => migrated partially", cmsClass.ClassName,
                    cmsClass.ClassName);
            }

            if (entityConfiguration.ExcludeCodeNames.Contains(cmsClass.ClassName, StringComparer.InvariantCultureIgnoreCase))
            {
                protocol.Warning(HandbookReferences.EntityExplicitlyExcludedByCodeName(cmsClass.ClassName, "CustomModule"), cmsClass);
                logger.LogWarning("CmsClass: {ClassName} was skipped => it is explicitly excluded in configuration", cmsClass.ClassName);
                continue;
            }

            var xbkDataClass = kxpClassFacade.GetClass(cmsClass.ClassGUID);

            protocol.FetchedTarget(xbkDataClass);

            DataClassInfo? savedDataClass = remapped() ? remappedClass : SaveClassUsingKxoApi(cmsClass, xbkDataClass);

            if (savedDataClass is not null)
            {
                Debug.Assert(savedDataClass.ClassID != 0, "xbkDataClass.ClassID != 0");
                xbkDataClass = DataClassInfoProvider.ProviderObject.Get(savedDataClass.ClassID);
                await MigrateAlternativeForms(cmsClass, savedDataClass, cancellationToken);

                var codeNameProvider = Service.Resolve<IContentItemCodeNameProvider>();
                if (toolConfiguration.ClassNamesConvertToContentHub.Any(x => cmsClass.ClassName.Equals(x, StringComparison.InvariantCultureIgnoreCase)))
                {
                    var fi = new FormInfo(cmsClass.ClassFormDefinition);

                    string? guidColumn = fi.GetFields(true, true).FirstOrDefault(x => x.Caption == "GUID" && x.DataType == KsFieldDataType.Guid)?.Name;
                    if (guidColumn is null)
                    {
                        logger.LogError($"Guid column not found for class {cmsClass.ClassName}");
                        continue;
                    }

                    string? lastModifiedColumn = fi.GetFields(true, true).FirstOrDefault(x => x.Caption == "Last modified" && x.DataType == KsFieldDataType.DateTime)?.Name;
                    if (lastModifiedColumn is null)
                    {
                        logger.LogWarning($"Last modified column not found for class {cmsClass.ClassName}. Current date will be used.");
                    }

                    DateTime now = DateTime.Now;
                    int counter = 1;
                    var items = modelFacade.SelectAllAsDictionary(cmsClass.ClassTableName!);
                    foreach (var item in items)
                    {
                        if (item is null)
                        {
                            continue;
                        }

                        Guid sourceGuid = (Guid)item[guidColumn!]!;
                        DateTime lastModifiedDate = lastModifiedColumn is not null ? (DateTime)item[lastModifiedColumn]! : now;

                        string name = await codeNameProvider.Get($"{xbkDataClass.ClassName}-{counter++}");

                        string displayName = string.Empty;
                        if (toolConfiguration.CustomModuleClassDisplayNamePatterns is { } displayNamePatterns
                            && displayNamePatterns.TryGetValue(cmsClass.ClassName, out string? pattern)
                            && pattern is not null)
                        {
                            displayName = pattern;
                            foreach (var kv in item)
                            {
                                displayName = displayName.Replace($"{{{kv.Key}}}", kv.Value?.ToString() ?? string.Empty);
                            }
                        }
                        displayName = string.IsNullOrEmpty(displayName) ? name : displayName;

                        CustomModuleItemMapperSource src = new(name, displayName, sourceGuid, lastModifiedDate, cmsClass, xbkDataClass, item!);
                        var models = contentItemMapper.Map(src);
                        foreach (var umtModel in models)
                        {
                            switch (await importer.ImportAsync(umtModel))
                            {
                                case { Success: false } result:
                                {
                                    logger.LogError("Failed to import: {Exception}, {ValidationResults}", result.Exception, JsonConvert.SerializeObject(result.ModelValidationResults));
                                    break;
                                }
                                default:
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    #region Migrate coupled data class data

                    if (cmsClass.ClassShowAsSystemTable is false)
                    {
                        Debug.Assert(xbkDataClass.ClassTableName != null, "k12Class.ClassTableName != null");

                        XNamespace nsSchema = "http://www.w3.org/2001/XMLSchema";
                        XNamespace msSchema = "urn:schemas-microsoft-com:xml-msdata";
                        if (string.IsNullOrEmpty(xbkDataClass.ClassXmlSchema))
                        {
                            logger.LogError("CmsClass: {ClassName} has no XML schema. Migration incomplete", cmsClass.ClassName);
                            continue;
                        }
                        var xDoc = XDocument.Parse(xbkDataClass.ClassXmlSchema);
                        var autoIncrementColumns = xDoc.Descendants(nsSchema + "element")
                            .Where(x => x.Attribute(msSchema + "AutoIncrement")?.Value == "true")
                            .Select(x => x.Attribute("name")?.Value).ToImmutableHashSet();

                        Debug.Assert(autoIncrementColumns.Count == 1, "autoIncrementColumns.Count == 1");

                        var r = (xbkDataClass.ClassTableName, xbkDataClass.ClassGUID, autoIncrementColumns);
                        logger.LogTrace("Class '{ClassGuild}' Resolved as: {Result}", cmsClass.ClassGUID, r);

                        try
                        {
                            // check if data is present in target tables
                            if (bulkDataCopyService.CheckIfDataExistsInTargetTable(xbkDataClass.ClassTableName))
                            {
                                logger.LogWarning("Data exists in target coupled data table '{TableName}' - cannot migrate, skipping form data migration", r.ClassTableName);
                                protocol.Append(HandbookReferences.DataMustNotExistInTargetInstanceTable(xbkDataClass.ClassTableName));
                                continue;
                            }

                            var bulkCopyRequest = new BulkCopyRequest(
                                xbkDataClass.ClassTableName,
                                s => true, // s => !autoIncrementColumns.Contains(s),
                                _ => true,
                                20000
                            );

                            logger.LogTrace("Bulk data copy request: {Request}", bulkCopyRequest);
                            bulkDataCopyService.CopyTableToTable(bulkCopyRequest);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Error while copying data to table");
                        }
                    }
                }
                #endregion
            }
        }

        // special case - member migration (CMS_User splits into CMS_User and CMS_Member in XbyK)
        await MigrateMemberClass(cancellationToken);
    }

    private Task MigrateMemberClass(CancellationToken cancellationToken)
    {
        var cmsUser = modelFacade.SelectAll<ICmsClass>().FirstOrDefault(x => x.ClassName == K12SystemClass.cms_user);
        var cmsUserSettings = modelFacade.SelectAll<ICmsClass>().FirstOrDefault(x => x.ClassName == K12SystemClass.cms_usersettings);

        if (cmsUser == null)
        {
            protocol.Warning<ICmsUser>(HandbookReferences.InvalidSourceData<ICmsUser>().WithMessage($"{K12SystemClass.cms_user} class not found"), null);
            return Task.CompletedTask;
        }

        if (cmsUserSettings == null)
        {
            protocol.Warning<ICmsUserSetting>(HandbookReferences.InvalidSourceData<ICmsUserSetting>().WithMessage($"{K12SystemClass.cms_usersettings} class not found"), null);
            return Task.CompletedTask;
        }

        var target = kxpClassFacade.GetClass("CMS.Member");

        PatchClass(cmsUser, out var cmsUserCsi, out var cmsUserFi);
        PatchClass(cmsUserSettings, out var cmsUserSettingsCsi, out var cmsUserSettingsFi);

        var memberFormInfo = new FormInfo(target.ClassFormDefinition);

        string[] includedSystemFields = toolConfiguration.MemberIncludeUserSystemFields?.Split('|', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

        var memberColumns = memberFormInfo.GetColumnNames();

        foreach (string? uColumn in cmsUserFi.GetColumnNames())
        {
            var field = cmsUserFi.GetFormField(uColumn);

            if (
                !memberColumns.Contains(uColumn) &&
                !field.PrimaryKey &&
                !MemberInfoMapper.MigratedUserFields.Contains(uColumn, StringComparer.InvariantCultureIgnoreCase)
                && (includedSystemFields.Contains(uColumn) || !field.System)
            )
            {
                field.System = false; // no longer system field
                memberFormInfo.AddFormItem(field);
            }
        }

        foreach (string? usColumn in cmsUserSettingsFi.GetColumnNames())
        {
            var field = cmsUserSettingsFi.GetFormField(usColumn);

            if (
                !memberColumns.Contains(usColumn) &&
                !field.PrimaryKey
                && (includedSystemFields.Contains(usColumn) || !field.System))
            {
                field.System = false; // no longer system field
                memberFormInfo.AddFormItem(field);
            }
        }

        target.ClassFormDefinition = memberFormInfo.GetXmlDefinition();
        DataClassInfoProvider.ProviderObject.Set(target);
        return Task.CompletedTask;
    }

    private void PatchClass(ICmsClass cmsClass, out ClassStructureInfo classStructureInfo, out FormInfo cmsUserFormInfo)
    {
        classStructureInfo = new ClassStructureInfo(cmsClass.ClassName, cmsClass.ClassXmlSchema, cmsClass.ClassTableName);
        var patcher = new FormDefinitionPatcher(
            logger,
            cmsClass.ClassFormDefinition,
            fieldMigrationService,
            cmsClass.ClassIsForm.GetValueOrDefault(false),
            cmsClass.ClassIsDocumentType,
            false,
            false
        );
        patcher.PatchFields();
        patcher.RemoveCategories();
        string? result = patcher.GetPatched();
        cmsUserFormInfo = new FormInfo(result);
    }

    private Task MigrateAlternativeForms(ICmsClass k12Class, DataClassInfo xbkDataClass, CancellationToken cancellationToken)
    {
        var k12AlternativeForms = modelFacade.SelectAll<ICmsAlternativeForm>()
            .Where(af => af.FormClassID == k12Class.ClassID);

        foreach (var k12AlternativeForm in k12AlternativeForms)
        {
            protocol.FetchedSource(k12AlternativeForm);

            var xbkAlternativeForm = AlternativeFormInfoProvider.ProviderObject.Get(k12AlternativeForm.FormGUID);
            protocol.FetchedTarget(xbkAlternativeForm);

            var mappingSource = new AlternativeFormMapperSource(k12AlternativeForm, xbkDataClass);
            var mapped = alternativeFormMapper.Map(mappingSource, xbkAlternativeForm);
            protocol.MappedTarget(mapped);

            try
            {
                if (mapped is { Success: true })
                {
                    (var alternativeFormInfo, bool newInstance) = mapped;
                    ArgumentNullException.ThrowIfNull(alternativeFormInfo, nameof(alternativeFormInfo));

                    AlternativeFormInfoProvider.ProviderObject.Set(alternativeFormInfo);

                    protocol.Success(k12AlternativeForm, alternativeFormInfo, mapped);
                    logger.LogEntitySetAction(newInstance, alternativeFormInfo);

                    primaryKeyMappingContext.SetMapping<ICmsAlternativeForm>(
                        r => r.FormID,
                        k12AlternativeForm.FormID,
                        alternativeFormInfo.FormID
                    );
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while saving alternative form {ResourceName}", k12AlternativeForm.FormName);
            }
        }
        return Task.CompletedTask;
    }

    private Task<List<ICmsClass>> GetResourceClasses(int k12ResourceId) => Task.FromResult(modelFacade
        .SelectWhere<ICmsClass>("ClassResourceID = @classResourceId", new SqlParameter("classResourceId", k12ResourceId))
        .ToList());

    private async Task MigrateResources(CancellationToken cancellationToken)
    {
        var k12CmsResources = modelFacade.SelectAll<ICmsResource>();

        foreach (var k12CmsResource in k12CmsResources)
        {
            protocol.FetchedSource(k12CmsResource);

#pragma warning disable CS0618 // Type or member is obsolete
            var xbkResource = ResourceInfoProvider.ProviderObject.Get(k12CmsResource.ResourceGUID);
#pragma warning restore CS0618 // Type or member is obsolete

            protocol.FetchedTarget(xbkResource);

            bool sysResourceInclude = K12SystemResource.ConvertToNonSysResource.Contains(k12CmsResource.ResourceName);
            bool isSystemResource = K12SystemResource.All.Contains(k12CmsResource.ResourceName);
            if (isSystemResource)
            {
                if (sysResourceInclude)
                {
                    logger.LogDebug("CMSResource is system resource ({Resource}) and is included in migration", Printer.GetEntityIdentityPrint(k12CmsResource));
                }
                else
                {
                    logger.LogDebug("CMSResource is system resource ({Resource})", Printer.GetEntityIdentityPrint(k12CmsResource));

                    var k12Classes = await GetResourceClasses(k12CmsResource.ResourceID);
                    if (k12Classes.Any(x => K12SystemClass.Customizable.Contains(x.ClassName)))
                    {
                        logger.LogDebug("CMSResource ({Resource}) contains customizable classes", Printer.GetEntityIdentityPrint(k12CmsResource));
                        if (xbkResource != null)
                        {
                            var handbookRef = HandbookReferences
                                .NotSupportedSkip<ICmsResource>()
                                .WithIdentityPrint(k12CmsResource);

                            logger.LogInformation("CMSResource is system resource and exists in target instance ({Resource}) => skipping", Printer.GetEntityIdentityPrint(k12CmsResource));
                            protocol.Append(handbookRef);
                            continue;
                        }

                        logger.LogInformation("CMSResource is system resource and NOT exists in target instance ({Resource}), contains customizable classes => will be migrated",
                            Printer.GetEntityIdentityPrint(k12CmsResource));
                    }
                    else
                    {
                        var handbookRef = HandbookReferences
                            .NotSupportedSkip<ICmsResource>()
                            .WithIdentityPrint(k12CmsResource);

                        logger.LogInformation("CMSResource is system resource and exists in target instance ({Resource}) => skipping", Printer.GetEntityIdentityPrint(k12CmsResource));
                        protocol.Append(handbookRef);
                        continue;
                    }
                }
            }
            else
            {
                logger.LogDebug("CMSResource is CUSTOM resource ({Resource})", Printer.GetEntityIdentityPrint(k12CmsResource));
            }

            var mapped = resourceMapper.Map(k12CmsResource, xbkResource);
            protocol.MappedTarget(mapped);

            try
            {
                if (mapped is { Success: true })
                {
                    (var resourceInfo, bool newInstance) = mapped;
                    ArgumentNullException.ThrowIfNull(resourceInfo, nameof(resourceInfo));

#pragma warning disable CS0618 // Type or member is obsolete
                    ResourceInfoProvider.ProviderObject.Set(resourceInfo);
#pragma warning restore CS0618 // Type or member is obsolete

                    protocol.Success(k12CmsResource, resourceInfo, mapped);
                    logger.LogEntitySetAction(newInstance, resourceInfo);

                    primaryKeyMappingContext.SetMapping<ICmsResource>(
                        r => r.ResourceID,
                        k12CmsResource.ResourceID,
                        resourceInfo.ResourceID
                    );
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while saving resource {ResourceName}", k12CmsResource.ResourceName);
            }
        }
    }

    private DataClassInfo? SaveClassUsingKxoApi(ICmsClass k12Class, DataClassInfo kxoDataClass)
    {
        var mapped = dataClassMapper.Map(k12Class, kxoDataClass);
        protocol.MappedTarget(mapped);

        try
        {
            if (mapped is { Success: true } result)
            {
                (var dataClassInfo, bool newInstance) = result;
                ArgumentNullException.ThrowIfNull(dataClassInfo, nameof(dataClassInfo));

                kxpClassFacade.SetClass(dataClassInfo);

                protocol.Success(k12Class, dataClassInfo, mapped);
                logger.LogEntitySetAction(newInstance, dataClassInfo);

                primaryKeyMappingContext.SetMapping<ICmsClass>(
                    r => r.ClassID,
                    k12Class.ClassID,
                    dataClassInfo.ClassID
                );

                return dataClassInfo;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while saving page type {ClassName}", k12Class.ClassName);
        }

        return null;
    }
}
