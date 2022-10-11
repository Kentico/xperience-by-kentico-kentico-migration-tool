namespace Migration.Toolkit.Core.Handlers;

using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Modules;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Enumerations;
using Migration.Toolkit.Common.Helpers;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.Helpers;
using Migration.Toolkit.Core.Mappers;
using Migration.Toolkit.Core.MigrationProtocol;
using Migration.Toolkit.Core.Services.BulkCopy;
using Migration.Toolkit.Core.Services.CmsClass;
using Migration.Toolkit.KX13.Context;
using Migration.Toolkit.KX13.Models;
using Migration.Toolkit.KXP.Api;
using Migration.Toolkit.KXP.Context;

public class MigrateCustomModulesCommandHandler : IRequestHandler<MigrateCustomModulesCommand, CommandResult>
{
    private readonly ILogger<MigrateCustomModulesCommandHandler> _logger;
    private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;
    private readonly IDbContextFactory<KxpContext> _kxpContextFactory;
    private readonly KxpClassFacade _kxpClassFacade;
    private readonly IEntityMapper<KX13M.CmsClass, DataClassInfo> _dataClassMapper;
    private readonly IEntityMapper<KX13M.CmsResource, ResourceInfo> _resourceMapper;
    private readonly IEntityMapper<AlternativeFormMapperSource, AlternativeFormInfo> _alternativeFormMapper;
    private readonly ToolkitConfiguration _toolkitConfiguration;
    private readonly PrimaryKeyMappingContext _primaryKeyMappingContext;
    private readonly IProtocol _protocol;
    private readonly BulkDataCopyService _bulkDataCopyService;

    public MigrateCustomModulesCommandHandler(
        ILogger<MigrateCustomModulesCommandHandler> logger,
        IDbContextFactory<KX13Context> kx13ContextFactory,
        IDbContextFactory<KxpContext> kxpContextFactory,
        KxpClassFacade kxpClassFacade,
        IEntityMapper<KX13.Models.CmsClass, DataClassInfo> dataClassMapper,
        IEntityMapper<KX13M.CmsResource, ResourceInfo> resourceMapper,
        IEntityMapper<AlternativeFormMapperSource, AlternativeFormInfo> alternativeFormMapper,
        ToolkitConfiguration toolkitConfiguration,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol,
        BulkDataCopyService bulkDataCopyService)
    {
        _logger = logger;
        _kx13ContextFactory = kx13ContextFactory;
        _kxpContextFactory = kxpContextFactory;
        _kxpClassFacade = kxpClassFacade;
        _dataClassMapper = dataClassMapper;
        _resourceMapper = resourceMapper;
        _alternativeFormMapper = alternativeFormMapper;
        _toolkitConfiguration = toolkitConfiguration;
        _primaryKeyMappingContext = primaryKeyMappingContext;
        _protocol = protocol;
        _bulkDataCopyService = bulkDataCopyService;
    }


    public async Task<CommandResult> Handle(MigrateCustomModulesCommand request, CancellationToken cancellationToken)
    {
        var entityConfiguration = _toolkitConfiguration.EntityConfigurations.GetEntityConfiguration<KX13M.CmsClass>();
        var siteIdExplicitMapping = _toolkitConfiguration.RequireExplicitMapping<KX13M.CmsSite>(s => s.SiteId);
        var migratedSiteIds = siteIdExplicitMapping.Keys.ToList();

        await MigrateResources(migratedSiteIds, cancellationToken);

        await MigrateClasses(migratedSiteIds, siteIdExplicitMapping, entityConfiguration, cancellationToken);

        return new GenericCommandResult();
    }

    private async Task MigrateClasses(List<int> migratedSiteIds, Dictionary<int, int> siteIdExplicitMapping, EntityConfiguration entityConfiguration, CancellationToken cancellationToken)
    {
        await using var kx13Context = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);
        var kx13ClassesResult = kx13Context.CmsClasses
            .Include(c => c.ClassResource).ThenInclude(cr => cr.Sites)
            .Include(c => c.Sites)
            .Where(x => !(x.ClassIsForm ?? false) && !x.ClassIsDocumentType)
            .OrderBy(x => x.ClassId)
            .AsSplitQuery();

        using var kx13Classes = EnumerableHelper.CreateDeferrableItemWrapper(kx13ClassesResult);

