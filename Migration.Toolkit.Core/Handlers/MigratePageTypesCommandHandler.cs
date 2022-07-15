namespace Migration.Toolkit.Core.Handlers;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using CMS.DataEngine;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Helpers;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.Helpers;
using Migration.Toolkit.Core.MigrationProtocol;
using Migration.Toolkit.KX13.Context;
using Migration.Toolkit.KX13.Models;
using Migration.Toolkit.KXO.Api;

public class MigratePageTypesCommandHandler : IRequestHandler<MigratePageTypesCommand, CommandResult>
{
    private readonly ILogger<MigratePageTypesCommandHandler> _logger;
    private readonly IEntityMapper<CmsClass, DataClassInfo> _dataClassMapper;
    private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;
    private readonly PrimaryKeyMappingContext _primaryKeyMappingContext;
    private readonly KxoClassFacade _kxoClassFacade;
    private readonly IMigrationProtocol _migrationProtocol;
    private readonly ToolkitConfiguration _toolkitConfiguration;

    public MigratePageTypesCommandHandler(
        ILogger<MigratePageTypesCommandHandler> logger,
        IEntityMapper<CmsClass, DataClassInfo> dataClassMapper,
        IDbContextFactory<KX13Context> kx13ContextFactory,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        KxoClassFacade kxoClassFacade,
        IMigrationProtocol migrationProtocol,
        ToolkitConfiguration toolkitConfiguration
    )
    {
        _logger = logger;
        _dataClassMapper = dataClassMapper;
        _kx13ContextFactory = kx13ContextFactory;
        _primaryKeyMappingContext = primaryKeyMappingContext;
        _kxoClassFacade = kxoClassFacade;
        _migrationProtocol = migrationProtocol;
        _toolkitConfiguration = toolkitConfiguration;
    }

    public async Task<CommandResult> Handle(MigratePageTypesCommand request, CancellationToken cancellationToken)
    {
        await using var kx13Context = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        Debug.Assert(_toolkitConfiguration.EntityConfigurations != null, "_toolkitConfiguration.EntityConfigurations != null");
        var entityConfiguration = _toolkitConfiguration.EntityConfigurations.GetEntityConfiguration<CmsClass>();
        
        var siteIdExplicitMapping = _toolkitConfiguration.RequireExplicitMapping<CmsSite>(s => s.SiteId);
        var migratedSiteIds = siteIdExplicitMapping.Keys.ToList();

        var cmsClassesDocumentTypes = kx13Context.CmsClasses.Include(c => c.Sites)
                .Where(x => x.ClassIsDocumentType).OrderBy(x => x.ClassId).AsEnumerable()
            ;

        using var kx13Classes = EnumerableHelper.CreateDeferrableItemWrapper(cmsClassesDocumentTypes);
        
        while(kx13Classes.GetNext(out var di))
        {
            var (_, kx13Class) = di;
            
            if (kx13Class.ClassInheritsFromClassId is { } classInheritsFromClassId && !_primaryKeyMappingContext.HasMapping<CmsClass>(c=> c.ClassId, classInheritsFromClassId))
            {
                // defer migration to later stage 
                if (kx13Classes.TryDeferItem(di))
                {
                    _logger.LogTrace("Class {Class} inheritance parent not found, deferring migration to end. Attempt {Attempt}", Printer.GetEntityIdentityPrint(kx13Class), di.Recurrence);
                }
                else
                {
                    _logger.LogErrorMissingDependency(kx13Class, nameof(kx13Class.ClassInheritsFromClassId), kx13Class.ClassInheritsFromClassId, typeof(DataClassInfo));
                    _migrationProtocol.Append(HandbookReferences
                        .MissingRequiredDependency<CmsClass>(nameof(CmsClass.ClassId), classInheritsFromClassId)
                        .NeedsManualAction()
                    );
                }

                continue;
            }

            if (!kx13Class.Sites.Any(s => migratedSiteIds.Contains(s.SiteId)))
            {
                // skip classes not included in current site
                _logger.LogTrace("Class {ClassName} skipped, no site mapping", kx13Class.ClassName);
                continue;
            }
            
            _migrationProtocol.FetchedSource(kx13Class);

            if (entityConfiguration.ExcludeCodeNames.Contains(kx13Class.ClassName, StringComparer.InvariantCultureIgnoreCase))
            {
                _migrationProtocol.Warning(HandbookReferences.EntityExplicitlyExcludedByCodeName(kx13Class.ClassName, "PageType"), kx13Class);
                _logger.LogWarning("CmsClass: {ClassName} was skipped => it is explicitly excluded in configuration", kx13Class.ClassName);
                continue;    
            }
            
            if (kx13Class.ClassName == "CMS.Root")
            {
                _migrationProtocol.Warning(HandbookReferences.CmsClassCmsRootClassTypeSkip, kx13Class);
                _logger.LogWarning("CmsClass: {ClassName} was skipped => CMS.Root cannot be migrated", kx13Class.ClassName);
                continue;
            }

            // kx13Class.ClassConnectionString check is not necessary

            var kxoDataClass = _kxoClassFacade.GetClass(kx13Class.ClassGuid);
            _migrationProtocol.FetchedTarget(kxoDataClass);

            var dataClassId = SaveUsingKxoApi(kx13Class, kxoDataClass);
            if (dataClassId is { } dcId)
            {
                MigrateClassSiteMappings(siteIdExplicitMapping, dcId, dataClassId, kx13Class);
            }
        }

        return new GenericCommandResult();
    }

    private void MigrateClassSiteMappings(Dictionary<int, int> siteIdMapping, int dcId, [DisallowNull] int? dataClassId, CmsClass kx13Class)
    {
        foreach (var (sourceSiteId, targetSiteId) in siteIdMapping)
        {
            try
            {
                var classSiteInfo = ClassSiteInfo.New();
                classSiteInfo.ClassID = dcId;
                classSiteInfo.SiteID = targetSiteId;
                ClassSiteInfoProvider.ProviderObject.Set(classSiteInfo);

                _migrationProtocol.Success(new { dataClassId, targetSiteId }, classSiteInfo, null);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to create target instance");
                _migrationProtocol.Append(HandbookReferences
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

    private int? SaveUsingKxoApi(CmsClass kx13Class, DataClassInfo kxoDataClass)
    {
        var mapped = _dataClassMapper.Map(kx13Class, kxoDataClass);
        _migrationProtocol.MappedTarget(mapped);

        try
        {
            if (mapped is { Success : true } result)
            {
                var (dataClassInfo, newInstance) = result;
                ArgumentNullException.ThrowIfNull(dataClassInfo, nameof(dataClassInfo));

                _kxoClassFacade.SetClass(dataClassInfo);

                _migrationProtocol.Success(kx13Class, dataClassInfo, mapped);
                _logger.LogEntitySetAction(newInstance, dataClassInfo);

                _primaryKeyMappingContext.SetMapping<CmsClass>(
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
}