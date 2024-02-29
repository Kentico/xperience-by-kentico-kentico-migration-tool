namespace Migration.Toolkit.Core.K11.Handlers;

using System.Collections.Immutable;
using System.Diagnostics;
using System.Xml.Linq;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Modules;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.Enumerations;
using Migration.Toolkit.Common.Helpers;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Common.Services.BulkCopy;
using Migration.Toolkit.Core.K11.Contexts;
using Migration.Toolkit.Core.K11.Helpers;
using Migration.Toolkit.Core.K11.Mappers;
using Migration.Toolkit.Core.K11.Services;
using Migration.Toolkit.Core.K11.Services.CmsClass;
using Migration.Toolkit.K11;
using Migration.Toolkit.K11.Models;
using Migration.Toolkit.KXP.Api;

public class MigrateCustomModulesCommandHandler(ILogger<MigrateCustomModulesCommandHandler> logger,
        IDbContextFactory<K11Context> k11ContextFactory,
        KxpClassFacade kxpClassFacade,
        IEntityMapper<CmsClass, DataClassInfo> dataClassMapper,
        IEntityMapper<CmsResource, ResourceInfo> resourceMapper,
        IEntityMapper<AlternativeFormMapperSource, AlternativeFormInfo> alternativeFormMapper,
        ToolkitConfiguration toolkitConfiguration,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol,
        BulkDataCopyService bulkDataCopyService,
        FieldMigrationService fieldMigrationService)
    : IRequestHandler<MigrateCustomModulesCommand, CommandResult>
{
    public async Task<CommandResult> Handle(MigrateCustomModulesCommand request, CancellationToken cancellationToken)
    {
        var entityConfiguration = toolkitConfiguration.EntityConfigurations.GetEntityConfiguration<CmsClass>();

        await MigrateResources(cancellationToken);

        await MigrateClasses(entityConfiguration, cancellationToken);

        return new GenericCommandResult();
    }

    private async Task MigrateClasses(EntityConfiguration entityConfiguration, CancellationToken cancellationToken)
    {
        await using var k11Context = await k11ContextFactory.CreateDbContextAsync(cancellationToken);
        var k11ClassesResult = k11Context.CmsClasses
            .Include(c => c.ClassResource).ThenInclude(cr => cr.Sites)
            .Include(c => c.Sites)
            .Where(x => !(x.ClassIsForm ?? false) && !x.ClassIsDocumentType && !x.ClassResource.ResourceName.StartsWith("CMS."))
            .OrderBy(x => x.ClassId)
            .AsSplitQuery();

        using var k11Classes = EnumerableHelper.CreateDeferrableItemWrapper(k11ClassesResult);

        while (k11Classes.GetNext(out var di))
        {
            var (_, k11Class) = di;

            if (k11Class.ClassIsCustomTable)
            {
                continue;
            }

            if (k11Class.ClassInheritsFromClassId is { } classInheritsFromClassId && !primaryKeyMappingContext.HasMapping<CmsClass>(c => c.ClassId, classInheritsFromClassId))
            {
                // defer migration to later stage
                if (k11Classes.TryDeferItem(di))
                {
                    logger.LogTrace("Class {Class} inheritance parent not found, deferring migration to end. Attempt {Attempt}", Printer.GetEntityIdentityPrint(k11Class), di.Recurrence);
                }
                else
                {
                    logger.LogErrorMissingDependency(k11Class, nameof(k11Class.ClassInheritsFromClassId), k11Class.ClassInheritsFromClassId, typeof(DataClassInfo));
                    protocol.Append(HandbookReferences
                        .MissingRequiredDependency<CmsClass>(nameof(CmsClass.ClassId), classInheritsFromClassId)
                        .NeedsManualAction()
                    );
                }

                continue;
            }

            protocol.FetchedSource(k11Class);

            var k11ResourceName = k11Class.ClassResource?.ResourceName;
            if (k11ResourceName != null && K12SystemResource.All.Contains(k11ResourceName) && !K12SystemResource.ConvertToNonSysResource.Contains(k11ResourceName))
            {
                if (!K12SystemClass.Customizable.Contains(k11Class.ClassName))
                {
                    logger.LogDebug("Class '{ClassClassName}' is part of system resource '{ResourceName}' and is not customizable", k11Class.ClassName, k11Class.ClassName);
                    continue;
                }

                logger.LogInformation("Class '{ClassClassName}' is part of system resource '{ResourceName}' and is customizable => migrated partially", k11Class.ClassName,
                    k11Class.ClassName);
            }

            if (entityConfiguration.ExcludeCodeNames.Contains(k11Class.ClassName, StringComparer.InvariantCultureIgnoreCase))
            {
                protocol.Warning(HandbookReferences.EntityExplicitlyExcludedByCodeName(k11Class.ClassName, "CustomModule"), k11Class);
                logger.LogWarning("CmsClass: {ClassName} was skipped => it is explicitly excluded in configuration", k11Class.ClassName);
                continue;
            }

            var xbkDataClass = kxpClassFacade.GetClass(k11Class.ClassGuid);

            protocol.FetchedTarget(xbkDataClass);

            if (SaveClassUsingKxoApi(k11Class, xbkDataClass) is {} savedDataClass)
            {
                Debug.Assert(savedDataClass.ClassID != 0, "xbkDataClass.ClassID != 0");
                xbkDataClass = DataClassInfoProvider.ProviderObject.Get(savedDataClass.ClassID);
                await MigrateAlternativeForms(k11Class, savedDataClass, cancellationToken);

                #region Migrate coupled data class data

                if (k11Class.ClassShowAsSystemTable is false)
                {
                    XNamespace nsSchema = "http://www.w3.org/2001/XMLSchema";
                    XNamespace msSchema = "urn:schemas-microsoft-com:xml-msdata";
                    var xDoc = XDocument.Parse(xbkDataClass.ClassXmlSchema);
                    var autoIncrementColumns = xDoc.Descendants(nsSchema + "element")
                        .Where(x => x.Attribute(msSchema + "AutoIncrement")?.Value == "true")
                        .Select(x => x.Attribute("name")?.Value).ToImmutableHashSet();

                    Debug.Assert(autoIncrementColumns.Count == 1, "autoIncrementColumns.Count == 1");

                    var r = (xbkDataClass.ClassTableName, xbkDataClass.ClassGUID, autoIncrementColumns);
                    logger.LogTrace("Class '{ClassGuild}' Resolved as: {Result}", k11Class.ClassGuid, r);

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
                        logger.LogError(ex,"Error while copying data to table");
                    }
                }

                #endregion
            }
        }

        // special case - member migration (CMS_User splits into CMS_User and CMS_Member in XbK)
        await MigrateMemberClass(cancellationToken);
    }

    private async Task MigrateMemberClass(CancellationToken cancellationToken)
    {
        await using var k11Context = await k11ContextFactory.CreateDbContextAsync(cancellationToken);

        var cmsUser = k11Context.CmsClasses.FirstOrDefault(x => x.ClassName == K12SystemClass.cms_user);
        var cmsUserSettings = k11Context.CmsClasses.FirstOrDefault(x => x.ClassName == K12SystemClass.cms_usersettings);

        if (cmsUser == null)
        {
            protocol.Warning<CmsUser>(HandbookReferences.InvalidSourceData<CmsUser>().WithMessage($"{K12SystemClass.cms_user} class not found"), null);
            return;
        }

        if (cmsUserSettings == null)
        {
            protocol.Warning<CmsUserSetting>(HandbookReferences.InvalidSourceData<CmsUserSetting>().WithMessage($"{K12SystemClass.cms_usersettings} class not found"), null);
            return;
        }

        var target = kxpClassFacade.GetClass("CMS.Member");

        PatchClass(cmsUser, out var cmsUserCsi, out var cmsUserFi);
        PatchClass(cmsUserSettings, out var cmsUserSettingsCsi, out var cmsUserSettingsFi);

        var memberFormInfo = new FormInfo(target.ClassFormDefinition);

        var includedSystemFields = toolkitConfiguration.MemberIncludeUserSystemFields?.Split('|', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

        var memberColumns = memberFormInfo.GetColumnNames();

        foreach (var uColumn in cmsUserFi.GetColumnNames())
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

        foreach (var usColumn in cmsUserSettingsFi.GetColumnNames())
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
    }

    private void PatchClass(CmsClass cmsUser, out ClassStructureInfo classStructureInfo, out FormInfo cmsUserFormInfo)
    {
        classStructureInfo = new ClassStructureInfo(cmsUser.ClassName, cmsUser.ClassXmlSchema, cmsUser.ClassTableName);
        var patcher = new FormDefinitionPatcher(
            logger,
            cmsUser.ClassFormDefinition,
            fieldMigrationService,
            cmsUser.ClassIsForm.GetValueOrDefault(false),
            cmsUser.ClassIsDocumentType,
            false,
            false
        );
        patcher.PatchFields();
        patcher.RemoveCategories(); // TODO tk: 2022-10-11 remove when supported
        var result = patcher.GetPatched();
        cmsUserFormInfo = new FormInfo(result);
    }

    private async Task MigrateAlternativeForms(CmsClass k11Class, DataClassInfo xbkDataClass, CancellationToken cancellationToken)
    {
        var k11Context = await k11ContextFactory.CreateDbContextAsync(cancellationToken);

        var k11AlternativeForms = k11Context.CmsAlternativeForms
            .Include(af => af.FormClass)
            .Include(af => af.FormCoupledClass)
            .Where(af => af.FormClassId == k11Class.ClassId);

        foreach (var k11AlternativeForm in k11AlternativeForms)
        {
            protocol.FetchedSource(k11AlternativeForm);

            var xbkAlternativeForm = AlternativeFormInfoProvider.ProviderObject.Get(k11AlternativeForm.FormGuid);
            protocol.FetchedTarget(xbkAlternativeForm);

            var mappingSource = new AlternativeFormMapperSource(k11AlternativeForm, xbkDataClass);
            var mapped = alternativeFormMapper.Map(mappingSource, xbkAlternativeForm);
            protocol.MappedTarget(mapped);

            try
            {
                if (mapped is { Success : true })
                {
                    var (alternativeFormInfo, newInstance) = mapped;
                    ArgumentNullException.ThrowIfNull(alternativeFormInfo, nameof(alternativeFormInfo));

                    AlternativeFormInfoProvider.ProviderObject.Set(alternativeFormInfo);

                    protocol.Success(k11AlternativeForm, alternativeFormInfo, mapped);
                    logger.LogEntitySetAction(newInstance, alternativeFormInfo);

                    primaryKeyMappingContext.SetMapping<CmsAlternativeForm>(
                        r => r.FormId,
                        k11AlternativeForm.FormId,
                        alternativeFormInfo.FormID
                    );
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while saving alternative form {ResourceName}", k11AlternativeForm.FormName);
            }
        }
    }

    private async Task<List<CmsClass>> GetResourceClasses(int k11ResourceId, CancellationToken cancellationToken)
    {
        await using var k11Context = await k11ContextFactory.CreateDbContextAsync(cancellationToken);

        return await k11Context.CmsClasses.Where(x => x.ClassResourceId == k11ResourceId).ToListAsync();
    }

    private async Task MigrateResources(CancellationToken cancellationToken)
    {
        await using var k11Context = await k11ContextFactory.CreateDbContextAsync(cancellationToken);

        var k11CmsResources = k11Context.CmsResources
            .Include(cr => cr.Sites);

        foreach (var k11CmsResource in k11CmsResources)
        {
            protocol.FetchedSource(k11CmsResource);

            var xbkResource = ResourceInfoProvider.ProviderObject.Get(k11CmsResource.ResourceGuid);

            protocol.FetchedTarget(xbkResource);

            var sysResourceInclude = K12SystemResource.ConvertToNonSysResource.Contains(k11CmsResource.ResourceName);
            var isSystemResource = K12SystemResource.All.Contains(k11CmsResource.ResourceName);
            if (isSystemResource)
            {
                if (sysResourceInclude)
                {
                    logger.LogDebug("CMSResource is system resource ({Resource}) and is included in migration", Printer.GetEntityIdentityPrint(k11CmsResource));
                }
                else
                {
                    logger.LogDebug("CMSResource is system resource ({Resource})", Printer.GetEntityIdentityPrint(k11CmsResource));

                    var k11Classes = await GetResourceClasses(k11CmsResource.ResourceId, cancellationToken);
                    if (k11Classes.Any(x => K12SystemClass.Customizable.Contains(x.ClassName)))
                    {
                        logger.LogDebug("CMSResource ({Resource}) contains customizable classes", Printer.GetEntityIdentityPrint(k11CmsResource));
                        if (xbkResource != null)
                        {
                            var handbookRef = HandbookReferences
                                .NotSupportedSkip<CmsResource>()
                                .WithIdentityPrint(k11CmsResource);

                            logger.LogInformation("CMSResource is system resource and exists in target instance ({Resource}) => skipping", Printer.GetEntityIdentityPrint(k11CmsResource));
                            protocol.Append(handbookRef);
                            continue;
                        }

                        logger.LogInformation("CMSResource is system resource and NOT exists in target instance ({Resource}), contains customizable classes => will be migrated",
                            Printer.GetEntityIdentityPrint(k11CmsResource));
                    }
                    else
                    {
                        var handbookRef = HandbookReferences
                            .NotSupportedSkip<CmsResource>()
                            .WithIdentityPrint(k11CmsResource);

                        logger.LogInformation("CMSResource is system resource and exists in target instance ({Resource}) => skipping", Printer.GetEntityIdentityPrint(k11CmsResource));
                        protocol.Append(handbookRef);
                        continue;
                    }
                }
            }
            else
            {
                logger.LogDebug("CMSResource is CUSTOM resource ({Resource})", Printer.GetEntityIdentityPrint(k11CmsResource));
            }

            var mapped = resourceMapper.Map(k11CmsResource, xbkResource);
            protocol.MappedTarget(mapped);

            try
            {
                if (mapped is { Success : true })
                {
                    var (resourceInfo, newInstance) = mapped;
                    ArgumentNullException.ThrowIfNull(resourceInfo, nameof(resourceInfo));

                    ResourceInfoProvider.ProviderObject.Set(resourceInfo);

                    protocol.Success(k11CmsResource, resourceInfo, mapped);
                    logger.LogEntitySetAction(newInstance, resourceInfo);

                    primaryKeyMappingContext.SetMapping<CmsResource>(
                        r => r.ResourceId,
                        k11CmsResource.ResourceId,
                        resourceInfo.ResourceID
                    );
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while saving resource {ResourceName}", k11CmsResource.ResourceName);
            }
        }
    }

    private DataClassInfo? SaveClassUsingKxoApi(CmsClass k11Class, DataClassInfo kxoDataClass)
    {
        var mapped = dataClassMapper.Map(k11Class, kxoDataClass);
        protocol.MappedTarget(mapped);

        try
        {
            if (mapped is { Success : true } result)
            {
                var (dataClassInfo, newInstance) = result;
                ArgumentNullException.ThrowIfNull(dataClassInfo, nameof(dataClassInfo));

                kxpClassFacade.SetClass(dataClassInfo);

                protocol.Success(k11Class, dataClassInfo, mapped);
                logger.LogEntitySetAction(newInstance, dataClassInfo);

                primaryKeyMappingContext.SetMapping<CmsClass>(
                    r => r.ClassId,
                    k11Class.ClassId,
                    dataClassInfo.ClassID
                );

                return dataClassInfo;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while saving page type {ClassName}", k11Class.ClassName);
        }

        return null;
    }
}