namespace Migration.Toolkit.Core.KX12.Handlers;

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
using Migration.Toolkit.Core.KX12.Contexts;
using Migration.Toolkit.Core.KX12.Helpers;
using Migration.Toolkit.Core.KX12.Mappers;
using Migration.Toolkit.Core.KX12.Services;
using Migration.Toolkit.Core.KX12.Services.CmsClass;
using Migration.Toolkit.KX12.Context;
using Migration.Toolkit.KX12.Models;
using Migration.Toolkit.KXP.Api;

public class MigrateCustomModulesCommandHandler : IRequestHandler<MigrateCustomModulesCommand, CommandResult>
{
    private readonly ILogger<MigrateCustomModulesCommandHandler> _logger;
    private readonly IDbContextFactory<KX12Context> _kx12ContextFactory;
    private readonly KxpClassFacade _kxpClassFacade;
    private readonly IEntityMapper<KX12M.CmsClass, DataClassInfo> _dataClassMapper;
    private readonly IEntityMapper<KX12M.CmsResource, ResourceInfo> _resourceMapper;
    private readonly IEntityMapper<AlternativeFormMapperSource, AlternativeFormInfo> _alternativeFormMapper;
    private readonly ToolkitConfiguration _toolkitConfiguration;
    private readonly PrimaryKeyMappingContext _primaryKeyMappingContext;
    private readonly IProtocol _protocol;
    private readonly BulkDataCopyService _bulkDataCopyService;
    private readonly FieldMigrationService _fieldMigrationService;

    public MigrateCustomModulesCommandHandler(
        ILogger<MigrateCustomModulesCommandHandler> logger,
        IDbContextFactory<KX12Context> kx12ContextFactory,
        KxpClassFacade kxpClassFacade,
        IEntityMapper<KX12M.CmsClass, DataClassInfo> dataClassMapper,
        IEntityMapper<KX12M.CmsResource, ResourceInfo> resourceMapper,
        IEntityMapper<AlternativeFormMapperSource, AlternativeFormInfo> alternativeFormMapper,
        ToolkitConfiguration toolkitConfiguration,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol,
        BulkDataCopyService bulkDataCopyService,
        FieldMigrationService fieldMigrationService)
    {
        _logger = logger;
        _kx12ContextFactory = kx12ContextFactory;
        _kxpClassFacade = kxpClassFacade;
        _dataClassMapper = dataClassMapper;
        _resourceMapper = resourceMapper;
        _alternativeFormMapper = alternativeFormMapper;
        _toolkitConfiguration = toolkitConfiguration;
        _primaryKeyMappingContext = primaryKeyMappingContext;
        _protocol = protocol;
        _bulkDataCopyService = bulkDataCopyService;
        _fieldMigrationService = fieldMigrationService;
    }


    public async Task<CommandResult> Handle(MigrateCustomModulesCommand request, CancellationToken cancellationToken)
    {
        var entityConfiguration = _toolkitConfiguration.EntityConfigurations.GetEntityConfiguration<KX12M.CmsClass>();

        await MigrateResources(cancellationToken);

        await MigrateClasses(entityConfiguration, cancellationToken);

        return new GenericCommandResult();
    }