        while (kx13Classes.GetNext(out var di))
        {
            var (_, kx13Class) = di;

            if (kx13Class.ClassInheritsFromClassId is { } classInheritsFromClassId && !_primaryKeyMappingContext.HasMapping<KX13M.CmsClass>(c => c.ClassId, classInheritsFromClassId))
            {
                // defer migration to later stage
                if (kx13Classes.TryDeferItem(di))
                {
                    _logger.LogTrace("Class {Class} inheritance parent not found, deferring migration to end. Attempt {Attempt}", Printer.GetEntityIdentityPrint(kx13Class), di.Recurrence);
                }
                else
                {
                    _logger.LogErrorMissingDependency(kx13Class, nameof(kx13Class.ClassInheritsFromClassId), kx13Class.ClassInheritsFromClassId, typeof(DataClassInfo));
                    _protocol.Append(HandbookReferences
                        .MissingRequiredDependency<KX13M.CmsClass>(nameof(KX13M.CmsClass.ClassId), classInheritsFromClassId)
                        .NeedsManualAction()
                    );
                }

                continue;
            }

            _protocol.FetchedSource(kx13Class);

            if (!kx13Class.Sites.Any(s => migratedSiteIds.Contains(s.SiteId)) &&
                !(kx13Class.ClassResource?.Sites.Any(s => migratedSiteIds.Contains(s.SiteId)) ?? false))
            {
                // skip classes not included in current site
                _logger.LogTrace("Class {ClassName} skipped, no site mapping", kx13Class.ClassName);
                continue;
            }

            // TODO tk: 2022-10-10 deal with custom tables

            var kx13ResourceName = kx13Class.ClassResource?.ResourceName;
            if (kx13ResourceName != null && Kx13SystemResource.All.Contains(kx13ResourceName))
            {
                if (!Kx13SystemClass.Customizable.Contains(kx13Class.ClassName))
                {
                    _logger.LogInformation("Class '{Kx13ClassClassName}' is part of system resource '{Kx13ResourceName}' and is not customizable", kx13Class.ClassName, kx13Class.ClassName);
                    var handbookRef = HandbookReferences
                        .NotSupportedSkip<KX13M.CmsClass>()
                        .WithMessage($"Class '{kx13Class.ClassName}' is part of system resource '{kx13ResourceName}' and is not customizable");

                    _protocol.Append(handbookRef);
                    continue;
                }

                _logger.LogInformation("Class '{Kx13ClassClassName}' is part of system resource '{Kx13ResourceName}' and is customizable => migrated partially", kx13Class.ClassName,
                    kx13Class.ClassName);
            }

            if (entityConfiguration.ExcludeCodeNames.Contains(kx13Class.ClassName, StringComparer.InvariantCultureIgnoreCase))
            {
                _protocol.Warning(HandbookReferences.EntityExplicitlyExcludedByCodeName(kx13Class.ClassName, "CustomModule"), kx13Class);
                _logger.LogWarning("CmsClass: {ClassName} was skipped => it is explicitly excluded in configuration", kx13Class.ClassName);
                continue;
            }

            var xbkDataClass = _kxpClassFacade.GetClass(kx13Class.ClassGuid);

            _protocol.FetchedTarget(xbkDataClass);

            var dataClassId = SaveClassUsingKxoApi(kx13Class, xbkDataClass);
            if (dataClassId is { } dcId)
            {
                MigrateClassSiteMappings(siteIdExplicitMapping, dcId, dataClassId, kx13Class);

                xbkDataClass = DataClassInfoProvider.ProviderObject.Get(dcId);
                await MigrateAlternativeForms(kx13Class, xbkDataClass, cancellationToken);

                #region Migrate coupled data class data

                if (!xbkDataClass.ClassShowAsSystemTable)
                {
                    Debug.Assert(xbkDataClass.ClassTableName != null, "kx13Class.ClassTableName != null");
                    // var csi = new ClassStructureInfo(kx13Class.ClassXmlSchema, kx13Class.ClassXmlSchema, kx13Class.ClassTableName);

                    XNamespace nsSchema = "http://www.w3.org/2001/XMLSchema";
                    XNamespace msSchema = "urn:schemas-microsoft-com:xml-msdata";
                    var xDoc = XDocument.Parse(xbkDataClass.ClassXmlSchema);
                    var autoIncrementColumns = xDoc.Descendants(nsSchema + "element")
                        .Where(x => x.Attribute(msSchema + "AutoIncrement")?.Value == "true")
                        .Select(x => x.Attribute("name")?.Value).ToImmutableHashSet();

                    Debug.Assert(autoIncrementColumns.Count == 1, "autoIncrementColumns.Count == 1");
                    // TODO tk: 2022-07-08 not true : autoIncrementColumns.First() == csi.IDColumn
                    // Debug.Assert(autoIncrementColumns.First() == csi.IDColumn, "autoIncrementColumns.First() == csi.IDColumn");

                    var r = (xbkDataClass.ClassTableName, xbkDataClass.ClassGUID, autoIncrementColumns);
                    _logger.LogTrace("Class '{ClassGuild}' Resolved as: {Result}", kx13Class.ClassGuid, r);

                    // check if data is present in target tables
                    if (_bulkDataCopyService.CheckIfDataExistsInTargetTable(xbkDataClass.ClassTableName))
                    {
                        _logger.LogWarning("Data exists in target coupled data table '{TableName}' - cannot migrate, skipping form data migration", r.ClassTableName);
                        _protocol.Append(HandbookReferences.DataMustNotExistInTargetInstanceTable(xbkDataClass.ClassTableName));
                        continue;
                    }

                    var bulkCopyRequest = new BulkCopyRequest(
                        xbkDataClass.ClassTableName, s => !autoIncrementColumns.Contains(s), _ => true,
                        20000
                    );

                    _logger.LogTrace("Bulk data copy request: {Request}", bulkCopyRequest);
                    _bulkDataCopyService.CopyTableToTable(bulkCopyRequest);
                }

                #endregion
            }
        }
    }

    private async Task MigrateAlternativeForms(CmsClass kx13Class, DataClassInfo xbkDataClass, CancellationToken cancellationToken)
    {
        var kx13Context = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        var kx13AlternativeForms = kx13Context.CmsAlternativeForms
            .Include(af => af.FormClass)
            .Include(af => af.FormCoupledClass)
            .Where(af => af.FormClassId == kx13Class.ClassId);

        foreach (var kx13AlternativeForm in kx13AlternativeForms)
        {
            _protocol.FetchedSource(kx13AlternativeForm);

            var xbkAlternativeForm = AlternativeFormInfoProvider.ProviderObject.Get(kx13AlternativeForm.FormGuid);
            _protocol.FetchedTarget(xbkAlternativeForm);

            var mappingSource = new AlternativeFormMapperSource(kx13AlternativeForm, xbkDataClass);
            var mapped = _alternativeFormMapper.Map(mappingSource, xbkAlternativeForm);
            _protocol.MappedTarget(mapped);

            try
            {
                if (mapped is { Success : true })
                {
                    var (alternativeFormInfo, newInstance) = mapped;
                    ArgumentNullException.ThrowIfNull(alternativeFormInfo, nameof(alternativeFormInfo));

                    AlternativeFormInfoProvider.ProviderObject.Set(alternativeFormInfo);

                    _protocol.Success(kx13AlternativeForm, alternativeFormInfo, mapped);
                    _logger.LogEntitySetAction(newInstance, alternativeFormInfo);

                    _primaryKeyMappingContext.SetMapping<KX13M.CmsAlternativeForm>(
                        r => r.FormId,
                        kx13AlternativeForm.FormId,
                        alternativeFormInfo.FormID
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while saving alternative form {ResourceName}", kx13AlternativeForm.FormName);
            }
        }
    }

    private async Task<List<KX13M.CmsClass>> GetResourceClasses(int kx13ResourceId, CancellationToken cancellationToken)
    {
        await using var kx13Context = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        return await kx13Context.CmsClasses.Where(x => x.ClassResourceId == kx13ResourceId).ToListAsync();
    }

    private async Task MigrateResources(List<int> migratedSites, CancellationToken cancellationToken)
    {
        await using var kx13Context = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        var kx13CmsResources = kx13Context.CmsResources
            .Include(cr => cr.Sites);

        foreach (var kx13CmsResource in kx13CmsResources)
        {
            _protocol.FetchedSource(kx13CmsResource);

            var xbkResource = ResourceInfoProvider.ProviderObject.Get(kx13CmsResource.ResourceGuid);

            _protocol.FetchedTarget(xbkResource);

            var isSystemResource = Kx13SystemResource.All.Contains(kx13CmsResource.ResourceName);
            if (isSystemResource)
            {
                _logger.LogDebug("CMSResource is system resource ({Resource})", Printer.GetEntityIdentityPrint(kx13CmsResource));

                var kx13Classes = await GetResourceClasses(kx13CmsResource.ResourceId, cancellationToken);
                if (kx13Classes.Any(x => Kx13SystemClass.Customizable.Contains(x.ClassName)))
                {
                    _logger.LogDebug("CMSResource ({Resource}) contains customizable classes", Printer.GetEntityIdentityPrint(kx13CmsResource));
                    if (xbkResource != null)
                    {
                        var handbookRef = HandbookReferences
                            .NotSupportedSkip<KX13M.CmsResource>()
                            .WithIdentityPrint(kx13CmsResource);

                        _logger.LogInformation("CMSResource is system resource and exists in target instance ({Resource}) => skipping", Printer.GetEntityIdentityPrint(kx13CmsResource));
                        _protocol.Append(handbookRef);
                        continue;
                    }

                    _logger.LogInformation("CMSResource is system resource and NOT exists in target instance ({Resource}), contains customizable classes => will be migrated",
                        Printer.GetEntityIdentityPrint(kx13CmsResource));
                }
                else
                {
                    var handbookRef = HandbookReferences
                        .NotSupportedSkip<KX13M.CmsResource>()
                        .WithIdentityPrint(kx13CmsResource);

                    _logger.LogInformation("CMSResource is system resource and exists in target instance ({Resource}) => skipping", Printer.GetEntityIdentityPrint(kx13CmsResource));
                    _protocol.Append(handbookRef);
                    continue;
                }
            }
            else
            {
                _logger.LogDebug("CMSResource is CUSTOM resource ({Resource})", Printer.GetEntityIdentityPrint(kx13CmsResource));
            }

            if (!kx13CmsResource.Sites.Any(s => migratedSites.Contains(s.SiteId)))
            {
                _logger.LogDebug("CMSResource '{ResourceName}' is not included in migrated site => skipping", kx13CmsResource.ResourceName);
                continue;
            }

            var mapped = _resourceMapper.Map(kx13CmsResource, xbkResource);
            _protocol.MappedTarget(mapped);

            try
            {
                if (mapped is { Success : true })
                {
                    var (resourceInfo, newInstance) = mapped;
                    ArgumentNullException.ThrowIfNull(resourceInfo, nameof(resourceInfo));

                    ResourceInfoProvider.ProviderObject.Set(resourceInfo);

                    _protocol.Success(kx13CmsResource, resourceInfo, mapped);
                    _logger.LogEntitySetAction(newInstance, resourceInfo);

                    _primaryKeyMappingContext.SetMapping<KX13M.CmsResource>(
                        r => r.ResourceId,
                        kx13CmsResource.ResourceId,
                        resourceInfo.ResourceID
                    );

                    // migrate site resource mapping
                    MigrateResourceToSiteMapping(migratedSites, resourceInfo, kx13CmsResource);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while saving resource {ResourceName}", kx13CmsResource.ResourceName);
            }
        }
    }

    private void MigrateResourceToSiteMapping(List<int> migratedSites, ResourceInfo resourceInfo, KX13M.CmsResource kx13CmsResource)
    {
        foreach (var migratedSiteId in migratedSites)
        {
            var xbkSiteId = _primaryKeyMappingContext.RequireMapFromSource<KX13M.CmsSite>(s => s.SiteId, migratedSiteId);
            try
            {
                var resourceSiteInfo = ResourceSiteInfo.New(rsi =>
                {
                    rsi.ResourceID = resourceInfo.ResourceID;
                    rsi.SiteID = xbkSiteId;
                });
                ResourceSiteInfoProvider.ProviderObject.Set(resourceSiteInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while saving resource '{ResourceName}' to siteId '{SiteId}' mapping", kx13CmsResource.ResourceName, xbkSiteId);
            }
        }
    }

    private int? SaveClassUsingKxoApi(KX13M.CmsClass kx13Class, DataClassInfo kxoDataClass)
    {
        var mapped = _dataClassMapper.Map(kx13Class, kxoDataClass);
        _protocol.MappedTarget(mapped);

        try
        {
            if (mapped is { Success : true } result)
            {
                var (dataClassInfo, newInstance) = result;
                ArgumentNullException.ThrowIfNull(dataClassInfo, nameof(dataClassInfo));

                _kxpClassFacade.SetClass(dataClassInfo);

                _protocol.Success(kx13Class, dataClassInfo, mapped);
                _logger.LogEntitySetAction(newInstance, dataClassInfo);

                _primaryKeyMappingContext.SetMapping<KX13M.CmsClass>(
                    r => r.ClassId,
                    kx13Class.ClassId,
                    dataClassInfo.ClassID
                );

                return dataClassInfo.ClassID;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while saving page type {ClassName}", kx13Class.ClassName);
        }

        return null;
    }

    private void MigrateClassSiteMappings(Dictionary<int, int> siteIdMapping, int dcId, [DisallowNull] int? dataClassId, KX13M.CmsClass kx13Class)
    {
        foreach (var (sourceSiteId, targetSiteId) in siteIdMapping)
        {
            try
            {
                var classSiteInfo = ClassSiteInfo.New();
                classSiteInfo.ClassID = dcId;
                classSiteInfo.SiteID = targetSiteId;
                ClassSiteInfoProvider.ProviderObject.Set(classSiteInfo);

                _protocol.Success(new { dataClassId, targetSiteId }, classSiteInfo, null);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to create target instance");
                _protocol.Append(HandbookReferences
                    .ErrorCreatingTargetInstance<ClassSiteInfo>(exception)
                    .NeedsManualAction()
                    .WithData(new
                    {
                        exception,
                        sourceSiteId,
                        targetSiteId,
                        dataClassId,
                        sourceClassId = kx13Class.ClassId
                    })
                );
            }
        }
    }
}