    private async Task MigrateClasses(EntityConfiguration entityConfiguration, CancellationToken cancellationToken)
    {
        await using var kx12Context = await _kx12ContextFactory.CreateDbContextAsync(cancellationToken);
        var kx12ClassesResult = kx12Context.CmsClasses
            .Include(c => c.ClassResource).ThenInclude(cr => cr.Sites)
            .Include(c => c.Sites)
            .Where(x => !(x.ClassIsForm ?? false) && !x.ClassIsDocumentType && !x.ClassResource.ResourceName.StartsWith("CMS."))
            .OrderBy(x => x.ClassId)
            .AsSplitQuery();

        using var kx12Classes = EnumerableHelper.CreateDeferrableItemWrapper(kx12ClassesResult);

        while (kx12Classes.GetNext(out var di))
        {
            var (_, kx12Class) = di;

            if (kx12Class.ClassIsCustomTable)
            {
                continue;
            }

            if (kx12Class.ClassInheritsFromClassId is { } classInheritsFromClassId && !_primaryKeyMappingContext.HasMapping<KX12M.CmsClass>(c => c.ClassId, classInheritsFromClassId))
            {
                // defer migration to later stage
                if (kx12Classes.TryDeferItem(di))
                {
                    _logger.LogTrace("Class {Class} inheritance parent not found, deferring migration to end. Attempt {Attempt}", Printer.GetEntityIdentityPrint(kx12Class), di.Recurrence);
                }
                else
                {
                    _logger.LogErrorMissingDependency(kx12Class, nameof(kx12Class.ClassInheritsFromClassId), kx12Class.ClassInheritsFromClassId, typeof(DataClassInfo));
                    _protocol.Append(HandbookReferences
                        .MissingRequiredDependency<KX12M.CmsClass>(nameof(KX12M.CmsClass.ClassId), classInheritsFromClassId)
                        .NeedsManualAction()
                    );
                }

                continue;
            }

            _protocol.FetchedSource(kx12Class);

            var k12ResourceName = kx12Class.ClassResource?.ResourceName;
            if (k12ResourceName != null && K12SystemResource.All.Contains(k12ResourceName) && !K12SystemResource.ConvertToNonSysResource.Contains(k12ResourceName))
            {
                if (!K12SystemClass.Customizable.Contains(kx12Class.ClassName))
                {
                    _logger.LogDebug("Class '{ClassClassName}' is part of system resource '{ResourceName}' and is not customizable", kx12Class.ClassName, kx12Class.ClassName);
                    continue;
                }

                _logger.LogInformation("Class '{ClassClassName}' is part of system resource '{ResourceName}' and is customizable => migrated partially", kx12Class.ClassName,
                    kx12Class.ClassName);
            }

            if (entityConfiguration.ExcludeCodeNames.Contains(kx12Class.ClassName, StringComparer.InvariantCultureIgnoreCase))
            {
                _protocol.Warning(HandbookReferences.EntityExplicitlyExcludedByCodeName(kx12Class.ClassName, "CustomModule"), kx12Class);
                _logger.LogWarning("CmsClass: {ClassName} was skipped => it is explicitly excluded in configuration", kx12Class.ClassName);
                continue;
            }

            var xbkDataClass = _kxpClassFacade.GetClass(kx12Class.ClassGuid);

            _protocol.FetchedTarget(xbkDataClass);

            if (SaveClassUsingKxoApi(kx12Class, xbkDataClass) is {} savedDataClass)
            {
                Debug.Assert(savedDataClass.ClassID != 0, "xbkDataClass.ClassID != 0");
                xbkDataClass = DataClassInfoProvider.ProviderObject.Get(savedDataClass.ClassID);
                await MigrateAlternativeForms(kx12Class, savedDataClass, cancellationToken);

                #region Migrate coupled data class data

                if (kx12Class.ClassShowAsSystemTable is false)
                {
                    Debug.Assert(xbkDataClass.ClassTableName != null, "k12Class.ClassTableName != null");

                    XNamespace nsSchema = "http://www.w3.org/2001/XMLSchema";
                    XNamespace msSchema = "urn:schemas-microsoft-com:xml-msdata";
                    var xDoc = XDocument.Parse(xbkDataClass.ClassXmlSchema);
                    var autoIncrementColumns = xDoc.Descendants(nsSchema + "element")
                        .Where(x => x.Attribute(msSchema + "AutoIncrement")?.Value == "true")
                        .Select(x => x.Attribute("name")?.Value).ToImmutableHashSet();

                    Debug.Assert(autoIncrementColumns.Count == 1, "autoIncrementColumns.Count == 1");

                    var r = (xbkDataClass.ClassTableName, xbkDataClass.ClassGUID, autoIncrementColumns);
                    _logger.LogTrace("Class '{ClassGuild}' Resolved as: {Result}", kx12Class.ClassGuid, r);

                    try
                    {
                        // check if data is present in target tables
                        if (_bulkDataCopyService.CheckIfDataExistsInTargetTable(xbkDataClass.ClassTableName))
                        {
                            _logger.LogWarning("Data exists in target coupled data table '{TableName}' - cannot migrate, skipping form data migration", r.ClassTableName);
                            _protocol.Append(HandbookReferences.DataMustNotExistInTargetInstanceTable(xbkDataClass.ClassTableName));
                            continue;
                        }

                        var bulkCopyRequest = new BulkCopyRequest(
                            xbkDataClass.ClassTableName,
                            s => true, // s => !autoIncrementColumns.Contains(s),
                            _ => true,
                            20000
                        );

                        _logger.LogTrace("Bulk data copy request: {Request}", bulkCopyRequest);
                        _bulkDataCopyService.CopyTableToTable(bulkCopyRequest);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,"Error while copying data to table");
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
        await using var kx12Context = await _kx12ContextFactory.CreateDbContextAsync(cancellationToken);

        var cmsUser = kx12Context.CmsClasses.FirstOrDefault(x => x.ClassName == K12SystemClass.cms_user);
        var cmsUserSettings = kx12Context.CmsClasses.FirstOrDefault(x => x.ClassName == K12SystemClass.cms_usersettings);

        if (cmsUser == null)
        {
            _protocol.Warning<KX12M.CmsUser>(HandbookReferences.InvalidSourceData<KX12M.CmsUser>().WithMessage($"{K12SystemClass.cms_user} class not found"), null);
            return;
        }

        if (cmsUserSettings == null)
        {
            _protocol.Warning<KX12M.CmsUserSetting>(HandbookReferences.InvalidSourceData<KX12M.CmsUserSetting>().WithMessage($"{K12SystemClass.cms_usersettings} class not found"), null);
            return;
        }

        var target = _kxpClassFacade.GetClass("CMS.Member");

        PatchClass(cmsUser, out var cmsUserCsi, out var cmsUserFi);
        PatchClass(cmsUserSettings, out var cmsUserSettingsCsi, out var cmsUserSettingsFi);

        var memberFormInfo = new FormInfo(target.ClassFormDefinition);

        var includedSystemFields = _toolkitConfiguration.MemberIncludeUserSystemFields?.Split('|', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

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
            _logger,
            cmsUser.ClassFormDefinition,
            _fieldMigrationService,
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

    private async Task MigrateAlternativeForms(CmsClass k12Class, DataClassInfo xbkDataClass, CancellationToken cancellationToken)
    {
        var kx12Context = await _kx12ContextFactory.CreateDbContextAsync(cancellationToken);

        var k12AlternativeForms = kx12Context.CmsAlternativeForms
            .Include(af => af.FormClass)
            .Include(af => af.FormCoupledClass)
            .Where(af => af.FormClassId == k12Class.ClassId);

        foreach (var k12AlternativeForm in k12AlternativeForms)
        {
            _protocol.FetchedSource(k12AlternativeForm);

            var xbkAlternativeForm = AlternativeFormInfoProvider.ProviderObject.Get(k12AlternativeForm.FormGuid);
            _protocol.FetchedTarget(xbkAlternativeForm);

            var mappingSource = new AlternativeFormMapperSource(k12AlternativeForm, xbkDataClass);
            var mapped = _alternativeFormMapper.Map(mappingSource, xbkAlternativeForm);
            _protocol.MappedTarget(mapped);

            try
            {
                if (mapped is { Success : true })
                {
                    var (alternativeFormInfo, newInstance) = mapped;
                    ArgumentNullException.ThrowIfNull(alternativeFormInfo, nameof(alternativeFormInfo));

                    AlternativeFormInfoProvider.ProviderObject.Set(alternativeFormInfo);

                    _protocol.Success(k12AlternativeForm, alternativeFormInfo, mapped);
                    _logger.LogEntitySetAction(newInstance, alternativeFormInfo);

                    _primaryKeyMappingContext.SetMapping<KX12M.CmsAlternativeForm>(
                        r => r.FormId,
                        k12AlternativeForm.FormId,
                        alternativeFormInfo.FormID
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while saving alternative form {ResourceName}", k12AlternativeForm.FormName);
            }
        }
    }

    private async Task<List<KX12M.CmsClass>> GetResourceClasses(int k12ResourceId, CancellationToken cancellationToken)
    {
        await using var kx12Context = await _kx12ContextFactory.CreateDbContextAsync(cancellationToken);

        return await kx12Context.CmsClasses.Where(x => x.ClassResourceId == k12ResourceId).ToListAsync();
    }

    private async Task MigrateResources(CancellationToken cancellationToken)
    {
        await using var kx12Context = await _kx12ContextFactory.CreateDbContextAsync(cancellationToken);

        var k12CmsResources = kx12Context.CmsResources
            .Include(cr => cr.Sites);

        foreach (var k12CmsResource in k12CmsResources)
        {
            _protocol.FetchedSource(k12CmsResource);

            var xbkResource = ResourceInfoProvider.ProviderObject.Get(k12CmsResource.ResourceGuid);

            _protocol.FetchedTarget(xbkResource);

            var sysResourceInclude = K12SystemResource.ConvertToNonSysResource.Contains(k12CmsResource.ResourceName);
            var isSystemResource = K12SystemResource.All.Contains(k12CmsResource.ResourceName);
            if (isSystemResource)
            {
                if (sysResourceInclude)
                {
                    _logger.LogDebug("CMSResource is system resource ({Resource}) and is included in migration", Printer.GetEntityIdentityPrint(k12CmsResource));
                }
                else
                {
                    _logger.LogDebug("CMSResource is system resource ({Resource})", Printer.GetEntityIdentityPrint(k12CmsResource));

                    var k12Classes = await GetResourceClasses(k12CmsResource.ResourceId, cancellationToken);
                    if (k12Classes.Any(x => K12SystemClass.Customizable.Contains(x.ClassName)))
                    {
                        _logger.LogDebug("CMSResource ({Resource}) contains customizable classes", Printer.GetEntityIdentityPrint(k12CmsResource));
                        if (xbkResource != null)
                        {
                            var handbookRef = HandbookReferences
                                .NotSupportedSkip<KX12M.CmsResource>()
                                .WithIdentityPrint(k12CmsResource);

                            _logger.LogInformation("CMSResource is system resource and exists in target instance ({Resource}) => skipping", Printer.GetEntityIdentityPrint(k12CmsResource));
                            _protocol.Append(handbookRef);
                            continue;
                        }

                        _logger.LogInformation("CMSResource is system resource and NOT exists in target instance ({Resource}), contains customizable classes => will be migrated",
                            Printer.GetEntityIdentityPrint(k12CmsResource));
                    }
                    else
                    {
                        var handbookRef = HandbookReferences
                            .NotSupportedSkip<KX12M.CmsResource>()
                            .WithIdentityPrint(k12CmsResource);

                        _logger.LogInformation("CMSResource is system resource and exists in target instance ({Resource}) => skipping", Printer.GetEntityIdentityPrint(k12CmsResource));
                        _protocol.Append(handbookRef);
                        continue;
                    }
                }
            }
            else
            {
                _logger.LogDebug("CMSResource is CUSTOM resource ({Resource})", Printer.GetEntityIdentityPrint(k12CmsResource));
            }

            var mapped = _resourceMapper.Map(k12CmsResource, xbkResource);
            _protocol.MappedTarget(mapped);

            try
            {
                if (mapped is { Success : true })
                {
                    var (resourceInfo, newInstance) = mapped;
                    ArgumentNullException.ThrowIfNull(resourceInfo, nameof(resourceInfo));

                    ResourceInfoProvider.ProviderObject.Set(resourceInfo);

                    _protocol.Success(k12CmsResource, resourceInfo, mapped);
                    _logger.LogEntitySetAction(newInstance, resourceInfo);

                    _primaryKeyMappingContext.SetMapping<KX12M.CmsResource>(
                        r => r.ResourceId,
                        k12CmsResource.ResourceId,
                        resourceInfo.ResourceID
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while saving resource {ResourceName}", k12CmsResource.ResourceName);
            }
        }
    }

    private DataClassInfo? SaveClassUsingKxoApi(KX12M.CmsClass k12Class, DataClassInfo kxoDataClass)
    {
        var mapped = _dataClassMapper.Map(k12Class, kxoDataClass);
        _protocol.MappedTarget(mapped);

        try
        {
            if (mapped is { Success : true } result)
            {
                var (dataClassInfo, newInstance) = result;
                ArgumentNullException.ThrowIfNull(dataClassInfo, nameof(dataClassInfo));

                _kxpClassFacade.SetClass(dataClassInfo);

                _protocol.Success(k12Class, dataClassInfo, mapped);
                _logger.LogEntitySetAction(newInstance, dataClassInfo);

                _primaryKeyMappingContext.SetMapping<KX12M.CmsClass>(
                    r => r.ClassId,
                    k12Class.ClassId,
                    dataClassInfo.ClassID
                );

                return dataClassInfo;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while saving page type {ClassName}", k12Class.ClassName);
        }

        return null;
    }
}